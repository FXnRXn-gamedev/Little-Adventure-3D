using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace FXnRXn
{
	/// <summary>
	/// Base enemy class with stats, behavior, and state management
	/// Implements IDamageable interface
	/// </summary>
	[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(EnemyStateMachine))]
	[RequireComponent(typeof(EnemyDetectionSystem))]
    public class EnemyBase : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Enemy Data")]
	    [field: SerializeField] protected EnemyData enemyData;
        
	    [Title("References")]
	    [ReadOnly] [field: SerializeField] protected Transform player;
	    [ReadOnly] [field: SerializeField] protected Animator animator;
        
	    [Title("Debug")]
	    [field: SerializeField] protected bool showDebugInfo = false;
	    [field: SerializeField] protected bool showGizmos = true;

	    // Components
	    protected NavMeshAgent _agent;
	    protected EnemyStateMachine _stateMachine;
	    protected EnemyDetectionSystem _detectionSystem;

	    // Stats
	    protected float _currentHealth;
	    protected float _maxHealth;
	    protected bool _isDead;

	    // Combat
	    protected float _lastAttackTime;
	    protected Transform _currentTarget;

	    // Events
	    public event Action<float, float> OnHealthChanged;
	    public event Action OnDeath;
	    public event Action<GameObject> OnTargetAcquired;
	    public event Action OnTargetLost;

	    // Properties
	    


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        private void Awake()
        {
	        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
	        if (_stateMachine == null) _stateMachine = GetComponent<EnemyStateMachine>();
	        if (_detectionSystem == null) _detectionSystem = GetComponent<EnemyDetectionSystem>();
	        if (animator == null) animator = GetComponent<Animator>();

	        InitializeStats();
	        ConfigureAgent();
        }

        protected virtual void Update()
        {
	        // if (CanAttack())
	        // {
		       //  TryAttack();
		       //  _lastAttackTime = Time.time;
	        // }
        }


        // ------------------------------------------- Initialize ------------------------------------------------------

        protected virtual void InitializeStats()
        {
	        if (enemyData == null)
	        {
		        Debug.LogError($"[EnemyBase] EnemyData not assigned on {gameObject.name}");
		        return;
	        }

	        _maxHealth = enemyData.maxHealth;
	        _currentHealth = _maxHealth;
	        _isDead = false;
        }

        protected virtual void ConfigureAgent()
        {
	        if (_agent != null && enemyData != null)
	        {
		        _agent.speed = enemyData.moveSpeed;
		        _agent.angularSpeed = 360f;
		        _agent.acceleration = 8f;
		        _agent.stoppingDistance = enemyData.attackRange * 0.8f;
	        }
        }
        
        // ------------------------------------ IDamageable Implementation ---------------------------------------------


    	// -------------------------------------------- Combat ---------------------------------------------------------
	    
	    public virtual bool CanAttack()
	    {
		    float distanceToTarget = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
		    if (distanceToTarget > 2f) return false;
		    
		    float timeSinceLastAttack = Time.time - _lastAttackTime;
		    return timeSinceLastAttack >= 1.5f;
	    }

	    public virtual void Attack()
	    {
		    TryAttack();
	    }

	    public virtual void TryAttack()
	    {
		    IDamageable targetDamageable = PlayerController.Instance.GetComponent<IDamageable>();
		    if(targetDamageable == null) return;
		    
		    Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
		    targetDamageable.TakeDamage(this, direction, 5f, PlayerController.Instance.transform.position, Vector3.up);
	    }


	    // ----------------------------------------- Target Management -------------------------------------------------

	    public virtual void SetTarget(Transform target)
	    {
		    if (_currentTarget != target)
		    {
			    _currentTarget = target;
			    OnTargetAcquired?.Invoke(target.gameObject);

			    if (showDebugInfo)
			    {
				    Debug.Log($"[EnemyBase] {enemyData.enemyName} acquired target: {target.name}");
			    }
		    }
	    }
	    public virtual void ClearTarget()
	    {
		    if (_currentTarget != null)
		    {
			    OnTargetLost?.Invoke();
			    _currentTarget = null;

			    if (showDebugInfo)
			    {
				    Debug.Log($"[EnemyBase] {enemyData.enemyName} lost target");
			    }
		    }
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    public Transform CurrentTarget => _currentTarget;
	    public void SetCurrentTarget(Transform target) => _currentTarget = target;
	    
	    public float CurrentHealth => _currentHealth;
	    public float MaxHealth => _maxHealth;
	    public bool IsDead => _isDead;
	    public EnemyData EnemyData => enemyData;
	    public NavMeshAgent Agent => _agent;
	    public EnemyStateMachine StateMachine => _stateMachine;
	    
	    
	    // --------------------------------------------- Debug ---------------------------------------------------------

	    private void OnDrawGizmosSelected()
	    {
		    if(!showGizmos) return;
		    // Draw attack range
		    Gizmos.color = Color.red;
		    Gizmos.DrawWireSphere(transform.position, 2f);
	    }
    }
}