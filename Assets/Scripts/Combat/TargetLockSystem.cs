using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArcadiaOnline.Combat
{
    /// <summary>
    /// Target Lock. Lihat docs/01_GDD/05_Combat.md dan
    /// docs/02_TDD/CombatArchitecture.md.
    /// </summary>
    public class TargetLockSystem : MonoBehaviour
    {
        [SerializeField] private float _lockRange = 15f;
        [SerializeField] private float _switchCooldown = 0.5f;
        [SerializeField] private LayerMask _enemyLayer;

        private List<Transform> _availableTargets = new List<Transform>();
        private int _currentTargetIndex;
        private float _switchTimer;

        public Transform CurrentTarget { get; private set; }
        public bool HasTarget => CurrentTarget != null;

        private void Update()
        {
            if (_switchTimer > 0f) _switchTimer -= Time.deltaTime;

            if (CurrentTarget != null && !IsTargetInRange())
            {
                ClearTarget();
            }
        }

        public Transform FindNearestTarget()
        {
            RefreshAvailableTargets();
            if (_availableTargets.Count == 0) return null;

            Transform nearest = _availableTargets
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .First();

            CurrentTarget = nearest;
            _currentTargetIndex = _availableTargets.IndexOf(nearest);
            return CurrentTarget;
        }

        public void SwitchToNextTarget()
        {
            if (_switchTimer > 0f || _availableTargets.Count <= 1) return;

            RefreshAvailableTargets();
            if (_availableTargets.Count == 0) return;

            _currentTargetIndex = (_currentTargetIndex + 1) % _availableTargets.Count;
            CurrentTarget = _availableTargets[_currentTargetIndex];
            _switchTimer = _switchCooldown;
        }

        public void SwitchToPreviousTarget()
        {
            if (_switchTimer > 0f || _availableTargets.Count <= 1) return;

            RefreshAvailableTargets();
            if (_availableTargets.Count == 0) return;

            _currentTargetIndex--;
            if (_currentTargetIndex < 0) _currentTargetIndex = _availableTargets.Count - 1;
            CurrentTarget = _availableTargets[_currentTargetIndex];
            _switchTimer = _switchCooldown;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
        }

        public bool IsTargetInRange()
        {
            if (CurrentTarget == null) return false;
            return Vector3.Distance(transform.position, CurrentTarget.position) <= _lockRange;
        }

        private void RefreshAvailableTargets()
        {
            _availableTargets.Clear();
            Collider[] hits = Physics.OverlapSphere(transform.position, _lockRange, _enemyLayer);
            foreach (var hit in hits)
            {
                _availableTargets.Add(hit.transform);
            }
        }
    }
}
