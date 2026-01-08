using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Enemy detection system with vision and hearing
	/// Manages awareness levels and target detection
	/// </summary>
    public class EnemyDetectionSystem : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("References")]
	    [ReadOnly] [field: SerializeField] private EnemyBase enemyBase;
	    [field: SerializeField] private EnemyData enemyData;
	    [field: SerializeField] private Transform eyePosition;
	    
	    [Title("Detection Settings")]
	    [field: SerializeField] private LayerMask targetLayer;
	    [field: SerializeField] private LayerMask obstacleMask;
	    [field: SerializeField] private float detectionUpdateRate = 0.2f;
	    
	    [Title("Awareness")]
	    [field: SerializeField] [Range(0f, 1f)] private float awarenessLevel = 0f;
	    [field: SerializeField] private float awarenessIncreaseRate = 0.5f;
	    [field: SerializeField] private float awarenessDecreaseRate = 0.2f;
	    [field: SerializeField] private float awarenessThreshold = 0.7f;
        
	    [Title("Debug")]
	    [field: SerializeField] private bool showDebugInfo = false;
	    [field: SerializeField] private bool showGizmos = true;
	    
	    private Transform _detectedTarget;
	    private float _lastDetectionTime;
	    private bool _hasLineOfSight;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        if (enemyBase == null) enemyBase = GetComponent<EnemyBase>();
	        if (enemyData == null && enemyBase != null) enemyData = enemyBase?.EnemyData;
	        if (eyePosition == null) eyePosition = transform.GetChild(0).transform;
        }

        private void Update()
        {
	        if (Time.time >= _lastDetectionTime + detectionUpdateRate)
	        {
		        _lastDetectionTime = Time.time;
		        PerformDetection();
	        }

	        UpdateAwareness();
        }
        // ---------------------------------------- Public Properties --------------------------------------------------
        /// <summary>
        /// Manually alert the enemy to a target
        /// </summary>
        public void AlertToTarget(Transform target)
        {
	        _detectedTarget = target;
	        awarenessLevel = 1f;
            
	        if (enemyBase != null)
	        {
		        enemyBase.SetTarget(target);
	        }

	        if (showDebugInfo)
	        {
		        Debug.Log($"[EnemyDetectionSystem] Alerted to target: {target.name}");
	        }
        }

        /// <summary>
        /// Resets detection and awareness
        /// </summary>
        public void ResetDetection()
        {
	        _detectedTarget = null;
	        awarenessLevel = 0f;
	        _hasLineOfSight = false;

	        if (enemyBase != null)
	        {
		        enemyBase.ClearTarget();
	        }
        }

    	// ---------------------------------------- Private Properties -------------------------------------------------
	    private void PerformDetection()
	    {
		    if (enemyData == null) return;

		    // Vision detection
		    bool visionDetected = CheckVision();
            
		    // Hearing detection
		    bool hearingDetected = CheckHearing();

		    // Update detected target
		    if (visionDetected || hearingDetected)
		    {
			    if (_detectedTarget != null && enemyBase != null)
			    {
				    enemyBase.SetTarget(_detectedTarget);
                    
				    // Increase awareness
				    awarenessLevel = Mathf.Min(1f, awarenessLevel + awarenessIncreaseRate * detectionUpdateRate);
			    }
		    }
		    else
		    {
			    // Decrease awareness
			    awarenessLevel = Mathf.Max(0f, awarenessLevel - awarenessDecreaseRate * detectionUpdateRate);

			    // Clear target if awareness drops below threshold
			    if (awarenessLevel < awarenessThreshold * 0.5f && enemyBase != null)
			    {
				    enemyBase.ClearTarget();
				    _detectedTarget = null;
			    }
		    }
	    }

	    private bool CheckVision()
	    {
		    Collider[] targetsInRange = Physics.OverlapSphere(eyePosition.position, 
			    enemyData.visionRange, targetLayer);

		    foreach (var targetCollider in targetsInRange)
		    {
			    Transform target = targetCollider.transform;
			    Vector3 directionToTarget = (target.position - eyePosition.position).normalized;

			    // Check if within vision cone
			    float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
			    if (angleToTarget > enemyData.visionAngle / 2f)
				    continue;

			    // Check line of sight
			    float distanceToTarget = Vector3.Distance(eyePosition.position, target.position);
			    if (Physics.Raycast(eyePosition.position, directionToTarget, distanceToTarget, obstacleMask))
			    {
				    _hasLineOfSight = false;
				    continue;
			    }

			    // Target detected
			    _hasLineOfSight = true;
			    _detectedTarget = target;

			    if (showDebugInfo)
			    {
				    Debug.Log($"[EnemyDetectionSystem] Vision detected: {target.name}");
			    }

			    return true;
		    }

		    _hasLineOfSight = false;
		    return false;
	    }

	    private bool CheckHearing()
	    {
		    Collider[] targetsInRange = Physics.OverlapSphere(transform.position, 
			    enemyData.hearingRange, targetLayer);

		    foreach (var targetCollider in targetsInRange)
		    {
			    // Check if target is making noise (moving)
			    // For now, just detect any target in hearing range
			    _detectedTarget = targetCollider.transform;

			    if (showDebugInfo)
			    {
				    Debug.Log($"[EnemyDetectionSystem] Hearing detected: {targetCollider.name}");
			    }

			    return true;
		    }

		    return false;
	    }

	    private void UpdateAwareness()
	    {
		    // Awareness affects detection sensitivity
		    // Higher awareness = easier to detect
	    }

    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    public Transform DetectedTarget => _detectedTarget;
	    public float AwarenessLevel => awarenessLevel;
	    public bool HasLineOfSight => _hasLineOfSight;
	    
	    
	    
	    // ----------------------------------------------- Debug -------------------------------------------------------
	    private void OnDrawGizmosSelected()
	    {
		    if (!showGizmos || enemyData == null) return;

		    // Draw vision range
		    Gizmos.color = _hasLineOfSight ? Color.red : Color.yellow;
		    Gizmos.DrawWireSphere(eyePosition != null ? eyePosition.position : transform.position, 
			    enemyData.visionRange);

		    // Draw vision cone
		    Vector3 forward = transform.forward * enemyData.visionRange;
		    Vector3 leftBoundary = Quaternion.Euler(0, -enemyData.visionAngle / 2f, 0) * forward;
		    Vector3 rightBoundary = Quaternion.Euler(0, enemyData.visionAngle / 2f, 0) * forward;

		    Gizmos.color = Color.yellow;
		    Gizmos.DrawRay(eyePosition != null ? eyePosition.position : transform.position, leftBoundary);
		    Gizmos.DrawRay(eyePosition != null ? eyePosition.position : transform.position, rightBoundary);

		    // Draw hearing range
		    Gizmos.color = Color.cyan;
		    Gizmos.DrawWireSphere(transform.position, enemyData.hearingRange);

		    // Draw line to detected target
		    if (_detectedTarget != null)
		    {
			    Gizmos.color = Color.red;
			    Gizmos.DrawLine(eyePosition != null ? eyePosition.position : transform.position, 
				    _detectedTarget.position);
		    }
	    }

    }
}