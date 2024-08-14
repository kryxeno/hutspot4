using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            TestLobby.Instance.JoinLobbyById(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
}
