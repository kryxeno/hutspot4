using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using Unity.Netcode;

public class SimplePlatformMove : NetworkBehaviour, IMoverController
{
    public PhysicsMover Mover;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    //Moving things
    public Vector3 TranslationAxis = Vector3.right;
    public float TranslationPeriod = 10;
    public float TranslationSpeed = 1;
    public Vector3 RotationAxis = Vector3.up;
    public float RotSpeed = 10;
    public Vector3 OscillationAxis = Vector3.zero;
    public float OscillationPeriod = 10;
    public float OscillationSpeed = 10;


    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            Mover.enabled = true;
            _originalPosition = Mover.Rigidbody.position;
            _originalRotation = Mover.Rigidbody.rotation;
            Mover.MoverController = this;
        }
    }

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        if (IsServer)
        {
            UpdateStateClientRpc(Mover.GetState());
        }

        goalPosition = _originalPosition + (Mathf.Sin(Time.time * TranslationSpeed) * TranslationPeriod * TranslationAxis.normalized);

        Quaternion targetRotForOscillation = Quaternion.Euler(OscillationAxis.normalized * (Mathf.Sin(Time.time * OscillationSpeed) * OscillationPeriod)) * _originalRotation;
        goalRotation = Quaternion.Euler(RotSpeed * Time.time * RotationAxis) * targetRotForOscillation;
    }

    private void FixedUpdate()
    {
        
    }


    [ClientRpc]
    private void UpdateStateClientRpc(PhysicsMoverState receivedState)
    {
        Mover.ApplyState(receivedState);
    }

}
