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

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private float lobbyListUpdateTimer;
    private string playerName;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await UnityServices.InitializeAsync();
        playerName = "Megatimtim" + UnityEngine.Random.Range(10, 100);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId + " " + playerName);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void Update()
    {
        HandleLobbyHeatbeat();
        HandleLobbyPollForUpdates();
        HandlePeriodicLobbyListUpdate();
    }

    private async void HandleLobbyHeatbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
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
    public async void CreateLobby()
    {
        try
        {

            string lobbyName = "TestLobby";
            int maxPlayers = 4;

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { "LobbyName", new DataObject(DataObject.VisibilityOptions.Public, lobbyName) },
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Story") },
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = lobby;


            Debug.Log("Lobby created: " + lobby.Name + ' ' + lobby.MaxPlayers + ' ' + lobby.Id + ' ' + lobby.LobbyCode);
            PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error creating lobby: " + e.Message);
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

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log("Lobby: " + lobby.Name + ' ' + lobby.MaxPlayers + ' ' + lobby.Data["GameMode"].Value);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error creating lobby: " + e.Message);
        }
    }

    [Command]
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            Debug.Log("Joined lobby: " + joinedLobby.Name + ' ' + joinedLobby.Players.Count);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error joining lobby: " + e.Message);
        }
    }

    [Command]
    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

            Debug.Log("Joined lobby: " + joinedLobby.Name + ' ' + joinedLobby.Players.Count);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error joining lobby: " + e.Message);
        }
    }

    [Command]
    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await Lobbies.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);

            Debug.Log("Joined lobby: " + joinedLobby.Name + ' ' + joinedLobby.Players.Count);
            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error joining lobby: " + e.Message);
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
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) }
            }
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
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

            hostLobby = await Lobbies.Instance.UpdatePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId, player);
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error updating player: " + e.Message);
        }
    }

    [Command]
    private async void LeaveLobby()
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
            await Lobbies.Instance.RemovePlayerAsync(hostLobby.Id, playerId);
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
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = hostLobby.Players[1].Id
            });
            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
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
            hostLobby = null;
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Error deleting lobby: " + e.Message);
        }
    }
}
