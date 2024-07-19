using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Test : NetworkBehaviour
{
    public TextMeshProUGUI text;
    private float timeSinceLastPong = 0f;
    private const float pongInterval = 3f;

    override public void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string owner = IsHost ? "owner" : "client";
            text.text = $"I am the {owner}";
            Debug.Log(text.text);
        }
        else
        {
            text.enabled = false;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PongClientRpc(Time.frameCount, "hello, world", OwnerClientId); // Server -> Client
        }

        // Update the time since the last pong
        timeSinceLastPong += Time.deltaTime;

        // Check if 10 seconds have passed
        if (timeSinceLastPong >= pongInterval && IsOwner)
        {
            PongServerRpc(transform.position, new ServerRpcParams()); // Client -> Server

            // Reset the timer
            timeSinceLastPong = 0f;
        }
    }

    [ClientRpc]
    void PongClientRpc(int somenumber, string sometext, ulong _OwnerClientId = 0)
    {
        text.text = $"{_OwnerClientId} : {somenumber} : {sometext}";
        Debug.Log(text.text);
    }

    [ServerRpc(RequireOwnership = false)]
    void PongServerRpc(Vector3 position, ServerRpcParams serverRpcParams)
    {
        Debug.Log($"Server received {position} from {serverRpcParams.Receive.SenderClientId}");
    }
}
