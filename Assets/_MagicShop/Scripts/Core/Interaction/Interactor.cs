using UnityEngine;
using UnityEngine.InputSystem;
using MagicShop.Core;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Interactor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform interactionPosition;
    [SerializeField] private int pickableLimit = 5;

    private InputSystem_Actions inputActions;
    private IInteractable currentInteractable;
    private InteractionAction currentAction;
    private bool isInteracting;
    private List<Pickable> currentPicked;
    private GameObject currentBlueprint;
    public Transform InteractionPosition => interactionPosition;

    #region lifecycle
    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        currentPicked = new List<Pickable>();
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
        DisplayPickableBlueprint();
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
            Debug.Log("Executing interaction with: " + currentInteractable.GetPrompt());
            ExecuteInteraction();
        }
        else if (currentBlueprint != null)
        {
            Debug.Log("Placing item: " + currentPicked[currentPicked.Count - 1].name);
            PlaceItem();
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
        HandleSecondaryAction(context);
    }

    private void HandleSecondaryAction(InputAction.CallbackContext context)
    {
        if (currentPicked != null && currentPicked.Count > 0)
        {
            DropLastPickable();
            return;
        }
    }

    #endregion

    #region Pickable Interaction

    private void PlaceItem()
    {
        var lastPickable = currentPicked[currentPicked.Count - 1];
        lastPickable.PlaceItem(currentBlueprint.transform.position);
        currentPicked.Remove(lastPickable);
        Destroy(currentBlueprint);
        currentBlueprint = null;
    }

    public List<Pickable> GetPickable()
    {
        return currentPicked;
    }

    public bool SetPickable(Pickable pickable)
    {
        // TODO: notificar usuario caso currentPicked nao seja nulo
        if (currentPicked.Count < pickableLimit)
        {
            currentPicked.Add(pickable);
            return true;
        }
        return false;
    }

    public void ClearPickable(int index)
    {
        if (index >= 0 && index < currentPicked.Count)
        {
            currentPicked.RemoveAt(index);
        }
    }

    private void DropAllPickables()
    {
        for (int i = 0; i < currentPicked.Count; i++)
        {
            currentPicked[i].Drop();
        }
        currentPicked.Clear();
    }

    private void DropLastPickable()
    {
        if (currentPicked.Count > 0)
        {
            int lastIndex = currentPicked.Count - 1;
            currentPicked[lastIndex].Drop();
            currentPicked.RemoveAt(lastIndex);
            Destroy(currentBlueprint);
            currentBlueprint = null;
        }
    }
    
    private void DisplayPickableBlueprint()
    {
        if (currentPicked.Count > 0)
        {
            var lastPickable = currentPicked[currentPicked.Count - 1];

            // raycast para detectar aonde o cursor do mouse aponta no mundo
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 cursorPosition;
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                cursorPosition = hit.point;
            }
            else
            {
                Destroy(currentBlueprint);
                currentBlueprint = null;
                return;
            }

            if (currentBlueprint == null)
            {
                // Cria o blueprint do item no mundo
                currentBlueprint = Instantiate(lastPickable.BlueprintPrefab, cursorPosition, Quaternion.identity);
            }
            else
            {
                // Atualiza a posição do blueprint para seguir o cursor
                currentBlueprint.transform.position = cursorPosition;
            }

        }
        else
        {
            if (currentBlueprint != null)
            {
                currentBlueprint = null;
                Destroy(currentBlueprint);
            }
        }
    }

    #endregion
}
