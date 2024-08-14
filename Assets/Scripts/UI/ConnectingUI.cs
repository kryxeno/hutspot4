using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    private void Start()
    {
        TestMultiplayer.Instance.OnTryingToJoinGame += TestMultiplayer_OnTryingToJoinGame;
        TestMultiplayer.Instance.OnFailedToJoinGame += TestMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void TestMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void TestMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
        TestMultiplayer.Instance.OnTryingToJoinGame -= TestMultiplayer_OnTryingToJoinGame;
        TestMultiplayer.Instance.OnFailedToJoinGame -= TestMultiplayer_OnFailedToJoinGame;
    }
}
