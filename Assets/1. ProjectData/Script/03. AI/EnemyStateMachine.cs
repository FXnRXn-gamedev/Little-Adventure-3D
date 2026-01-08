using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// State machine for enemy AI
	/// Manages transitions between Idle, Patrol, Chase, Attack, Flee, and Dead states
	/// </summary>
    [RequireComponent(typeof(EnemyBase))]
	public class EnemyStateMachine : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [Title ("References")]
	    [ReadOnly] [field: SerializeField] private EnemyBase enemyBase;
        
	    [Title ("Current State")]
	    [field: SerializeField] private EnemyState currentState = EnemyState.Idle;
        
	    [Title("Debug")]
	    [field: SerializeField] private bool showDebugInfo = false;

	    private IEnemyState[] _states;
	    private IEnemyState _currentStateInstance;

	    public EnemyState CurrentState => currentState;
	    public event Action<EnemyState, EnemyState> OnStateChanged;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        if (enemyBase == null) enemyBase = GetComponent<EnemyBase>();

	        InitializeStates();
        }

        private void Start()
        {
	        // Enter initial state
	        ChangeState(currentState);
        }

        private void Update()
        {
	        _currentStateInstance?.Update();
        }

        private void FixedUpdate()
        {
	        _currentStateInstance?.FixedUpdate();
        }


    	// ---------------------------------------- Public Properties --------------------------------------------------


    	// ---------------------------------------- Private Properties -------------------------------------------------
	    private void InitializeStates()
	    {
		    _states = new IEnemyState[(int)EnemyState.Count];
            
		    _states[(int)EnemyState.Idle] = new IdleState(this, enemyBase);
		    _states[(int)EnemyState.Patrol] = new PatrolState(this, enemyBase);
		    _states[(int)EnemyState.Chase] = new ChaseState(this, enemyBase);
		    _states[(int)EnemyState.Attack] = new AttackState(this, enemyBase);
		    _states[(int)EnemyState.Flee] = new FleeState(this, enemyBase);
		    _states[(int)EnemyState.Dead] = new DeadState(this, enemyBase);
	    }

	    /// <summary>
	    /// Changes to a new state
	    /// </summary>
	    public void ChangeState(EnemyState newState)
	    {
		    if (currentState == newState) return;

		    EnemyState previousState = currentState;

		    // Exit current state
		    _currentStateInstance?.Exit();

		    // Change state
		    currentState = newState;
		    _currentStateInstance = _states[(int)newState];

		    // Enter new state
		    _currentStateInstance?.Enter();

		    OnStateChanged?.Invoke(previousState, newState);

		    if (showDebugInfo)
		    {
			    Debug.Log($"[EnemyStateMachine] {enemyBase.EnemyData.enemyName}: {previousState} -> {newState}");
		    }
	    }

	    /// <summary>
	    /// Forces a state change (bypasses checks)
	    /// </summary>
	    public void ForceState(EnemyState newState)
	    {
		    ChangeState(newState);
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
	
	public enum EnemyState
	{
		Idle,
		Patrol,
		Chase,
		Attack,
		Flee,
		Dead,
		Count // Used for array sizing
	}
	
	#region State Interfaces and Implementations
	public interface IEnemyState
	{
		void Enter();
		void Update();
		void FixedUpdate();
		void Exit();
	}

	public class IdleState : IEnemyState
	{
		private EnemyStateMachine _stateMachine;
		private EnemyBase _enemy;
		private float _idleTime;
		private float _idleDuration = 2f;
		
		public IdleState(EnemyStateMachine stateMachine, EnemyBase enemy)
		{
			_stateMachine = stateMachine;
			_enemy = enemy;
		}

		public void Enter()
		{
			_idleTime = 0f;
			if (_enemy.Agent != null)
			{
				_enemy.Agent.isStopped = true;
			}
		}

		public void Update()
		{
			_idleTime += Time.deltaTime;

			// Check for target
			if (_enemy.CurrentTarget != null)
			{
				_stateMachine.ChangeState(EnemyState.Chase);
				return;
			}

			// Return to patrol after idle duration
			if (_idleTime >= _idleDuration && _enemy.EnemyData.hasPatrolRoute)
			{
				_stateMachine.ChangeState(EnemyState.Patrol);
			}
		}

		public void FixedUpdate() { }

		public void Exit()
		{
			if (_enemy.Agent != null)
			{
				_enemy.Agent.isStopped = false;
			}
		}
		
	}
	
	public class PatrolState : IEnemyState
    {
        private EnemyStateMachine _stateMachine;
        private EnemyBase _enemy;
        private PatrolRoute _patrolRoute;
        private float _waitTimer;
        private bool _isWaiting;

        public PatrolState(EnemyStateMachine stateMachine, EnemyBase enemy)
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
            _patrolRoute = enemy.GetComponent<PatrolRoute>();
        }

        public void Enter()
        {
            if (_enemy.Agent != null)
            {
                _enemy.Agent.speed = _enemy.EnemyData.patrolSpeed;
                _enemy.Agent.isStopped = false;
            }

            // Set destination to first waypoint if patrol route exists
            if (_patrolRoute != null && _patrolRoute.GetWaypointCount() > 0)
            {
                Transform waypoint = _patrolRoute.GetCurrentWaypoint();
                if (waypoint != null && _enemy.Agent != null)
                {
                    _enemy.Agent.SetDestination(waypoint.position);
                }
            }
        }

        public void Update()
        {
            // Check for target
            if (_enemy.CurrentTarget != null)
            {
                _stateMachine.ChangeState(EnemyState.Chase);
                return;
            }

            // Handle patrol with route
            if (_patrolRoute != null && _patrolRoute.GetWaypointCount() > 0)
            {
                if (_isWaiting)
                {
                    _waitTimer -= Time.deltaTime;
                    if (_waitTimer <= 0f)
                    {
                        _isWaiting = false;
                        
                        // Move to next waypoint
                        Transform nextWaypoint = _patrolRoute.GetNextWaypoint();
                        if (nextWaypoint != null && _enemy.Agent != null)
                        {
                            _enemy.Agent.SetDestination(nextWaypoint.position);
                        }
                    }
                }
                else
                {
                    // Check if reached waypoint
                    if (_patrolRoute.IsAtWaypoint(_enemy.transform.position))
                    {
                        _isWaiting = true;
                        _waitTimer = _patrolRoute.GetWaitTime();
                        
                        if (_enemy.Agent != null)
                        {
                            _enemy.Agent.isStopped = true;
                        }
                    }
                }
            }
            else
            {
                // No patrol route, just idle
                if (_enemy.Agent != null && !_enemy.Agent.hasPath)
                {
                    _stateMachine.ChangeState(EnemyState.Idle);
                }
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            _isWaiting = false;
            _waitTimer = 0f;
            
            if (_enemy.Agent != null)
            {
                _enemy.Agent.isStopped = false;
            }
        }
    }
	
	public class ChaseState : IEnemyState
	{
		private EnemyStateMachine _stateMachine;
		private EnemyBase _enemy;

		public ChaseState(EnemyStateMachine stateMachine, EnemyBase enemy)
		{
			_stateMachine = stateMachine;
			_enemy = enemy;
		}

		public void Enter()
		{
			if (_enemy.Agent != null)
			{
				_enemy.Agent.speed = _enemy.EnemyData.moveSpeed;
				_enemy.Agent.isStopped = false;
			}
		}

		public void Update()
		{
			// Lost target
			if (_enemy.CurrentTarget == null)
			{
				_stateMachine.ChangeState(EnemyState.Idle);
				return;
			}

			// Check if should flee
			if (_enemy.EnemyData.canFlee && 
			    _enemy.CurrentHealth / _enemy.MaxHealth <= _enemy.EnemyData.fleeHealthThreshold)
			{
				_stateMachine.ChangeState(EnemyState.Flee);
				return;
			}

			// Move towards target
			if (_enemy.Agent != null)
			{
				_enemy.Agent.SetDestination(_enemy.CurrentTarget.position);
			}

			// Check if in attack range
			float distanceToTarget = Vector3.Distance(_enemy.transform.position, _enemy.CurrentTarget.position);
			if (distanceToTarget <= _enemy.EnemyData.attackRange)
			{
				_stateMachine.ChangeState(EnemyState.Attack);
			}
		}

		public void FixedUpdate() { }

		public void Exit() { }
	}
	
	public class AttackState : IEnemyState
    {
        private EnemyStateMachine _stateMachine;
        private EnemyBase _enemy;

        public AttackState(EnemyStateMachine stateMachine, EnemyBase enemy)
        {
            _stateMachine = stateMachine;
            _enemy = enemy;
        }

        public void Enter()
        {
            if (_enemy.Agent != null)
            {
                _enemy.Agent.isStopped = true;
            }
        }

        public void Update()
        {
            // Lost target
            if (_enemy.CurrentTarget == null)
            {
                _stateMachine.ChangeState(EnemyState.Idle);
                return;
            }

            // Check if should flee
            if (_enemy.EnemyData.canFlee && 
                _enemy.CurrentHealth / _enemy.MaxHealth <= _enemy.EnemyData.fleeHealthThreshold)
            {
                _stateMachine.ChangeState(EnemyState.Flee);
                return;
            }

            // Check if target out of range
            float distanceToTarget = Vector3.Distance(_enemy.transform.position, _enemy.CurrentTarget.position);
            if (distanceToTarget > _enemy.EnemyData.attackRange * 1.2f)
            {
                _stateMachine.ChangeState(EnemyState.Chase);
                return;
            }

            // Face target
            Vector3 direction = (_enemy.CurrentTarget.position - _enemy.transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, 
                    Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }

            // Attack if possible
            if (_enemy.CanAttack())
            {
                _enemy.Attack();
            }
        }

        public void FixedUpdate() { }

        public void Exit()
        {
            if (_enemy.Agent != null)
            {
                _enemy.Agent.isStopped = false;
            }
        }
    }
	
	public class FleeState : IEnemyState
	{
		private EnemyStateMachine _stateMachine;
		private EnemyBase _enemy;

		public FleeState(EnemyStateMachine stateMachine, EnemyBase enemy)
		{
			_stateMachine = stateMachine;
			_enemy = enemy;
		}

		public void Enter()
		{
			if (_enemy.Agent != null)
			{
				_enemy.Agent.speed = _enemy.EnemyData.moveSpeed * 1.2f; // Flee faster
				_enemy.Agent.isStopped = false;
			}
		}

		public void Update()
		{
			if (_enemy.CurrentTarget == null)
			{
				_stateMachine.ChangeState(EnemyState.Idle);
				return;
			}

			// Flee away from target
			Vector3 fleeDirection = (_enemy.transform.position - _enemy.CurrentTarget.position).normalized;
			Vector3 fleePosition = _enemy.transform.position + fleeDirection * 10f;

			if (_enemy.Agent != null)
			{
				_enemy.Agent.SetDestination(fleePosition);
			}

			// Check if safe (far enough or health recovered)
			float distanceToTarget = Vector3.Distance(_enemy.transform.position, _enemy.CurrentTarget.position);
			if (distanceToTarget > _enemy.EnemyData.deAggroRange || 
			    _enemy.CurrentHealth / _enemy.MaxHealth > _enemy.EnemyData.fleeHealthThreshold * 1.5f)
			{
				_enemy.ClearTarget();
				_stateMachine.ChangeState(EnemyState.Idle);
			}
		}

		public void FixedUpdate() { }

		public void Exit() { }
	}
	
	public class DeadState : IEnemyState
	{
		private EnemyStateMachine _stateMachine;
		private EnemyBase _enemy;

		public DeadState(EnemyStateMachine stateMachine, EnemyBase enemy)
		{
			_stateMachine = stateMachine;
			_enemy = enemy;
		}

		public void Enter()
		{
			if (_enemy.Agent != null)
			{
				_enemy.Agent.isStopped = true;
				_enemy.Agent.enabled = false;
			}
		}

		public void Update() { }

		public void FixedUpdate() { }

		public void Exit() { }
	}
	
	
	#endregion
}