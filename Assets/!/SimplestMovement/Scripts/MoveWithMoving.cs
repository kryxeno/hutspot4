using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMoving : MonoBehaviour
{
    [SerializeField] private float raycastLength;
    [SerializeField] private Transform raycastStart;
    [SerializeField] private LayerMask moveLayer;

    private bool hasMover;

    private Vector3 lastMoverPos;
    private Vector3 moverVelocity;


    private void Awake()
    {
        hasMover = false;
    }

    void LateUpdate()
    {
        RaycastHit hit;
        bool hitsMover = Physics.Raycast(raycastStart.position, -transform.up, out hit, raycastLength, moveLayer);
        if (hitsMover && !hasMover)
        {
            hasMover = true;
        }

        if (!hitsMover)
        {
            hasMover = false;
        }

        if (hasMover)
        {
            // It is important to have a correct lastMoverPos here already... could cause bugs this way
            if (lastMoverPos == null) lastMoverPos = hit.transform.position;

            if ((lastMoverPos - hit.transform.position).magnitude > 0.2f) lastMoverPos = hit.transform.position;

            moverVelocity = hit.transform.position - lastMoverPos;
            transform.position += moverVelocity;

            lastMoverPos = hit.transform.position;
        }
    }
}
