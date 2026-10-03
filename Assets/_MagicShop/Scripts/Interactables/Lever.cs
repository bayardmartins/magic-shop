using MagicShop.Core;
using Unity.VisualScripting;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private string actionName = "Generic Action";
    [SerializeField] private string unavailableHint = "This action is not available.";
    [SerializeField] private string endMessage = "Generic action ended.";
    [SerializeField] private string startMessage = "Generic action started.";
    [SerializeField] private Sprite icon;
    [SerializeField] private Transform restPositionReference;
    [SerializeField] private Transform activePositionReference;

    InteractionAction action;
    bool isInteracting = false;

    void Awake()
    {
        action = new InteractionAction(
            actionName,
            icon,
            interactor => true, // Always available
            interactor => Interact(),
            unavailableHint
        );
    }

    void Start()
    {
        if (restPositionReference == null)
        {
            Debug.LogWarning("[Lever] Rest position reference is not assigned.");
            restPositionReference = transform; // Default to the current position if not assigned
            //restPositionReference.position = 
        }
    }

    public InteractionAction GetAvailableAction(Interactor interactor)
    {
        return action;
    }

    public string GetPrompt()
    {
        return actionName;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public void OnInteractionEnded(Interactor interactor)
    {
        isInteracting = false;
        Debug.Log(endMessage);
    }

    public void OnInteractionStarted(Interactor interactor)
    {
        isInteracting = true;
        Debug.Log(startMessage);
    }

    private void Interact()
    {
        if (!isInteracting)
        {
            Debug.LogWarning("No interaction in progress.");
            return;
        }


    }
}
