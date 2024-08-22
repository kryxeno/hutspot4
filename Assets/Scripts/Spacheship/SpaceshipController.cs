using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [Header("Main References")]
    private Rigidbody shipRb;

    //Hovering variables
    [Header("Hover Attributes")]
    [SerializeField] private bool hoverModeOn;
    [SerializeField] private float hoverForce;
    [SerializeField] [Range(0f, 1f) ]private float hoverVariance;
    [SerializeField] private float minHoverHeight;
    [SerializeField] private float maxHoverHeight;



    [Header("Flying attributes")]
    [SerializeField] [Range(0,1)] private float throttle;
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float bankMult;
    [SerializeField] [Range(0f,4f)] private float vh_ratio;

    private float vInput;
    private float hInput;

    private void Awake()
    {
        shipRb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        FlyControls();
        if (hoverModeOn)
        {
            Hover();
        }
        else
        {
            shipRb.useGravity = true;
        }

        //Toggle hover with H
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHoverMode();
        }
    }


    private void FlyControls()
    {
        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");

        //Handle vertical rotation and input
        transform.Rotate(Vector3.right * vInput * Time.deltaTime * turnSpeed);

        //Handle horizontal rotation and input
        transform.Rotate(-Vector3.forward * hInput * Time.deltaTime * turnSpeed * vh_ratio);

        if (Input.GetKeyDown(KeyCode.Z) && throttle <= 1)
        {
            throttle += 0.1f;
            if (throttle > 1) { throttle = 1; }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            throttle -= 0.1f;
            if (throttle < 0) { throttle = 0; }
        }

        ApplyThrust();
        ApplyDrag();
    }


    private void ApplyThrust()
    {
        if (shipRb.velocity.magnitude < maxSpeed)
        {
            //Add some thrust vectoring for when you are banking
            Vector3 targetVector = transform.forward + new Vector3(-transform.rotation.z * bankMult, 0f, 0f);
            targetVector = targetVector.normalized;
            Debug.DrawRay(transform.position, targetVector * 20f);
            shipRb.AddForce(targetVector * thrustForce * Time.deltaTime * throttle);
        }
    }

    private void ApplyDrag()
    {
        Vector3 flightVector = shipRb.velocity.normalized;
        shipRb.AddForce(-flightVector * drag * Time.deltaTime);

        //If the ship is moving very slowly, set it to 0
        if (shipRb.velocity.magnitude < 0.1f)
        {
            shipRb.velocity = Vector3.zero;
        }
    }


    private void ToggleHoverMode()
    {
        if (hoverModeOn) { hoverModeOn = false; }
        else { hoverModeOn = true; }
    }


    private void Hover()
    {
        //Draw min ray
        Debug.DrawRay(transform.position + Vector3.forward, Vector3.down * minHoverHeight, Color.red);
        //Draw max ray
        Debug.DrawRay(transform.position, Vector3.down * maxHoverHeight, Color.green);


        //Another Idea is to start hovering, and turn off gravity when we have reached the right height.

        if (Physics.Raycast(transform.position, Vector3.down, maxHoverHeight))
        {
            shipRb.AddForce(Vector3.up * hoverForce * Time.deltaTime, ForceMode.Force);
        }
        else
        {
            shipRb.useGravity = false;
        }

        



        //Start hovering while the maxhover is in range
        //Stop when it gets out of range
        //Only start again when it is back in range


        /*
        //Do the hovering if you are between min and max hover distance

        if (Physics.Raycast(transform.position, Vector3.down, maxHoverHeight))
        {
            if (strongHover)
            {
                shipRb.AddForce(Vector3.up * hoverForce * Time.deltaTime, ForceMode.Force);
            }
            else
            {
                shipRb.AddForce(Vector3.up * hoverForce * Time.deltaTime * hoverVariance, ForceMode.Force);
            }   
        }
        

        if (Physics.Raycast(transform.position, Vector3.down, minHoverHeight))
        {
            strongHover = true;
        }
        else
        {
            strongHover = false;
        }

        //Cap the vertical speed no matter how much force is applied
        if (shipRb.velocity.y > maxVertSpeed)
        {
            shipRb.velocity = new Vector3(shipRb.velocity.x, maxVertSpeed, shipRb.velocity.z);
        }
        */


        //Or maybe turn off the gravity when we reach the max hover or something, and turn it back on when we get close to the ground.
    }





}
