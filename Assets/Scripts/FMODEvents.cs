using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents Instance { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference UiClicked { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one FMODEvents in the scene");
        }
        Instance = this;
    }
}
