using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookAround : MonoBehaviour
{
    float rotationX = 0f;
    float rotationY = 0f;

    [SerializeField] private float sensitivity;
    [SerializeField] private float clampAngle;

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationX -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY += Input.GetAxis("Mouse X") * sensitivity;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        //transform.localEulerAngles = new Vector3(rotationX, rotationY, 0f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
