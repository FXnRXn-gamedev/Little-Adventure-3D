using UnityEngine;
using Sirenix.OdinInspector;


namespace FXnRXn
{
	/// <summary>
	/// ScriptableObject containing movement parameters
	/// Allows for easy tweaking and different movement profiles
	/// </summary>
	[CreateAssetMenu(fileName = "New Movement Data", menuName = "TopDownRPG/Movement/Movement Data")]
	public class MovementData : ScriptableObject
	{
		[BoxGroup("Movement")]
		public float walkSpeed = 5f;
		[BoxGroup("Movement")]
		public float runSpeed = 8f;
		[BoxGroup("Movement")]
		public float acceleration = 10f;
		[BoxGroup("Movement")]
		public float deceleration = 15f;
		[BoxGroup("Movement")]
		public float rotationSpeed = 720f;

		[BoxGroup("Dash")]
		public float dashDistance = 5f;
		[BoxGroup("Dash")]
		public float dashDuration = 0.2f;
		[BoxGroup("Dash")]
		public float dashCooldown = 1f;
		[BoxGroup("Dash")]
		public float dashStaminaCost = 20f;
		[BoxGroup("Dash")]
		public float iFramesDuration = 0.15f;

		[BoxGroup("Roll")] 
		public bool canRoll = true;
		[BoxGroup("Roll")] 
		public bool isRolling = false;
		[BoxGroup("Roll")] 
		public float rollingTime = 0.4f;
		[BoxGroup("Roll")] 
		public float rollCoolingTime = 1f;
		[BoxGroup("Roll")] 
		public float rollSpeed = 8f;

		[BoxGroup("Physics")]
		public float gravity = -20f;
		[BoxGroup("Physics")]
		public float maxSlopeAngle = 45f;
		[BoxGroup("Physics")]
		public float stepOffset = 0.3f;

		[BoxGroup("Ground Detection")]
		public LayerMask groundLayer;
		[BoxGroup("Ground Detection")]
		public float groundCheckDistance = 0.2f;

		[BoxGroup("Animation")]
		public float animationSmoothTime = 0.1f;
	}
}

