using MagicShop.Core;
using UnityEngine;

public class GenericInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private InteractionUI ui;
    [SerializeField] private string actionName = "Generic Action";
    [SerializeField] private string unavailableHint = "This action is not available.";
    [SerializeField] private string endMessage = "Generic action ended.";
    [SerializeField] private string startMessage = "Generic action started.";
    InteractionAction action;

    public InteractionUI UI => ui;

    void Awake()
    {
        action = new InteractionAction(
            actionName,
            interactor => true, // Always available
            interactor => Debug.Log($"{actionName} executed!"),
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
        Debug.Log(endMessage);

    }

    public void OnInteractionStarted(Interactor interactor)
    {
        Debug.Log(startMessage);
    }
}
