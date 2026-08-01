using UnityEngine;
using UnityEngine.AI;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Monster
{
    /// <summary>
    /// AI monster dasar: Patrol -> Chase -> Attack.
    /// Lihat docs/02_TDD/Architecture.md (State Machine untuk AI) dan
    /// docs/02_TDD/AIArchitecture.md.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class MonsterAI : MonoBehaviour
    {
        [SerializeField] private float _detectRange = 8f;
        [SerializeField] private float _attackRange = 2f;
        [SerializeField] private float _attackCooldown = 2f;
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private LayerMask _playerLayer;

        public NavMeshAgent Agent { get; private set; }
        public Transform PlayerTarget { get; private set; }
        public float AttackCooldown => _attackCooldown;
        public float AttackRange => _attackRange;

        private readonly StateMachine _stateMachine = new StateMachine();
        private int _patrolIndex;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _stateMachine.ChangeState(new PatrolState(this));
        }

        private void Update()
        {
            DetectPlayer();
            _stateMachine.Update();
        }

        private void DetectPlayer()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _detectRange, _playerLayer);
            PlayerTarget = hits.Length > 0 ? hits[0].transform : null;
        }

        public Transform GetNextPatrolPoint()
        {
            if (_patrolPoints == null || _patrolPoints.Length == 0) return null;
            _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
            return _patrolPoints[_patrolIndex];
        }

        public void ChangeState(IState state) => _stateMachine.ChangeState(state);
    }

    public class PatrolState : IState
    {
        private readonly MonsterAI _ai;
        public PatrolState(MonsterAI ai) => _ai = ai;

        public void Enter()
        {
            var point = _ai.GetNextPatrolPoint();
            if (point != null) _ai.Agent.SetDestination(point.position);
        }

        public void Update()
        {
            if (_ai.PlayerTarget != null)
            {
                _ai.ChangeState(new ChaseState(_ai));
                return;
            }

            if (!_ai.Agent.pathPending && _ai.Agent.remainingDistance < 0.5f)
            {
                Enter(); // ambil patrol point berikutnya
            }
        }

        public void Exit() { }
    }

    public class ChaseState : IState
    {
        private readonly MonsterAI _ai;
        public ChaseState(MonsterAI ai) => _ai = ai;

        public void Enter() { }

        public void Update()
        {
            if (_ai.PlayerTarget == null)
            {
                _ai.ChangeState(new PatrolState(_ai));
                return;
            }

            _ai.Agent.SetDestination(_ai.PlayerTarget.position);

            float distance = Vector3.Distance(_ai.transform.position, _ai.PlayerTarget.position);
            if (distance <= _ai.AttackRange)
            {
                _ai.ChangeState(new AttackState(_ai));
            }
        }

        public void Exit() { }
    }

    public class AttackState : IState
    {
        private readonly MonsterAI _ai;
        private float _cooldownTimer;

        public AttackState(MonsterAI ai) => _ai = ai;

        public void Enter()
        {
            _ai.Agent.isStopped = true;
            _cooldownTimer = 0f;
        }

        public void Update()
        {
            if (_ai.PlayerTarget == null)
            {
                _ai.ChangeState(new PatrolState(_ai));
                return;
            }

            float distance = Vector3.Distance(_ai.transform.position, _ai.PlayerTarget.position);
            if (distance > _ai.AttackRange)
            {
                _ai.ChangeState(new ChaseState(_ai));
                return;
            }

            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                // TODO: panggil DamageCalculator & terapkan damage ke player
                // via IDamageable pada _ai.PlayerTarget.
                _cooldownTimer = _ai.AttackCooldown;
            }
        }

        public void Exit()
        {
            _ai.Agent.isStopped = false;
        }
    }
}
