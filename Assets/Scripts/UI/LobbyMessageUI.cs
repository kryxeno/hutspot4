using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        TestMultiplayer.Instance.OnFailedToJoinGame += TestMultiplayer_OnFailedToJoinGame;
        TestLobby.Instance.OnCreateLobbyStarted += TestLobby_OnCreateLobbyStarted;
        TestLobby.Instance.OnCreateLobbyFailed += TestLobby_OnCreateLobbyFailed;
        TestLobby.Instance.OnJoinStarted += TestLobby_OnJoinStarted;
        TestLobby.Instance.OnJoinFailed += TestLobby_OnJoinFailed;
        TestLobby.Instance.OnQuickJoinFailed += TestLobby_OnQuickJoinFailed;


        Hide();
    }

    private void TestLobby_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Could not find a lobby to join");
    }

    private void TestLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join lobby");
    }

    private void TestLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining lobby...");

    }

    private void TestLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to create lobby");
    }

    private void TestLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating lobby...");
    }

    private void TestMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        TestMultiplayer.Instance.OnFailedToJoinGame -= TestMultiplayer_OnFailedToJoinGame;
    }
}
