using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinByCodeButton;
    [SerializeField] private TMP_InputField codeInputField;

    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);
        joinByCodeButton.onClick.AddListener(OnJoinByCodeButtonClicked);

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        TestLobby.Instance.OnLobbyListChanged += TestLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void TestLobby_OnLobbyListChanged(object sender, TestLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void OnCreateLobbyButtonClicked()
    {
        Debug.Log("Create Lobby Button Clicked");
        // TestLobby.Instance.CreateLobby();
        TestMultiplayer.Instance.StartHost();
        Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    }

    private void OnQuickJoinButtonClicked()
    {
        Debug.Log("Quick Join Button Clicked");
        TestMultiplayer.Instance.StartClient();
        // TestLobby.Instance.QuickJoinLobby();
    }

    private void OnJoinByCodeButtonClicked()
    {
        Debug.Log("Join By Code Button Clicked");
        TestLobby.Instance.JoinLobbyByCode(codeInputField.text);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);

            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
