using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        leaveButton.onClick.AddListener(OnLeaveButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);
    }

    private void Start()
    {
        Lobby lobby = TestLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }

    private void OnLeaveButtonClicked()
    {
        TestLobby.Instance.LeaveLobby();
        TestMultiplayer.Instance.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void OnReadyButtonClicked()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
    }
}
