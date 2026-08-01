using UnityEngine;
using ArcadiaOnline.Managers;

namespace ArcadiaOnline.Player
{
    /// <summary>
    /// PlayerController sederhana pakai Input Manager (lama).
    /// Untuk test Fase 1 - nanti di-upgrade ke Input System.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class SimplePlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 9f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrain = 10f;
        [SerializeField] private float staminaRegen = 5f;
        [SerializeField] private float staminaCooldown = 2f;

        [Header("Gravity")]
        [SerializeField] private float gravity = -20f;

        [Header("Player Info")]
        [SerializeField] private string gender = "male"; // male atau female

        [Header("Sound")]
        [SerializeField] private float footstepInterval = 0.4f; // Interval langkah kaki
        private float footstepTimer = 0f;

        // Components
        private CharacterController controller;

        // State
        private Vector3 velocity;
        private bool isRunning;
        private float currentStamina;
        private float staminaCooldownTimer;

        // Properties
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public bool IsRunning => isRunning;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            currentStamina = maxStamina;
        }

        void Update()
        {
            HandleMovement();
            HandleStamina();
            ApplyGravity();
        }

        private void HandleMovement()
        {
            // Input menggunakan Input Manager (lama)
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
            float vertical = Input.GetAxisRaw("Vertical");     // W/S
            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            // Cek sprint
            bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
            isRunning = wantsToRun && currentStamina > 0 && inputDir.magnitude > 0.1f;

            float speed = isRunning ? runSpeed : walkSpeed;

            if (inputDir.magnitude >= 0.1f)
            {
                // Play footstep sound
                HandleFootstepSound(isRunning);

                // Gerak RELATIF terhadap arah kamera
                Camera cam = Camera.main;
                if (cam != null)
                {
                    Vector3 camForward = cam.transform.forward;
                    Vector3 camRight = cam.transform.right;
                    camForward.y = 0f;
                    camRight.y = 0f;
                    camForward.Normalize();
                    camRight.Normalize();

                    // Hitung arah gerak berdasarkan kamera
                    Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

                    // Rotasi ke arah gerakan
                    float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                    float smoothedAngle = Mathf.LerpAngle(
                        transform.eulerAngles.y,
                        targetAngle,
                        rotationSpeed * Time.deltaTime
                    );
                    transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                    // Gerakkan karakter
                    controller.Move(moveDir * speed * Time.deltaTime);
                }
                else
                {
                    // Fallback: gerak world space
                    Vector3 move = transform.forward * speed;
                    controller.Move(move * Time.deltaTime);
                }
            }
        }

        private void HandleStamina()
        {
            if (isRunning)
            {
                currentStamina -= staminaDrain * Time.deltaTime;
                currentStamina = Mathf.Max(0, currentStamina);
                staminaCooldownTimer = staminaCooldown;

                if (currentStamina <= 0)
                {
                    isRunning = false;
                    Debug.Log("[Player] Stamina habis!");
                }
            }
            else
            {
                if (staminaCooldownTimer > 0)
                {
                    staminaCooldownTimer -= Time.deltaTime;
                }
                else if (currentStamina < maxStamina)
                {
                    currentStamina += staminaRegen * Time.deltaTime;
                    currentStamina = Mathf.Min(maxStamina, currentStamina);
                }
            }
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleFootstepSound(bool sprinting)
        {
            if (JobSFXManager.Instance == null) return;

            // Interval lebih cepat saat sprint
            float interval = sprinting ? footstepInterval * 0.6f : footstepInterval;

            footstepTimer += Time.deltaTime;
            if (footstepTimer >= interval)
            {
                footstepTimer = 0f;
                JobSFXManager.Instance.PlayRun(gender);
            }
        }
    }
}
