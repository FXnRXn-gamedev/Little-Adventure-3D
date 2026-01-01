using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Handles player movement with smooth acceleration/deceleration
	/// Supports 8-way directional movement and camera-relative controls
	/// Uses CharacterController for physics-based movement
	/// </summary>
	[RequireComponent(typeof(CharacterController))]
	public class PlayerMovementController : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------


	    [Title("Data")]
	    [field: SerializeField] private MovementData		movementData;
	    
	    [Title("Debug")]
	    [field: SerializeField] private bool				showDebugInfo = false;



	    
	    private PlayerController		_playerController;
	    private CharacterController		_controller;
	    private Vector3					_velocity;
	    private Vector3					_currentMovement;
	    private Vector2					_movementInput;
	    private bool					_isRunning;
	    private bool					_isGrounded;
	    private float					_currentSpeed;
	    private float					_angleDeltaRad;

	    // ---------------------------------------- Initialization -----------------------------------------------------
	    public void Init(PlayerController pc)
	    {
		    _playerController = pc;
		    if (movementData == null) movementData = Resources.Load<MovementData>("Data/Player/Movement Data") != null ? 
				    Resources.Load<MovementData>("Data/Player/Movement Data") : 
				    new MovementData();
		    
		    if(_controller == null)_controller = GetComponent<CharacterController>();

		    movementData.canRoll = true;
		    movementData.isRolling = false;
	    }

	    // ---------------------------------------- Public Properties --------------------------------------------------
	    /// <summary>
	    /// Sets the movement input (called from input system)
	    /// </summary>
	    public void SetMovementInput(Vector2 input)
	    {
		    _movementInput = input;
	    }

	    /// <summary>
	    /// Sets the running state
	    /// </summary>
	    public void SetRunning(bool running)
	    {
		    _isRunning = running;
	    }
	    
	    public void CheckGroundStatus()
	    {
		    _isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 
			    movementData.groundCheckDistance, movementData.groundLayer);
			   
	    }
	    
	    public void ApplyGravity()
	    {
		    if (_isGrounded && _velocity.y < 0)
		    {
			    _velocity.y = -2f; // Small downward force to keep grounded
		    }
		    else
		    {
			    _velocity.y += movementData.gravity * Time.deltaTime;
		    }
		    _controller.Move(_velocity * Time.deltaTime);
	    }
	    
	    /// <summary>
	    /// Handles movement logic (call this from FixedUpdate or Update)
	    /// </summary>
	    public void UpdateHandleMovement()
	    {
		    if (_movementInput.magnitude < 0.1f)
		    {
			    // Decelerate
			    _currentMovement = Vector3.Lerp(_currentMovement, Vector3.zero, movementData.deceleration * Time.deltaTime);
		    }
		    else
		    {
			    // Calculate movement direction relative to camera
			    Vector3 moveDirection = CalculateMoveDirection();
			    // moveDirection.Normalize();
			    //moveDirection = Quaternion.Euler(0.0f, -45.0f, 0.0f) * moveDirection;
			    // Determine target speed
			    float targetSpeed = _isRunning ? movementData.runSpeed : movementData.walkSpeed;
                
			    // Accelerate towards target speed
			    Vector3 targetMovement = moveDirection * targetSpeed;
			    _currentMovement = Vector3.Lerp(_currentMovement, targetMovement, 
				    movementData.acceleration * Time.deltaTime);
                
			    // Rotate character to face movement direction
			    RotateTowardsMovement(moveDirection);
		    }
		    
		    // Apply movement
		    _controller.Move(_currentMovement * Time.deltaTime);
		    _currentSpeed = _currentMovement.magnitude;

		    // Update animation parameters
	    }

	    public void RollStartInputPressed()
	    {
		    if(movementData == null || !movementData.canRoll) return;
		    movementData.canRoll = false;
		    movementData.isRolling = true;
		    
		    RollStartAsync().Forget();
		    RollCooldownAsync().Forget();
	    }

	    private async UniTask RollStartAsync()
	    {
		    movementData.isRolling = true;
		    PlayerController.Instance.GetPlayerAnimationHandler.RollAnimation();

		    Vector3 rollDirection = _currentMovement.magnitude > 0.1f ? _currentMovement.normalized : transform.forward;
		    float rollStartTime = Time.time;

		    while (Time.time - rollStartTime < movementData.rollingTime)
		    {
			    _controller.Move(rollDirection * movementData.rollSpeed * Time.deltaTime);
			    await UniTask.Yield();
		    }
		   // await UniTask.Delay(TimeSpan.FromSeconds(movementData.rollingTime));
		    movementData.isRolling = false;
	    }

	    private async UniTask RollCooldownAsync()
	    {
		    await UniTask.Delay(TimeSpan.FromSeconds(movementData.rollCoolingTime));
		    movementData.canRoll = true;
	    }

	    // ---------------------------------------- Private Properties -------------------------------------------------
	    
	    public Vector3 CalculateMoveDirection()
	    {
		    // Get camera forward and right vectors (flattened)
		    Vector3 cameraForward = TopDownCameraController.Instance.GetMainCameraForward;
		    Vector3 cameraRight = TopDownCameraController.Instance.GetMainCameraRight;
            
		    cameraForward.y = 0f;
		    cameraRight.y = 0f;
		    cameraForward.Normalize();
		    cameraRight.Normalize();

		    // Calculate movement direction
		    Vector3 moveDirection = (cameraForward * _movementInput.y + cameraRight * _movementInput.x).normalized;
            
		    // Handle slope movement
		    if (IsOnSlope())
		    {
			    moveDirection = Vector3.ProjectOnPlane(moveDirection, GetSlopeNormal()).normalized;
		    }

		    return moveDirection.normalized;
	    }

	    private void RotateTowardsMovement(Vector3 direction)
	    {
		    if (direction.magnitude < 0.1f)
		    {
			    _angleDeltaRad = 0f;
			    return;
		    }
		    
		    // Calculate angle delta between current facing and movement direction
		    Vector3 currentForward = transform.forward;
		    currentForward.y = 0f;
		    currentForward.Normalize();

		    Vector3 targetDirection = direction;
		    targetDirection.y = 0f;
		    targetDirection.Normalize();

		    // Calculate signed angle in radians
		    _angleDeltaRad = Vector3.SignedAngle(currentForward, targetDirection, Vector3.up) * Mathf.Deg2Rad;
		    
		    //DebugSystem.Info($"Angle: {_angleDeltaRad:F2} | Direction: {direction} | Current: {currentForward}");


		    Quaternion targetRotation = Quaternion.LookRotation(direction);
		    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
			    movementData.rotationSpeed * Time.deltaTime);
	    }
	    
	    private bool IsOnSlope()
	    {
		    if (!_isGrounded) return false;

		    RaycastHit hit;
		    if (Physics.Raycast(transform.position, Vector3.down, out hit, 
			        movementData.groundCheckDistance + 0.1f, movementData.groundLayer))
		    {
			    float angle = Vector3.Angle(Vector3.up, hit.normal);
			    return angle > 0f && angle < movementData.maxSlopeAngle;
		    }

		    return false;
	    }

	    private Vector3 GetSlopeNormal()
	    {
		    RaycastHit hit;
		    if (Physics.Raycast(transform.position, Vector3.down, out hit, 
			        movementData.groundCheckDistance + 0.1f, movementData.groundLayer))
		    {
			    return hit.normal;
		    }

		    return Vector3.up;
	    }


	    // ------------------------------------------ Helper Method ----------------------------------------------------

	    public Vector3 GetCurrentMovement => _currentMovement;
	    public Vector3 Velocity => _velocity;
	    public bool IsGrounded => _isGrounded;
	    public bool IsMoving => _movementInput.magnitude > 0.1f;
	    public float CurrentSpeed => _currentSpeed;
	    public Vector2 MovementInput => _movementInput;
	    public MovementData GetMovementData => movementData;
	    public float AngleDeltaRad => _angleDeltaRad;
	    
	    public void DebugInfo()
	    { 
		    if (showDebugInfo) DebugSystem.Info($"Speed: {_currentSpeed:F2} | Grounded: {_isGrounded} | Input: {_movementInput}");
	    }
	    
	    private void OnDrawGizmosSelected()
	    {
		    // Draw ground check ray
		    Gizmos.color = _isGrounded ? Color.green : Color.red;
		    Vector3 start = transform.position + Vector3.up * 0.1f;
		    Vector3 end = start + Vector3.down * movementData.groundCheckDistance;
		    Gizmos.DrawLine(start, end);
	    }

    }

	
}