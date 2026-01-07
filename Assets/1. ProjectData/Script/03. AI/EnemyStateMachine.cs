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
		    //_states[(int)EnemyState.Patrol] = new PatrolState(this, enemyBase);
		    //_states[(int)EnemyState.Chase] = new ChaseState(this, enemyBase);
		    //_states[(int)EnemyState.Attack] = new AttackState(this, enemyBase);
		    //_states[(int)EnemyState.Flee] = new FleeState(this, enemyBase);
		    //_states[(int)EnemyState.Dead] = new DeadState(this, enemyBase);
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
	
	
	#endregion
}