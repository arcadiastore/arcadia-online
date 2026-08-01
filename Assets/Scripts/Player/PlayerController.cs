using UnityEngine;
using ArcadiaOnline.InputHandling;
using ArcadiaOnline.Combat;

namespace ArcadiaOnline.Player
{
    /// <summary>
    /// Movement & aksi dasar pemain. Lihat docs/01_GDD/04_Player.md,
    /// docs/01_GDD/05_Combat.md, dan docs/02_TDD/Input.md.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 7f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private Transform _cameraTransform;

        [Header("Combat")]
        [SerializeField] private TargetLockSystem _targetLock;
        [SerializeField] private SkillSystem _skillSystem;

        private CharacterController _characterController;
        private PlayerStats _stats;
        private Vector3 _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _stats = GetComponent<PlayerStats>();

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (InputManager.Instance == null) return;

            HandleMovement();
            HandleCombatInput();
        }

        private void HandleMovement()
        {
            Vector2 moveInput = InputManager.Instance.MoveInput;
            bool sprinting = InputManager.Instance.IsSprinting && _stats.TrySpendStamina(10f * Time.deltaTime);

            Vector3 forward = _cameraTransform != null ? _cameraTransform.forward : Vector3.forward;
            Vector3 right = _cameraTransform != null ? _cameraTransform.right : Vector3.right;
            forward.y = 0f; right.y = 0f;
            forward.Normalize(); right.Normalize();

            Vector3 moveDir = (forward * moveInput.y + right * moveInput.x);
            float speed = sprinting ? _sprintSpeed : _walkSpeed;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            // Gravity
            if (_characterController.isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }
            _velocity.y += _gravity * Time.deltaTime;

            Vector3 finalMove = moveDir * speed + Vector3.up * _velocity.y;
            _characterController.Move(finalMove * Time.deltaTime);
        }

        private void HandleCombatInput()
        {
            var input = InputManager.Instance;

            if (input.TargetLockPressed)
            {
                _targetLock?.FindNearestTarget();
            }

            if (input.AttackPressed && _targetLock != null && _targetLock.HasTarget)
            {
                // Ambil IDamageable dari target, lalu panggil
                // CombatManager.Instance.ExecuteAttack(...) sesuai stat ATK/DEF.
                var targetDamageable = _targetLock.CurrentTarget.GetComponent<IDamageable>();
                if (targetDamageable != null)
                {
                    CombatManager.Instance?.ExecuteAttack(
                        _stats, targetDamageable, _stats.BaseStats.atk, 0f);
                }
            }

            if (input.SkillPressedIndex >= 0 && _skillSystem != null)
            {
                _skillSystem.UseSkill(input.SkillPressedIndex);
            }

            if (input.DodgePressed)
            {
                CombatManager.Instance?.TryExecuteDodge();
            }

            if (input.FleePressed)
            {
                CombatManager.Instance?.ExecuteFlee();
            }
        }
    }
}
