using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        leaveButton.onClick.AddListener(OnLeaveButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);
    }

    private void OnLeaveButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.LobbyScene);
    }

    private void OnReadyButtonClicked()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
    }
}
