using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Toggle privateToggle;
    [SerializeField] private Button createLobbyButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
    }

    private void Start()
    {
        Hide();
    }

    private void OnCloseButtonClicked()
    {
        Hide();
    }

    private void OnCreateLobbyButtonClicked()
    {
        TestLobby.Instance.CreateLobby(lobbyNameInputField.text, privateToggle.isOn);
    }

    private void UpdateName()
    {
        string playerName = TestMultiplayer.Instance.GetPlayerName();
        lobbyNameInputField.text = playerName + "'s Lobby";
    }

    public void Show()
    {
        UpdateName();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
