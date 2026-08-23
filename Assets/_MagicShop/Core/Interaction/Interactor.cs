using UnityEngine;
using UnityEngine.InputSystem;
using MagicShop.Core;
using System;

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
        inputActions.Player.SecondaryAction.performed += OnSecondaryActionPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.PrimaryAction.performed -= OnPrimaryActionPerformed;
        inputActions.Player.SecondaryAction.performed -= OnSecondaryActionPerformed;
        inputActions.Player.Disable();
        ClearCurrentInteractable();
    }

    private void Update()
    {
        DetectTarget();
    }

    #endregion

    #region PrimaryAction
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
                    Sprite icon = currentInteractable.GetIcon();
                    InteractionUI.Instance.ShowPrompt(prompt, icon);
                }
            }
        }
    }

    private void OnPrimaryActionPerformed(InputAction.CallbackContext context)
    {
        HandlePrimaryAction(context);
    }

    private void HandlePrimaryAction(InputAction.CallbackContext context)
    {
        if (currentInteractable != null && currentAction != null && currentAction.CanExecute(this))
        {
            ExecuteInteraction();
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
            InteractionUI.Instance.HideAll();
        }

        currentInteractable = null;
        currentAction = null;
    }
    #endregion

    #region SecondaryAction
    private void OnSecondaryActionPerformed(InputAction.CallbackContext context)
    {
        HandleSecundaryAction(context);
    }

    private void HandleSecundaryAction(InputAction.CallbackContext context)
    {
        if (currentPicked != null)
        {
            DropPickable();
            return;
        }
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

    private void DropPickable()
    {
        currentPicked.Drop();
        ClearPickable();
    }

    #endregion
}
