using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LobbyCleanup : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (TestMultiplayer.Instance != null)
        {
            Destroy(TestMultiplayer.Instance.gameObject);
        }
    }
}
