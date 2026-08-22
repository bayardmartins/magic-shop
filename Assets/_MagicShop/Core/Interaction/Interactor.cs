using UnityEngine;
using UnityEngine.InputSystem;
using MagicShop.Core;

public class Interactor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform interactionPosition;

    private InputSystem_Actions inputActions;
    private IInteractable currentInteractable;
    private InteractionAction currentAction;
    private bool isInteracting;
    private Pickable currentPicked;

    InteractionUI ActiveUi => currentInteractable?.UI;
    public Transform InteractionPosition => interactionPosition;

    #region lifecycle
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
        inputActions.Player.PrimaryAction.performed += OnPrimaryActionPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.PrimaryAction.performed -= OnPrimaryActionPerformed;
        inputActions.Player.Disable();
        ClearCurrentInteractable();
    }

    private void Update()
    {
        DetectTarget();
    }

    #endregion

    #region Interaction
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

    private void HandleAction(InputAction.CallbackContext context)
    {
        if (currentInteractable != null && currentAction != null && currentAction.CanExecute(this))
        {
            ExecuteInteraction();
        }
    }

    private void OnPrimaryActionPerformed(InputAction.CallbackContext context)
    {
        HandleAction(context);
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
    #endregion

    #region Pickable Interaction

    public Pickable GetPickable()
    {
        return currentPicked;
    }

    public bool SetPickable(Pickable pickable)
    {
        // TODO: notificar usuario caso currentPicked nao seja nulo
        if (currentPicked == null)
        {
            currentPicked = pickable;
            return true;
        }
        return false;
    }

    public void ClearPickable()
    {
        currentPicked = null;
    }

    #endregion
}
