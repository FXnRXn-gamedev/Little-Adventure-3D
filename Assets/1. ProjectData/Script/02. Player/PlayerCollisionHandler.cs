using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FXnRXn
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [TriInspector.Title("Collision Settings")]
	    [field: SerializeField] private float		pushForce = 5f;
	    [field: SerializeField] private float		collisionDamping = 0.5f;
	    [field: SerializeField] private LayerMask	pushableLayer;
	    [field: SerializeField] private LayerMask	enemyLayer;
	    [field: SerializeField] private LayerMask	obstacleLayer;
	    [field: SerializeField] private LayerMask	stairLayer;

	    [TriInspector.Title("Stair Settings")]
	    [field: SerializeField] private float		stepHeight = 0.3f;
	    [field: SerializeField] private float		stepSearchDistance = 0.6f;
	    [field: SerializeField] private float		stepSmooth = 0.2f;
	    [field: SerializeField] private float		stairSlopeAngle = 45f; // Simulated slope angle
	    [field: SerializeField] private float		stairTransitionSpeed = 8f;
	    [field: SerializeField] private float      maxStairAngle = 60f; // Maximum angle for stairs
	    [field: SerializeField] private float      stairCheckRadius = 0.2f; // Radius for stair dete

	    [TriInspector.Title("Events")]
	    public UnityEvent<Collider> OnEnemyCollision;
	    public UnityEvent<Collider> OnObstacleCollision;
	    public UnityEvent<ControllerColliderHit> OnAnyCollision;

	    [TriInspector.Title("Debug")]
	    [SerializeField] private bool showCollisionGizmos = false;

	    // Private fields
	    private MovementData			_movementData;
	    private PlayerController		_playerController;
	    private CharacterController		_controller;
	    private Vector3					_velocity;
	    private Vector3 				_currentMovement;
	    private Vector3 				_externalForce;
	    private Vector3 				_collisionVelocity;
	    private float 					_stepOffset;
	    private bool 					_isOnStairs;
	    private Vector3					_targetStairNormal = Vector3.up; // Normal of the stair plane
	    private float					_currentStairBlend = 0f; // Smooth blend factor
	    // Collision tracking
	    private List<ControllerColliderHit> _frameCollisions = new List<ControllerColliderHit>();
	    private Dictionary<Collider, float> _collisionCooldowns = new Dictionary<Collider, float>();
	    private const float COLLISION_COOLDOWN = 0.2f;
	    
	    // Stair handling fields
	    private Vector3 _stairMovementAdjustment = Vector3.zero;
	    private float _stairVerticalOffset = 0f;
	    private RaycastHit _stairHit;
	    private bool _isSteppingUp = false;
	    private float _stepProgress = 0f;
		
	    

	    // ---------------------------------------- Initialization -----------------------------------------------------
	    public void Init(PlayerController pc)
	    {
		    _playerController = pc;
		    if(_controller == null)_controller = GetComponent<CharacterController>();
		    if (_movementData == null) _movementData = Resources.Load<MovementData>("Data/Player/Movement Data");
		    
	    }


    	// ------------------------------------------ Stair Handling ---------------------------------------------------

    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    // ---------------------------------------------  GIZMOS  ------------------------------------------------------

	    private void OnDrawGizmosSelected()
	    {
		   
	    }
    }
    }