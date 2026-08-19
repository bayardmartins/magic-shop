using UnityEngine;
using MagicShop.Core;

public class Interactor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField]
    private float interactionDistance = 5f;

    [SerializeField]
    private Camera mainCamera;

    private InputSystem_Actions inputActions;
    private IInteractable currentInteractable;
    private InteractionAction currentAction;
    private bool isInteracting;
    InteractionUI ActiveUi => currentInteractable?.UI;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        ClearCurrentInteractable();
    }

    private void Update()
    {
        DetectTarget();
        HandleAction();
    }

    private void DetectTarget()
    {
        IInteractable detectedInteractable = null;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            detectedInteractable = hit.collider.GetComponent<IInteractable>();
        }

        if (detectedInteractable != currentInteractable)
        {
            ClearCurrentInteractable();
            currentInteractable = detectedInteractable;

            if (currentInteractable != null)
            {
                currentAction = currentInteractable.GetAvailableAction(this);
                if (currentAction != null && currentAction.CanExecute(this))
                {
                    string prompt = currentInteractable.GetPrompt();
                    ActiveUi.ShowPrompt(prompt);
                }
            }
        }
    }

    private void HandleAction()
    {
        if (inputActions.Player.PrimaryAction.triggered)
        {
            if (currentInteractable != null && currentAction != null && currentAction.CanExecute(this))
            {
                ExecuteInteraction();
            }
        }
    }

    private void ExecuteInteraction()
    {
        if (isInteracting)
        {
            return;
        }

        isInteracting = true;
        currentInteractable.OnInteractionStarted(this);
        currentAction.Execute(this);
        currentInteractable.OnInteractionEnded(this);
        isInteracting = false;

        ClearCurrentInteractable();
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            ActiveUi.HideAll();
        }

        currentInteractable = null;
        currentAction = null;
    }
}
