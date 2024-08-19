using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController2 : MonoBehaviour
{
    [Header("Main References")]
    private Rigidbody shipRb;

    [Header("ThrustFlight Attributes")]
    [SerializeField] private float thrustForce;
    [SerializeField] private float drag;


    [Header("Mouse Controls")]
    [SerializeField] private Camera followCam;
    [SerializeField] private float turnSpeed;

    private void Awake()
    {
        shipRb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        FlyControls();
    }

    private void FixedUpdate()
    {
        Vector3 targetAlignment = followCam.transform.forward;
        transform.rotation = Quaternion.Lerp(transform.rotation, followCam.transform.rotation, turnSpeed * Time.deltaTime);
    }

    private void FlyControls()
    {
        Vector3 inputVector = InputVector();

        //Since the input vector is in a global space, we need to translate it to a local one
        Vector3 localVector = transform.right * inputVector.x + transform.up * inputVector.y + transform.forward * inputVector.z;
        shipRb.AddForce(localVector * thrustForce * Time.deltaTime);

        //While the player is not putting in any inputs, lets go towards a zero velocity
        if (inputVector == Vector3.zero)
        {
            //Set the speed to zero if we are almost there to prevent overshooting
            if (shipRb.velocity.magnitude <= 0.1f)
            {
                shipRb.velocity = Vector3.zero;
            }
            else //Otherwise put some force in the opposite direction of your movement
            {
                Vector3 dragVector = -shipRb.velocity.normalized;
                shipRb.AddForce(drag * dragVector * Time.deltaTime);
            }
        }

        
    }

    private Vector3 InputVector()
    {
        Vector3 iVec = Vector3.zero;
        //Up and down
        if (Input.GetKey(KeyCode.Space)) { iVec.y += 1; }
        if (Input.GetKey(KeyCode.LeftShift)) { iVec.y -= 1; }

        //Left and Right
        if (Input.GetKey(KeyCode.D)) { iVec.x += 1; }
        if (Input.GetKey(KeyCode.A)) { iVec.x -= 1; }

        //Forward and backward
        if (Input.GetKey(KeyCode.W)) { iVec.z += 1; }
        if (Input.GetKey(KeyCode.S)) { iVec.z -= 1; }

        return iVec;
    }
}
