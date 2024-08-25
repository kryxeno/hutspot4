using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class SetCameraPriorityForNetwork : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            virtualCam.Priority = 0;
        }
    }
}
