using UnityEngine;
using UnityEngine.InputSystem;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.InputHandling
{
    /// <summary>
    /// Wrapper input global. Lihat docs/02_TDD/Input.md.
    ///
    /// PENTING: script ini butuh asset "ArcadiaControls.inputactions" yang ada
    /// di Assets/InputActions/. Generate C# class-nya dengan cara:
    /// klik asset tsb di Inspector > centang "Generate C# Class" > Apply.
    /// Setelah itu ganti tipe `PlayerInputActions` di bawah sesuai nama class
    /// yang di-generate (default: nama file asset).
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        public Vector2 MoveInput { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool DodgePressed { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool DefendHeld { get; private set; }
        public bool FleePressed { get; private set; }
        public bool TargetLockPressed { get; private set; }
        public int SkillPressedIndex { get; private set; } = -1; // -1 = tidak ada
        public bool MenuPressed { get; private set; }
        public bool InventoryPressed { get; private set; }
        public bool MapPressed { get; private set; }
        public bool QuestPressed { get; private set; }

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _jumpAction;
        private InputAction _dodgeAction;
        private InputAction _attackAction;
        private InputAction _defendAction;
        private InputAction _fleeAction;
        private InputAction _targetLockAction;
        private InputAction _menuAction;
        private InputAction _inventoryAction;
        private InputAction _mapAction;
        private InputAction _questAction;
        private InputAction[] _skillActions = new InputAction[8];

        protected override void Awake()
        {
            base.Awake();
            _playerInput = GetComponent<PlayerInput>();

            if (_playerInput == null)
            {
                Debug.LogError("InputManager butuh komponen PlayerInput dengan " +
                    "ArcadiaControls.inputactions di-assign. Lihat Assets/InputActions/.");
                return;
            }

            var map = _playerInput.actions.FindActionMap("Player");
            _moveAction = map.FindAction("Move");
            _sprintAction = map.FindAction("Sprint");
            _jumpAction = map.FindAction("Jump");
            _dodgeAction = map.FindAction("Dodge");
            _attackAction = map.FindAction("Attack");
            _defendAction = map.FindAction("Defend");
            _fleeAction = map.FindAction("Flee");
            _targetLockAction = map.FindAction("TargetLock");

            var uiMap = _playerInput.actions.FindActionMap("UI");
            _menuAction = uiMap.FindAction("Menu");
            _inventoryAction = uiMap.FindAction("Inventory");
            _mapAction = uiMap.FindAction("Map");
            _questAction = uiMap.FindAction("Quest");

            for (int i = 0; i < 8; i++)
            {
                _skillActions[i] = map.FindAction($"Skill{i + 1}");
            }
        }

        private void Update()
        {
            if (_moveAction == null) return;

            MoveInput = _moveAction.ReadValue<Vector2>();
            IsSprinting = _sprintAction.IsPressed();
            JumpPressed = _jumpAction.WasPressedThisFrame();
            DodgePressed = _dodgeAction.WasPressedThisFrame();
            AttackPressed = _attackAction.WasPressedThisFrame();
            DefendHeld = _defendAction.IsPressed();
            FleePressed = _fleeAction.WasPressedThisFrame();
            TargetLockPressed = _targetLockAction.WasPressedThisFrame();

            MenuPressed = _menuAction.WasPressedThisFrame();
            InventoryPressed = _inventoryAction.WasPressedThisFrame();
            MapPressed = _mapAction.WasPressedThisFrame();
            QuestPressed = _questAction.WasPressedThisFrame();

            SkillPressedIndex = -1;
            for (int i = 0; i < _skillActions.Length; i++)
            {
                if (_skillActions[i] != null && _skillActions[i].WasPressedThisFrame())
                {
                    SkillPressedIndex = i;
                    break;
                }
            }
        }
    }
}
