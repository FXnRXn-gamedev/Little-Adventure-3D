using System;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Handles player functionality
	/// </summary>
    [RequireComponent(typeof(PlayerMovementController))]
	[RequireComponent(typeof(PlayerCollisionHandler))]
	public class PlayerController : MonoBehaviour
    {
	    // ------------------------------------------- Singleton -------------------------------------------------------
	    
	    public static PlayerController Instance { get; private set; }
	    
	    // ------------------------------------------ Properties -------------------------------------------------------

	    [TriInspector.Title("References")] 
	    [field: SerializeField] private PlayerMovementController movementController;
	    [field: SerializeField] private PlayerCollisionHandler collisionHandler;
	    [field: SerializeField] private PlayerAnimationHandler animationHandler;

	    public bool ReadyToMove { get; private set; } = true;



	    
	    
	    public PlayerMoveState MoveState { get; set; } = PlayerMoveState.Respawning;
	    
	    // ---------------------------------------- Unity Callback -----------------------------------------------------
	    private void Awake()
	    {
		    if (Instance == null) Instance = this;
		    if (movementController == null) movementController = GetComponent<PlayerMovementController>();
		    if (collisionHandler == null) collisionHandler = GetComponent<PlayerCollisionHandler>();
		    if (animationHandler == null) animationHandler = GetComponent<PlayerAnimationHandler>();
		    


		    movementController?.Init(this);
		    collisionHandler?.Init(this);

		    ReadyToMove = false;
		    MoveState = PlayerMoveState.Respawning;
	    }

	    private void Update()
	    {
		    if(! ReadyToMove) return;
		    
		    if (movementController == null) return;
		    movementController.CheckGroundStatus();
		    movementController.ApplyGravity();
		    movementController.UpdateHandleMovement();
		    movementController.DebugInfo();
		    
		    if (collisionHandler == null) return;
		    collisionHandler.HandleStairCollision();
		    
		    if(animationHandler == null) return;
		    animationHandler.UpdateLocomotionAnimations();
		    
		    UpdateMovement();
	    }

	    // ---------------------------------------- Public Properties --------------------------------------------------
	    private void UpdateMovement()
	    {
		    // State Update
		    if (movementController.GetMovementData.isRolling)
		    {
			    MoveState = PlayerMoveState.Rolling;
		    }
		    else
		    {
			    MoveState = MobileInputAdapter.Instance.GetInputMagnitude() > 0.1f
				    ? PlayerMoveState.Running
				    : PlayerMoveState.Idle;
		    }
		    
		    if(! MobileInputAdapter.Instance.EnableMobileControls()) return;
		    
		    if (movementController != null && MobileInputAdapter.Instance != null)
		    {
			    Vector2 input = MobileInputAdapter.Instance.GetInputDirection();
			    movementController.SetMovementInput(input);

			    // Auto-run if joystick is pushed far enough
			    bool isRunning = MobileInputAdapter.Instance.GetInputMagnitude() > 0.7f;
			    movementController.SetRunning(isRunning);
		    }
		    
	    }

	    // ---------------------------------------- Private Properties -------------------------------------------------
		
	    
		
	    

	    // ------------------------------------------ Helper Method ----------------------------------------------------

	    public PlayerMovementController GetPlayerMovementController => movementController;
	    public PlayerCollisionHandler GetPlayerCollisionHandler => collisionHandler;
	    public PlayerAnimationHandler GetPlayerAnimationHandler => animationHandler;
	    public void SetReadyToMove(bool value) => ReadyToMove = value;

    }
	
	[Serializable]
	public enum PlayerMoveState
	{
		Respawning,
		Idle,
		Running,
		Rolling
	}
}