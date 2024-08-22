using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipCamBehaviour : MonoBehaviour
{
    [SerializeField] Camera mainCam;

    // Update is called once per frame
    void Update()
    {
        mainCam.transform.position = transform.position;
    }
}
