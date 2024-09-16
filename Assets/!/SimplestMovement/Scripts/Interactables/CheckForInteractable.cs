using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class CheckForInteractable : MonoBehaviour
{
    [SerializeField] private float interactShowDistance;
    [SerializeField] private Transform rayCastStartPosition;
    [SerializeField] private GameObject InteractShower;

    [SerializeField] LayerMask interactLayers;

    private Interactable interactable;


    //Make sure we can use interactables here
    private Hutspot4Controls playerControls;

    private void Awake()
    {
        playerControls = new Hutspot4Controls();

        playerControls.Player.Interact.performed += PerformInteractFunctions;
    }

    private void OnEnable()
    {
        playerControls.Player.Interact.performed += PerformInteractFunctions;
        playerControls.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Interact.performed -= PerformInteractFunctions;
        playerControls.Player.Interact.Disable();
    }

    private void Start()
    {
        InteractShower = GameObject.Find("Interact Shower");
        InteractShower.SetActive(false);
    }


    private void Update()
    {
        RaycastHit interactableHit;

        //Check if we are looking at something
        if (Physics.Raycast(rayCastStartPosition.position, rayCastStartPosition.forward, out interactableHit, interactShowDistance, interactLayers))
        {
            //Then we check if it is an interactable
            interactable = interactableHit.transform.gameObject.GetComponent<Interactable>();
            if (interactable != null && !InteractShower.activeInHierarchy)
            {
                //If so, show the text, and make sure the function can be activated
                InteractShower.GetComponent<TextMeshProUGUI>().text = "[E] " + interactable.interactMessage;
                InteractShower.SetActive(true);
            }            
        }
        else
        {
            //Make sure we no longer see text, and that we have no more interactable
            if (InteractShower.activeInHierarchy) InteractShower.SetActive(false);
            interactable = null;
        }
    }

    private void PerformInteractFunctions(InputAction.CallbackContext obj)
    {
        //If the interactable is not null, trigger it.
        interactable?.TriggerInteractable();
    }
}
