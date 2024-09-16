using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace SpaceshipPrototyping
{
    public class SpaceshipController : MonoBehaviour
    {
        private Hutspot4Controls playerInput;
        private InputAction move;
        private InputAction look;


        [Header("Camera")]
        Vector2 mouseDelta;
        [SerializeField] CinemachineVirtualCamera virtualCam;
        [SerializeField] float sensitity;
        [SerializeField] float maxYAngle;
        [SerializeField] float maxXAngle;
        float cameraVerticalRotation;
        float cameraHorizontalRotation;

        [Header("Flying")]
        [SerializeField] float flySpeed;
        [SerializeField] float moveResponsiveness;
        [SerializeField] float rotateResponsiveness;
        Vector3 targetVec;

        [Header("Landing")]
        [SerializeField] Transform landTarget;
        [SerializeField] float landSmoothing;
        [SerializeField] float landDistanceThreshold;
        [SerializeField] float landAngleThreshold;
        [SerializeField] float landRotateSmoothing;


        private enum AirshipState
        {
            Flying,
            Grounded,
            Landing,
            Takeoff,
        }

        [SerializeField] private AirshipState airshipState;


        private void Awake()
        {
            playerInput = new Hutspot4Controls();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            airshipState = AirshipState.Flying;
        }

        private void OnEnable()
        {
            move = playerInput.Spaceship.Move;
            move.Enable();

            look = playerInput.Spaceship.Look;
            look.Enable();

            playerInput.Spaceship.ToggleLanding.performed += ToggleLanding;
            playerInput.Spaceship.ToggleLanding.Enable();
        }

        private void OnDisable()
        {
            move.Disable();
            look.Disable();

            playerInput.Spaceship.ToggleLanding.performed -= ToggleLanding;
            playerInput.Spaceship.ToggleLanding.Disable();
        }



        private void Update()
        {
            RotateCam();
            if (airshipState == AirshipState.Flying)
            {
                MoveAirship();
                TurnAirship();
            }
        }


        private void RotateCam()
        {
            mouseDelta = look.ReadValue<Vector2>();

            //Vertical camera rotation
            cameraVerticalRotation -= mouseDelta.y * sensitity * Time.deltaTime;
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -maxYAngle, maxYAngle);

            //horizontal camera Rotation
            cameraHorizontalRotation += mouseDelta.x * sensitity * Time.deltaTime;
            float centerRotation = transform.eulerAngles.y;
            cameraHorizontalRotation = Mathf.Clamp(cameraHorizontalRotation, centerRotation - maxXAngle, centerRotation + maxXAngle);

            //Actual rotation
            virtualCam.transform.rotation = Quaternion.Euler(cameraVerticalRotation, cameraHorizontalRotation, 0f);
        }

        private void MoveAirship()
        {
            Vector3 moveVec = move.ReadValue<Vector3>();

            moveVec = Quaternion.LookRotation(transform.forward) * moveVec;

            targetVec = Vector3.Lerp(targetVec, moveVec.normalized, Time.deltaTime * moveResponsiveness);

            transform.position += targetVec * flySpeed * Time.deltaTime;
        }

        private void TurnAirship()
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, virtualCam.transform.rotation, Time.deltaTime * rotateResponsiveness);
        }

        private void ToggleLanding(InputAction.CallbackContext obj)
        {
            switch (airshipState)
            {
                case AirshipState.Flying or AirshipState.Takeoff:
                    //Go land if we have a target
                    StopLandingCoroutines();
                    StartCoroutine("AlignWithTarget", landTarget);
                    return;

                case AirshipState.Grounded:
                    //Start flying, the coroutine is supposed to set the correct state
                    StopLandingCoroutines();
                    StartCoroutine("Takeoff", landTarget.position + Vector3.up * 5f);
                    return;

                case AirshipState.Landing:
                    //Stop landing and set state to flying
                    StopLandingCoroutines();
                    airshipState = AirshipState.Flying;
                    return;
            }

        }

        private void StopLandingCoroutines()
        {
            StopCoroutine("AlignWithTarget");
            StopCoroutine("LandToTarget");
            StopCoroutine("Takeoff");
        }


        IEnumerator AlignWithTarget(Transform target)
        {
            //Turn off the controls, and land the ship

            airshipState = AirshipState.Landing;

            float angle = Vector3.Angle(transform.forward, target.forward);

            float rotationTime = 0;

            while (angle > landAngleThreshold)
            {
                angle = Vector3.Angle(transform.forward, target.forward);
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotationTime);
                rotationTime += Time.deltaTime * landRotateSmoothing;
                yield return null;
            }

            Debug.Log("Finished Alignment");
            StartCoroutine("LandToTarget", landTarget);
            yield return null;

        }


        IEnumerator LandToTarget(Transform target)
        {
            float landTime = 0;

            float distanceToTarget = (transform.position - target.position).magnitude;
            while (distanceToTarget > landDistanceThreshold)
            {
                //Make sure the distance is closed
                distanceToTarget = (transform.position - target.position).magnitude;
                transform.position = Vector3.Slerp(transform.position, target.position, landTime);
                landTime += Time.deltaTime * landSmoothing;
                yield return null;
            }

            Debug.Log("Finished Landing");
            //Make sure the airship state is set to landing
            airshipState = AirshipState.Grounded;
            yield return null;
        }

        IEnumerator Takeoff(Vector3 target)
        {
            float takeoffTime = 0;

            airshipState = AirshipState.Takeoff;

            float distanceToTarget = (transform.position - target).magnitude;
            while (distanceToTarget > landDistanceThreshold)
            {
                //Make sure the distance is closed
                distanceToTarget = (transform.position - target).magnitude;
                transform.position = Vector3.Slerp(transform.position, target, takeoffTime);
                takeoffTime += Time.deltaTime * landSmoothing;
                yield return null;
            }

            Debug.Log("Finished Takeoff");

            //Make sure the airship state is set to flying again
            airshipState = AirshipState.Flying;
            yield return null;
        }
    }
}

