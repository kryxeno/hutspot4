using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtraPlayerControls : MonoBehaviour
{
    private Hutspot4Controls playerInput;

    private void Awake()
    {
        playerInput = new Hutspot4Controls();
    }
}
