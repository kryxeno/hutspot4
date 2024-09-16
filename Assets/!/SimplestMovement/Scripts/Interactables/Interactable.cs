using UnityEngine;
using UnityEngine.Events;


public class Interactable : MonoBehaviour
{
    public string interactMessage;

    [SerializeField] private UnityEvent _onInteractableTriggered;

    public void TriggerInteractable()
    {
        _onInteractableTriggered?.Invoke();
    }
}
