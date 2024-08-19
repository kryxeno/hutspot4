using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace KinematicCharacterController.Examples
{
    public class ExampleMovingPlatform : NetworkBehaviour, IMoverController
    {
        public PhysicsMover Mover;

        public Vector3 TranslationAxis = Vector3.right;
        public float TranslationPeriod = 10;
        public float TranslationSpeed = 1;
        public Vector3 RotationAxis = Vector3.up;
        public float RotSpeed = 10;
        public Vector3 OscillationAxis = Vector3.zero;
        public float OscillationPeriod = 10;
        public float OscillationSpeed = 10;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private Vector3 _receivedGoalPosition = Vector3.zero;
        private Quaternion _receivedGoalRotation = quaternion.identity;

        private void Start()
        {
            _originalPosition = Mover.Rigidbody.position;
            _originalRotation = Mover.Rigidbody.rotation;

            Mover.MoverController = this;
        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            if (!IsServer)
            {
                goalPosition = _receivedGoalPosition;
                goalRotation = _receivedGoalRotation;
                return;
            }

            goalPosition = _originalPosition + (Mathf.Sin(Time.time * TranslationSpeed) * TranslationPeriod * TranslationAxis.normalized);

            Quaternion targetRotForOscillation = Quaternion.Euler(OscillationAxis.normalized * (Mathf.Sin(Time.time * OscillationSpeed) * OscillationPeriod)) * _originalRotation;
            goalRotation = Quaternion.Euler(RotSpeed * Time.time * RotationAxis) * targetRotForOscillation;

            UpdateStateServerRpc(goalPosition, goalRotation);
        }


        [ServerRpc]
        void UpdateStateServerRpc(Vector3 goalPosition, Quaternion goalRotation)
        {
            UpdateStateClientRpc(goalPosition, goalRotation);
        }

        [ClientRpc]
        void UpdateStateClientRpc(Vector3 goalPosition, Quaternion goalRotation)
        {
            if (IsServer) return;
            _receivedGoalPosition = goalPosition;
            _receivedGoalRotation = goalRotation;
        }
    }
}