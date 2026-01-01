using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Handles player functionality
	/// </summary>
    [RequireComponent(typeof(PlayerMovementController))]
	[RequireComponent(typeof(PlayerCollisionHandler))]
	public class PlayerController : MonoBehaviour, IDamageable
    {
	    // ------------------------------------------- Singleton -------------------------------------------------------
	    
	    public static PlayerController Instance { get; private set; }
	    
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [Title("Data")] 
	    [field: SerializeField]private PlayerStatData playerStatData;
	    
	    
	    [Title("References")] 
	    [ReadOnly] [field: SerializeField] private PlayerMovementController movementController;
	    [ReadOnly] [field: SerializeField] private PlayerCollisionHandler collisionHandler;
	    [ReadOnly] [field: SerializeField] private PlayerAnimationHandler animationHandler;
	    [ReadOnly] [field: SerializeField] private WeaponManager weaponManager;

	    public bool ReadyToMove { get; private set; } = true;

	    private float _currentHealth;
	    private Vector2 _input;

	    
	    public PlayerMoveState PlayerState { get; set; } = PlayerMoveState.Respawning;
	    
	    // ---------------------------------------- Unity Callback -----------------------------------------------------
	    private void Awake()
	    {
		    if (Instance == null) Instance = this;
		    if (playerStatData == null) playerStatData = Resources.Load<PlayerStatData>("Data/Player/PlayerStatData");
		    if (movementController == null) movementController = GetComponent<PlayerMovementController>();
		    if (collisionHandler == null) collisionHandler = GetComponent<PlayerCollisionHandler>();
		    if (animationHandler == null) animationHandler = GetComponent<PlayerAnimationHandler>();
		    if (weaponManager == null) weaponManager = GetComponentInChildren<WeaponManager>();
		    
		    


		    movementController?.Init(this);
		    collisionHandler?.Init(this);
		    animationHandler?.Init(this);
		    weaponManager?.Init();

		    _currentHealth = playerStatData?.baseHealth ?? 100f;
		    
		    ReadyToMove = false;
		    PlayerState = PlayerMoveState.Respawning;
	    }

	    private void Update()
	    {
			if(IsDead) return;
			
		    if (movementController != null)
		    {
			    movementController.CheckGroundStatus();
			    movementController.ApplyGravity();
			    movementController.DebugInfo();
		    }
		    
		    if (collisionHandler != null)
		    {
			    
		    }
		    
		    if (animationHandler != null)
		    {
			    animationHandler.UpdateLocomotionAnimations();
		    }
		    
		    UpdateMovement();
	    }

	    private void FixedUpdate()
	    {
		    if (animationHandler != null) animationHandler.FixedUpdateLocomotionAnimations();
	    }


	    // ------------------------------------------ Interface Method -------------------------------------------------

	    public float CurrentHealth => _currentHealth;
	    public float MaxHealth => playerStatData?.baseHealth ?? 100f;
	    public bool IsDead => movementController.GetMovementData?.isDead ?? false;
	    public void TakeDamage(MonoBehaviour damager, Vector3 direction, float damage, Vector3 hitPoint, Vector3 hitNormal)
	    {
		    // Check if already dead or in invulnerability period
		    if (IsDead || movementController.GetMovementData.isHurting) return;
		    
		    // Trigger Hurt State
		    movementController.GetMovementData.isHurting = true;
		    // movementController.GetMovementData.dontHurtTime = 1f;
		    HurtCooldownTimerAsync().Forget();
		    
		    // Apply damage
		    _currentHealth -= damage;
		    _currentHealth = Mathf.Max(0, _currentHealth);
			
		    // Hurt Animation
		    animationHandler.HurtAnimation();

		    
		    
		    // TODO : Apply knockback if needed
		    // if (direction != Vector3.zero)
		    // {
		    //  movementController.ApplyKnockback(direction, damage);
		    // }
		    
		    // Check for death
		    if (_currentHealth <= 0)
		    {
			    Die();
		    }
    
		    // TODO : Visual feedback
		    // TODO : PlayHitEffect(hitPoint, hitNormal);
		    // TODO : PlayDamageSound();
	    }

	    public void Heal(float amount)
	    {
		    if (IsDead) return; // Can't heal if dead
    
		    _currentHealth += amount;
		    _currentHealth = Mathf.Min(_currentHealth, MaxHealth); // Clamp to max
    
		    // Optional: Visual feedback
		    // PlayHealEffect();
		    // PlayHealSound();
	    }
	    
	    public void Stunned()
	    {
		    if (IsDead) return; // Already dead
		    
		    PlayerState = PlayerMoveState.Stunned;
		    movementController.GetMovementData.isStunned = true;
	    }

	    public void Die()
	    {
		    if (IsDead) return; // Already dead
    
		    _currentHealth = 0;
		    movementController.GetMovementData.isDead = true;
		    animationHandler.DeathAnimation();
    
		    // Optional: Additional death logic
		    // DropItems();
		    // DisableControls();
		    // PlayDeathAnimation();
		    // RespawnAfterDelay();
	    }

	    // ---------------------------------------- Private Properties -------------------------------------------------
	    
	    private void UpdateMovement()
	    {
		    
		    // State Update
		    if (movementController.GetMovementData.isRolling)
		    {
			    PlayerState = PlayerMoveState.Rolling;
		    }
		    else if (movementController.GetMovementData.isHurting)
		    {
			    PlayerState = PlayerMoveState.Hurt;
		    }
		    else if (movementController.GetMovementData.isDead)
		    {
			    PlayerState = PlayerMoveState.Death;
		    }
		    else
		    {
			    PlayerState = MobileInputAdapter.Instance.GetInputMagnitude() > 0.1f
				    ? PlayerMoveState.Running
				    : PlayerMoveState.Idle;
		    }
		    
		    // MoveInput
		    
		    if(! ReadyToMove) return;
		    if(PlayerController.Instance.PlayerState == PlayerMoveState.Death || 
		       PlayerController.Instance.PlayerState == PlayerMoveState.Stunned) return;
		    
		    

		    if (MobileInputAdapter.Instance.EnableMobileControls())
		    {
			    if (movementController != null && MobileInputAdapter.Instance != null)
			    {
				    _input = MobileInputAdapter.Instance.GetInputDirection();
				    movementController.SetMovementInput(_input);

				    // Auto-run if joystick is pushed far enough
				    bool isRunning = MobileInputAdapter.Instance.GetInputMagnitude() > 0.7f;
				    movementController.SetRunning(isRunning);
			    }
		    }

		    if (KeyboardInputHandler.Instance != null)
		    {
			    _input = KeyboardInputHandler.Instance.OnKeyboardUpdateMove();
			    movementController.SetMovementInput(_input);
			    
			    bool isRunning = _input.magnitude > 0.7f;
			    movementController.SetRunning(isRunning);
		    }
		    
		    
		    if(movementController != null) movementController.UpdateHandleMovement();
		    
		    
	    }

	    private async UniTask HurtCooldownTimerAsync()
	    {
		    float hurtStartTime = Time.time;
		    while (Time.time - hurtStartTime < movementController.GetMovementData.dontHurtTime)
		    {
			    await UniTask.Yield();
		    }
		    movementController.GetMovementData.isHurting = false;
	    }
	    


	    // ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    public WeaponManager GetWeaponManager => weaponManager;
	    public PlayerMovementController GetPlayerMovementController => movementController;
	    public PlayerCollisionHandler GetPlayerCollisionHandler => collisionHandler;
	    public PlayerAnimationHandler GetPlayerAnimationHandler => animationHandler;

	    public PlayerStatData GetPlayerStatData => playerStatData;
	    public void SetReadyToMove(bool value) => ReadyToMove = value;
	    
	    
    }
	
	[Serializable]
	public enum PlayerMoveState
	{
		Respawning,
		Idle,
		Running,
		Rolling,
		Attacking,
		Dashing,
		Death,
		Stunned,
		Hurt
	}
}

