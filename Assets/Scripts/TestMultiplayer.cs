using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestMultiplayer : MonoBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;

    public static TestMultiplayer Instance { get; private set; }

    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData { clientId = clientId });
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerDataNetworkList.Count > playerIndex;
    }
}
