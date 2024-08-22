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
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    [SerializeField] private CreateLobbyUI createLobbyUI;

    private void Awake()
    {
        quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);
        createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        joinByCodeButton.onClick.AddListener(OnJoinByCodeButtonClicked);
        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = TestMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            TestMultiplayer.Instance.SetPlayerName(newText);
        });
        TestLobby.Instance.OnLobbyListChanged += TestLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void TestLobby_OnLobbyListChanged(object sender, TestLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void OnCreateLobbyButtonClicked()
    {
        createLobbyUI.Show();
        AudioManager.instance.PlayOneShot(FMODEvents.Instance.UiClicked, this.transform.position);
    }

    private void OnQuickJoinButtonClicked()
    {
        TestLobby.Instance.QuickJoinLobby();
        AudioManager.instance.PlayOneShot(FMODEvents.Instance.UiClicked, this.transform.position);
    }

    private void OnJoinByCodeButtonClicked()
    {
        TestLobby.Instance.JoinLobbyByCode(codeInputField.text);
        AudioManager.instance.PlayOneShot(FMODEvents.Instance.UiClicked, this.transform.position);
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

    private void OnDestroy()
    {
        TestLobby.Instance.OnLobbyListChanged -= TestLobby_OnLobbyListChanged;
    }
}
