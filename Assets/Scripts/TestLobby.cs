using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using QFSW.QC;
using System;

public class TestLobby : MonoBehaviour
{
    public static TestLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;

    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private float lobbyListUpdateTimer;
    private string playerName;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }


    void Update()
    {
        HandleLobbyHeatbeat();
        HandleLobbyPollForUpdates();
        HandlePeriodicLobbyListUpdate();
    }

    private async void HandleLobbyHeatbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                try
                {
                    Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobby.Id);
                    joinedLobby = lobby;
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log("Error polling for updates: " + e.Message);
                }
            }
        }
    }

    private void HandlePeriodicLobbyListUpdate()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn)
        {
            lobbyListUpdateTimer -= Time.deltaTime;
            if (lobbyListUpdateTimer < 0f)
            {
                float lobbyListUpdateTimerMax = 3f;
                lobbyListUpdateTimer = lobbyListUpdateTimerMax;
                ListLobbies();
            }
        }
    }

    [Command]
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { "LobbyName", new DataObject(DataObject.VisibilityOptions.Public, lobbyName) },
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Story") },
                }
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, TestMultiplayer.MAX_PLAYER_AMOUNT, createLobbyOptions);

            TestMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
            // Debug.Log("Lobby created: " + lobby.Name + ' ' + lobby.MaxPlayers + ' ' + lobby.Id + ' ' + lobby.LobbyCode);
            // PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error creating lobby: " + e.Message);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter> {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder> {
                    new(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = queryResponse.Results });

            // Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            // foreach (Lobby lobby in queryResponse.Results)
            // {
            //     Debug.Log("Lobby: " + lobby.Name + ' ' + lobby.MaxPlayers + ' ' + lobby.Data["GameMode"].Value);
            // }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error creating lobby: " + e.Message);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    [Command]
    public async void JoinLobbyByCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            TestMultiplayer.Instance.StartClient();

            Debug.Log("Joined lobby: " + joinedLobby.Name + ' ' + joinedLobby.Players.Count);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error joining lobby: " + e.Message);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    [Command]
    public async void JoinLobbyById(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            TestMultiplayer.Instance.StartClient();

            Debug.Log("Joined lobby: " + joinedLobby.Name + ' ' + joinedLobby.Players.Count);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error joining lobby: " + e.Message);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    [Command]
    public async void QuickJoinLobby()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
            TestMultiplayer.Instance.StartClient();

            Debug.Log("Joined lobby: " + joinedLobby.Name + ' ' + joinedLobby.Players.Count);
            PrintPlayers(joinedLobby);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error joining lobby: " + e.Message);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Lobby Gamemode: " + lobby.Data["GameMode"].Value);
        Debug.Log("Players in lobby: " + lobby.Players.Count);
        foreach (Player player in lobby.Players)
        {
            Debug.Log("Player: " + player.Id + ' ' + player.Data["PlayerName"].Value);
        }
    }

    [Command]
    private void PrintPlayers()
    {
        if (joinedLobby != null)
        {
            PrintPlayers(joinedLobby);
        }
        else
        {
            Debug.Log("No lobby joined");
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
            }},
        };
    }

    private UpdatePlayerOptions GetUpdatePlayerOptions(string newPlayerName)
    {
        return new UpdatePlayerOptions
        {
            Data = new Dictionary<string, PlayerDataObject> {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newPlayerName)
            }},
        };
    }

    [Command]
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
            }
            });

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error updating lobby: " + e.Message);
        }
    }

    [Command]
    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {
            playerName = newPlayerName;
            UpdatePlayerOptions player = GetUpdatePlayerOptions(newPlayerName);

            joinedLobby = await Lobbies.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, player);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error updating player: " + e.Message);
        }
    }

    [Command]
    public async void LeaveLobby()
    {
        try
        {
            await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error leaving lobby: " + e.Message);
        }
    }

    [Command]
    private async void KickPlayer(string playerId)
    {
        try
        {
            await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error kicking player: " + e.Message);
        }
    }

    [Command]
    private async void MigrateLobbyHost()
    {
        try
        {
            joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error migrating host: " + e.Message);
        }
    }

    [Command]
    private async void DeleteLobby()
    {
        try
        {
            await Lobbies.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error deleting lobby: " + e.Message);
        }
    }
}
