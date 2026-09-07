using UnityEngine;
using UnityEngine.InputSystem;
using MagicShop.Core;
using System;
using System.Collections.Generic;

public class Interactor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform interactionPosition;
    [SerializeField] private int pickableLimit = 5;

    [Header("Snap Configuration")]
    [SerializeField] private float snapMagneticForce = 1f;
    [SerializeField] private bool enableSnapping = true;

    private InputSystem_Actions inputActions;
    private IInteractable currentInteractable;
    private InteractionAction currentAction;
    private bool isInteracting;
    private List<Pickable> currentPicked;
    private Blueprint currentBlueprint;
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
        inputActions.Player.Interact.performed += OnInteractActionPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.PrimaryAction.performed -= OnPrimaryActionPerformed;
        inputActions.Player.SecondaryAction.performed -= OnSecondaryActionPerformed;
        inputActions.Player.Interact.performed -= OnInteractActionPerformed;
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
        HandleSecondaryAction(context);
    }

    private void HandleSecondaryAction(InputAction.CallbackContext context)
    {
        if (currentBlueprint != null)
        {
            PlaceItem();
        }
    }

    #endregion

    #region InteractAction
    private void OnInteractActionPerformed(InputAction.CallbackContext context)
    {
        HandleInteractionAction(context);
    }

    private void HandleInteractionAction(InputAction.CallbackContext context)
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
        lastPickable.PlaceItem(currentBlueprint.transform.position, currentBlueprint.transform.rotation);
        currentPicked.Remove(lastPickable);
        Destroy(currentBlueprint.gameObject);
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
            if (currentBlueprint != null)
                Destroy(currentBlueprint.gameObject);
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
                if (currentBlueprint != null)
                {
                    if (currentBlueprint != null)
                        Destroy(currentBlueprint.gameObject);
                    currentBlueprint = null;
                }
                return;
            }

            if (currentBlueprint == null)
            {
                // Cria o blueprint do item no mundo
                if (lastPickable.BlueprintPrefab == null)
                {
                    Debug.LogWarning($"Pickable {lastPickable.name} does not have a BlueprintPrefab assigned.");
                    return;
                }
                else
                {
                    currentBlueprint = Instantiate(lastPickable.BlueprintPrefab, cursorPosition, Quaternion.identity);
                }
            }

            // Aplica snap magnetico com alinhamento de snap points
            Vector3 finalPosition = cursorPosition;
            if (enableSnapping)
            {
                if (currentBlueprint != null)
                {
                    // Encontra o melhor par de snap points considerando o lado (left/right)
                    SnapZone.SnapPointPair bestPair = SnapZone.Instance.FindBestSnapPointPairForBlueprint(currentBlueprint, cursorPosition);

                    if (bestPair.blueprintSnapTransform != null && bestPair.environmentSnap != null)
                    {
                        // Calcula a posicao onde o snap point da blueprint se alinhara com o snap point do ambiente
                        Vector3 alignedPosition = SnapZone.Instance.CalculateAlignedPosition(
                            currentBlueprint.transform.position, 
                            bestPair.blueprintSnapTransform, 
                            bestPair.environmentSnap
                        );

                        // Interpola entre a posicao do cursor e a posicao alinhada
                        // snapMagneticForce controla quao forte eh o "ima" (0 = sem snap, 1 = snap completo)
                        finalPosition = Vector3.Lerp(cursorPosition, alignedPosition, snapMagneticForce);
                    }
                }
            }

            // Atualiza a posição do blueprint para seguir o cursor (com snap)
            currentBlueprint.transform.position = finalPosition;
        }
        else
        {
            if (currentBlueprint != null)
            {
                if (currentBlueprint != null)
                    Destroy(currentBlueprint.gameObject);
                currentBlueprint = null;
            }
        }
    }

    #endregion
}
