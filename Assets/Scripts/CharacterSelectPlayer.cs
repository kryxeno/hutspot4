using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    private void Start()
    {
        TestMultiplayer.Instance.OnPlayerDataNetworkListChanged += TestMultiplayer_OnPlayerDataNetworkListChanged;

        UpdatePlayer();
    }

    private void TestMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (TestMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
