using System;
using UnityEngine;

namespace FXnRXn
{
	[RequireComponent(typeof(Animator))]
    public class PlayerAnimationHandler : MonoBehaviour
    {
	    // ------------------------------------------ Animator Hash ----------------------------------------------------
	    
	    public static readonly int GROUNDED_HASH						= Animator.StringToHash("Grounded");
	    public static readonly int FORWARDSPEDD_HASH					= Animator.StringToHash("ForwardSpeed");
	    public static readonly int ROLL_HASH							= Animator.StringToHash("Roll");
	    public static readonly int ANGLEDELTARAD_HASH					= Animator.StringToHash("AngleDeltaRad");
	    
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    private Animator _animator;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        if (_animator == null) _animator = GetComponent<Animator>();
        }

        // ---------------------------------------- Locomotion Animation -----------------------------------------------
        public void UpdateLocomotionAnimations()
        {
	        if(_animator == null) return;
	        
	        // Calculate speed
	        _animator.SetFloat(FORWARDSPEDD_HASH, PlayerController.Instance.GetPlayerMovementController.CurrentSpeed);
	        //_animator.SetFloat(ANGLEDELTARAD_HASH, PlayerController.Instance.GetPlayerMovementController.AngleDeltaRad);
        }

        public void RollAnimation()
        {
	        if(_animator == null) return;
	        _animator.SetTrigger(ROLL_HASH);
        }
        
        
        // ---------------------------------------- Animation Properties -----------------------------------------------
	    
        /// <summary>
        /// Called when respawn animaiton ends from animation event
        /// </summary>
        public void RespawnAnimationEnd()
        {
	        if(PlayerController.Instance == null) return;
	        
	        PlayerController.Instance.SetReadyToMove(true);
	        PlayerController.Instance.MoveState = PlayerMoveState.Idle;
        }


    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}