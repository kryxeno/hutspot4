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
            text.text = $"{owner}";
            Debug.Log(text.text);
        }
        else
        {
            text.enabled = false;
        }
    }
    void Update()
    {
        // Update the time since the last pong
        timeSinceLastPong += Time.deltaTime;

        // Check if 10 seconds have passed
        if (timeSinceLastPong >= pongInterval && IsOwner)
        {
            // Reset the timer
            timeSinceLastPong = 0f;
        }
    }
}
