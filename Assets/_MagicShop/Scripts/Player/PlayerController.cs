using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace MagicShop.Player
{
    /// <summary>
    /// Controlador principal do player. Gerencia movimento e camera em primeira pessoa.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimento")]
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float groundDrag = 5f;

        [SerializeField]
        private float gravity = -9.81f;

        [Header("Camera")]
        [SerializeField]
        private CinemachineCamera cinemachineCamera;

        [SerializeField]
        private float mouseSensitivity = 0.03f;

        [SerializeField]
        private float maxLookAngle = 90f;

        private CharacterController characterController;
        private InputSystem_Actions inputActions;

        private Vector3 currentVelocity;
        private float currentPitch;

        private void Awake()
        {
            TryGetComponent(out characterController);
            if (characterController == null)
            {
                Debug.LogError("PlayerController requer um CharacterController no GameObject", gameObject);
                enabled = false;
                return;
            }

            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions?.Player.Enable();
        }

        private void OnDisable()
        {
            inputActions?.Player.Disable();
        }

        private void Start()
        {
            if (cinemachineCamera == null)
            {
                cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
                if (cinemachineCamera == null)
                {
                    Debug.LogWarning("Nenhuma CinemachineCamera encontrada na cena");
                }
            }
            CursorManager.Instance.LockCursor();
        }

        private void Update()
        {
            HandleMovement();
            HandleCamera();
        }

        /// <summary>
        /// Processa movimento do player baseado em input.
        /// </summary>
        private void HandleMovement()
        {
            Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

            // Converter input para direcao relativa ao player
            Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;

            // Aplicar velocidade de movimento
            Vector3 moveVelocity = moveDirection * moveSpeed;

            // Aplicar gravidade
            currentVelocity.y += gravity * Time.deltaTime;

            // Aplicar drag quando no chao
            if (characterController.isGrounded)
            {
                currentVelocity.y = 0;
                moveVelocity *= (1 - groundDrag * Time.deltaTime);
            }

            // Aplicar velocidade horizontal
            moveVelocity.y = currentVelocity.y;

            characterController.Move(moveVelocity * Time.deltaTime);
        }

        /// <summary>
        /// Processa rotacao da camera baseada em input Look.
        /// </summary>
        private void HandleCamera()
        {
            Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

            // Rotacao horizontal (yaw) - rotacionar o player
            float yaw = lookInput.x * mouseSensitivity;
            transform.Rotate(0, yaw, 0);

            // Rotacao vertical (pitch) - rotacionar APENAS a camera
            float pitch = lookInput.y * mouseSensitivity;
            currentPitch -= pitch;
            currentPitch = Mathf.Clamp(currentPitch, -maxLookAngle, maxLookAngle);

            if (cinemachineCamera != null)
            {
                // Aplicar APENAS pitch (yaw já é herdado do parent)
                cinemachineCamera.transform.localRotation = Quaternion.Euler(currentPitch, 0, 0);
            }
        }

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }
    }
}