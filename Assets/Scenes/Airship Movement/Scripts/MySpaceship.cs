using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System.Linq;

    public class MySpaceship : MonoBehaviour
    {
        public SpaceshipCamera OrbitCamera;
        public Transform CameraFollowPoint;
        public MySpaceshipController Spaceship;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";
        private const string UpInput = "Jump"; //Mapped to spacebar
        private const string DownInput = "Fire3"; //Mapped to shift, and also middle mouse bttn weirdly

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            OrbitCamera.SetFollowTransform(CameraFollowPoint);

            // Ignore the Spaceship's collider(s) for camera obstruction checks
            OrbitCamera.IgnoredColliders.Clear();
            OrbitCamera.IgnoredColliders.AddRange(Spaceship.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleSpaceshipInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);

            // Apply inputs to the camera
            OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (Input.GetMouseButtonDown(1))
            {
                OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f) ? OrbitCamera.DefaultDistance : 0f;
            }
        }

        private void HandleSpaceshipInput()
        {
            SpaceshipInputs SpaceshipInputs = new SpaceshipInputs();

            // Build the SpaceshipInputs struct
            SpaceshipInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            SpaceshipInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            SpaceshipInputs.MoveAxisUp = GetUpDownInput();
            SpaceshipInputs.CameraRotation = OrbitCamera.Transform.rotation;

            // Apply inputs to Spaceship
            Spaceship.SetInputs(ref SpaceshipInputs);
        }

        private float GetUpDownInput()
    {
        if (Input.GetButton(UpInput))
        {
            return 1;
        }
        else if (Input.GetButton(DownInput))
        {
            return -1;
        }
        else return 0;
        
    }
    }