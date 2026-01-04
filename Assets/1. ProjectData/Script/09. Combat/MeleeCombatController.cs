using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
    public class MeleeCombatController : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Attack Settings")]
	    [SerializeField] private float lightAttackDamageMultiplier = 1f;
	    [SerializeField] private float heavyAttackDamageMultiplier = 2f;
	    [SerializeField] private float chargedAttackDamageMultiplier = 3f;
	    [SerializeField] private float chargeTime = 1f;
        
	    [Title("Hit Detection")]
	    [SerializeField] private Transform attackPoint;
	    [SerializeField] private float attackRadius = 1.5f;
	    [SerializeField] private LayerMask enemyLayer;
        
	    [Title("Timing")]
	    [SerializeField] private float attackDuration = 0.5f;
	    [SerializeField] private float attackCooldown = 0.3f;
	    
	    [Title("Debug")]
	    [SerializeField] private bool showDebugInfo = false;
	    
	    
	    
	    private bool _isAttacking;
	    private bool _isCharging;
	    private float _chargeStartTime;
	    private float _lastAttackTime;

	    public bool IsAttacking => _isAttacking;
	    public bool CanAttack => !_isAttacking && Time.time >= _lastAttackTime + attackCooldown;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------


    	// ---------------------------------------- Public Properties --------------------------------------------------

	    /// <summary>
	    /// Performs a light attack
	    /// </summary>
	    public void PerformLightAttack()
	    {
		    if(!CanAttack) return;

		    _isAttacking = true;
		    _lastAttackTime = Time.time;

		    PerformAttack(1f);
		    
		    // [END ATTACK]
		    EndAttack(attackDuration).Forget();
		    
		    // [DEBUG]
		    if (showDebugInfo) DebugSystem.Custom("Light Attack Initiated", Color.crimson);
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------

	    private void PerformAttack(float damage)
	    {
		    // [Trigger attack animation]
		    if(PlayerController.Instance) PlayerController.Instance.GetPlayerAnimationHandler.LightAttackAnimation();
		    
		    // [Detect enemies in range]
		    
	    }

	    private async UniTask EndAttack(float duration)
	    {
		    await UniTask.Delay(TimeSpan.FromSeconds(duration));
		    _isAttacking = false;
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}