using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LobbyCleanup : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null || TestMultiplayer.Instance != null || TestLobby.Instance != null)
        {
            if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
            if (TestMultiplayer.Instance != null) Destroy(TestMultiplayer.Instance.gameObject);
            if (TestLobby.Instance != null) Destroy(TestLobby.Instance.gameObject);
            Loader.LoadAddative(Loader.Scene.ManagerScene);
        }
        else
        {
            Loader.LoadAddative(Loader.Scene.ManagerScene);
        }
    }
}
