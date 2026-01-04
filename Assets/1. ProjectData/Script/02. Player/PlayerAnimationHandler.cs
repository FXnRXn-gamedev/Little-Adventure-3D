using System;
using UnityEngine;

namespace FXnRXn
{
	[RequireComponent(typeof(Animator))]
    public class PlayerAnimationHandler : MonoBehaviour
    {
	    // ------------------------------------------ Animator Hash ----------------------------------------------------
	    
	    public readonly int Grounded_HASH						= Animator.StringToHash("Grounded");
	    public readonly int ForwardSpeed_HASH					= Animator.StringToHash("ForwardSpeed");
	    public readonly int Roll_HASH							= Animator.StringToHash("Roll");
	    public readonly int AngleDeltaRad_HASH					= Animator.StringToHash("AngleDeltaRad");
	    public readonly int Death_HASH							= Animator.StringToHash("Death");
	    public readonly int Hurt_HASH							= Animator.StringToHash("Hurt");
	    public readonly int StateTime_HASH						= Animator.StringToHash("StateTime");
	    public readonly int MeleeAttack_HASH					= Animator.StringToHash("MeleeAttack");
	    
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    private Animator _animator;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        if (_animator == null) _animator = GetComponent<Animator>();
        }
        
        public void Init(PlayerController pc)
        {
	        
        }

        // ---------------------------------------- Locomotion Animation -----------------------------------------------
        public void UpdateLocomotionAnimations()
        {
	        if(_animator == null) return;
	        
	        // Calculate speed
	        _animator.SetFloat(ForwardSpeed_HASH, PlayerController.Instance.GetPlayerMovementController.CurrentSpeed);
	        _animator.SetBool(Grounded_HASH, PlayerController.Instance.GetPlayerMovementController.IsGrounded);
        }

        public void FixedUpdateLocomotionAnimations()
        {
	        _animator.SetFloat(StateTime_HASH, Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
	        //_animator.ResetTrigger(MeleeAttack_HASH);
        }

        public void RollAnimation()
        {
	        if(_animator == null) return;
	        _animator.SetTrigger(Roll_HASH);
        }

        public void HurtAnimation()
        {
	        if(_animator == null) return;
	        _animator.SetTrigger(Hurt_HASH);
        }

        public void DeathAnimation()
        {
	        if(_animator == null) return;
	        _animator.SetTrigger(Death_HASH);
        }

        public void LightAttackAnimation()
        {
	        if(_animator == null) return;
	        _animator.SetTrigger(MeleeAttack_HASH);
        }
        
        
        // ---------------------------------------- Animation Properties -----------------------------------------------
	    
        /// <summary>
        /// Called when respawn animaiton ends from animation event
        /// </summary>
        public void RespawnAnimationEnd()
        {
	        if(PlayerController.Instance == null) return;
	        
	        PlayerController.Instance.SetReadyToMove(true);
	        PlayerController.Instance.PlayerState = PlayerMoveState.Idle;
        }


    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------

	    
    }
}