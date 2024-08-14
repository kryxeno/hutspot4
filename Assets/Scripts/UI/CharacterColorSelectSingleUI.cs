using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            TestMultiplayer.Instance.ChangePlayerColor(colorId);
            UpdateIsSelected();
        });
    }

    private void Start()
    {
        TestMultiplayer.Instance.OnPlayerDataNetworkListChanged += TestMultiplayer_OnPlayerDataNetworkListChanged;
        image.color = TestMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void TestMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (TestMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        TestMultiplayer.Instance.OnPlayerDataNetworkListChanged -= TestMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
