using MagicShop.Core;
using System;
using UnityEngine;

public class GenericInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string actionName = "Generic Action";
    [SerializeField] private string unavailableHint = "This action is not available.";
    [SerializeField] private string endMessage = "Generic action ended.";
    [SerializeField] private string startMessage = "Generic action started.";
    [SerializeField] private Sprite icon;
    InteractionAction action;
    Interactor currentInteractor;
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

    public InteractionAction GetAvailableAction(Interactor interactor)
    {
        return action;
    }

    public string GetPrompt()
    {
        return actionName;
    }

    public void OnInteractionEnded(Interactor interactor)
    {
        isInteracting = false;
        currentInteractor = null;
        Debug.Log(endMessage);        
    }

    public void OnInteractionStarted(Interactor interactor)
    {
        if (isInteracting)
        {
            Debug.LogWarning("Interaction already in progress.");
            return;
        }
        Debug.Log(startMessage);
        isInteracting = true;
        currentInteractor = interactor;
    }

    private void Interact()
    {
        Debug.Log("Generic interaction executed.");
    }

    public Sprite GetIcon()
    {
        throw new NotImplementedException();
    }
}
