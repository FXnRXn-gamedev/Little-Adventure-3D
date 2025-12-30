using System;
using UnityEngine;

namespace FXnRXn
{
    public class EnemyBase : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------

	    protected float _lastAttackTime = 0f;
	    protected Transform _currentTarget;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        protected virtual void Update()
        {
	        if (CanAttack())
	        {
		        TryAttack();
		        _lastAttackTime = Time.time;
	        }
        }


        // ---------------------------------------- Public Properties --------------------------------------------------


    	// -------------------------------------------- Combat ---------------------------------------------------------
	    
	    protected virtual bool CanAttack()
	    {
		    float distanceToTarget = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
		    if (distanceToTarget > 2f) return false;
		    
		    float timeSinceLastAttack = Time.time - _lastAttackTime;
		    return timeSinceLastAttack >= 1.5f;
	    }

	    protected virtual void TryAttack()
	    {
		    IDamageable targetDamageable = PlayerController.Instance.GetComponent<IDamageable>();
		    if(targetDamageable == null) return;
		    
		    Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
		    targetDamageable.TakeDamage(this, direction, 5f, PlayerController.Instance.transform.position, Vector3.up);
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    public Transform CurrentTarget => _currentTarget;
	    public void SetCurrentTarget(Transform target) => _currentTarget = target;
	    
	    
	    // --------------------------------------------- Debug ---------------------------------------------------------

	    private void OnDrawGizmosSelected()
	    {
		    // Draw attack range
		    Gizmos.color = Color.red;
		    Gizmos.DrawWireSphere(transform.position, 2f);
	    }
    }
}