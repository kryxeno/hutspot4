using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplestMovement;
using Unity.Netcode;

public class MovingPlatform : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int startingPoint;
    [SerializeField] private Transform[] points;
    private int i;

    private Vector3 lastFramePosition;
    public Vector3 currentVelocity { get; private set; }


    //Stuff that adds players, then makes sure they move
    [SerializeField] private SimplestFirstPersonController[] playerControllers;
    [SerializeField] private int playersOnPlatform;

    public override void OnNetworkSpawn()
    {
        transform.position = points[startingPoint].position;
        playerControllers = new SimplestFirstPersonController[4];
    }



    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, points[i].position) < 0.2f)
        {
            i++;
            if (i == points.Length) i = 0;
        }

        Vector3 moveUpdate = (points[i].position - transform.position).normalized * speed * Time.deltaTime;

        transform.position += moveUpdate; //Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        //if we have controllers to update we do this
        for (int i = 0; i < playerControllers.Length; i++)
        {
            if (playerControllers[i] == null) break;
            else playerControllers[i].MovePlayer(moveUpdate);
        }        
        

        if (lastFramePosition == null) lastFramePosition = transform.position;
        else
        {
            currentVelocity = transform.position - lastFramePosition;
            lastFramePosition = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        playerControllers[playersOnPlatform] = other.GetComponent<SimplestFirstPersonController>();
        playersOnPlatform++;
        
    }

    private void OnTriggerExit(Collider other)
    {

        playersOnPlatform--;
        playerControllers[playersOnPlatform] = null;

    }
}
