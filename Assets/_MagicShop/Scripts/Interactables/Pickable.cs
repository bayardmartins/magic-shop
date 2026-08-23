using MagicShop.Core;
using UnityEngine;

public class Pickable : MonoBehaviour, IInteractable
{
    [SerializeField] private string actionName = "Generic Action";
    [SerializeField] private string unavailableHint = "This action is not available.";
    [SerializeField] private string endMessage = "Generic action ended.";
    [SerializeField] private string startMessage = "Generic action started.";
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject blueprintPrefab;

    InteractionAction action;
    Interactor currentInteractor;
    bool isInteracting = false;
    Rigidbody rb;

    public GameObject BlueprintPrefab => blueprintPrefab;

    void Awake()
    {
        action = new InteractionAction(
            actionName,
            icon,
            interactor => true, // Always available
            interactor => Interact(),
            unavailableHint
        );
        rb = GetComponent<Rigidbody>();
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
        if (!isInteracting || currentInteractor == null)
        {
            Debug.LogWarning("No interaction in progress.");
            return;
        }

        bool success = currentInteractor.SetPickable(this);
        if (!success)
        {
            Debug.LogWarning("Failed to pick up the item.");
            return;
        }

        transform.position = currentInteractor.InteractionPosition.position;
        transform.SetParent(currentInteractor.transform);
        rb.isKinematic = true;
        Debug.Log("Generic interaction executed.");
    }

    public void Drop()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
    }

    public void PlaceItem(Vector3 position)
    {
        transform.position = position;
        transform.SetParent(null);
        rb.isKinematic = false;
    }
}
