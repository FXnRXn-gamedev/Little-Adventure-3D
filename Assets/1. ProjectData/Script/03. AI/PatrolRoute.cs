using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Manages enemy patrol routes with waypoints
	/// Supports looping and ping-pong patrol patterns
	/// </summary>
    public class PatrolRoute : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [TitleGroup("Waypoints")]
	    [SerializeField] private List<Transform> waypoints = new List<Transform>();
	    [SerializeField] private bool autoGenerateWaypoints = false;
	    [SerializeField] private float waypointRadius = 0.5f;
	    
	    [TitleGroup("Patrol Settings")]
	    [SerializeField] private PatrolMode patrolMode = PatrolMode.Loop;
	    [SerializeField] private float waitTimeAtWaypoint = 2f;
	    [SerializeField] private bool randomizeWaitTime = false;
	    [SerializeField] private float minWaitTime = 1f;
	    [SerializeField] private float maxWaitTime = 3f;
	    
	    [Title("Debug")]
	    [SerializeField] private bool showGizmos = true;
	    [SerializeField] private Color waypointColor = Color.yellow;
	    [SerializeField] private Color pathColor = Color.green;
	    
	    private int _currentWaypointIndex = 0;
	    private bool _isReversing = false;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        if (autoGenerateWaypoints && waypoints.Count == 0)
	        {
		        GenerateWaypointsFromChildren();
	        }
        }


    	// ---------------------------------------- Public Properties --------------------------------------------------
	    /// <summary>
	    /// Adds a waypoint at runtime
	    /// </summary>
	    public void AddWaypoint(Transform waypoint)
	    {
		    if (!waypoints.Contains(waypoint))
		    {
			    waypoints.Add(waypoint);
		    }
	    }

	    /// <summary>
	    /// Removes a waypoint
	    /// </summary>
	    public void RemoveWaypoint(Transform waypoint)
	    {
		    waypoints.Remove(waypoint);
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------
	    private void AdvanceWaypoint()
	    {
		    switch (patrolMode)
		    {
			    case PatrolMode.Loop:
				    _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
				    break;

			    case PatrolMode.PingPong:
				    if (_isReversing)
				    {
					    _currentWaypointIndex--;
					    if (_currentWaypointIndex <= 0)
					    {
						    _currentWaypointIndex = 0;
						    _isReversing = false;
					    }
				    }
				    else
				    {
					    _currentWaypointIndex++;
					    if (_currentWaypointIndex >= waypoints.Count - 1)
					    {
						    _currentWaypointIndex = waypoints.Count - 1;
						    _isReversing = true;
					    }
				    }
				    break;

			    case PatrolMode.Once:
				    if (_currentWaypointIndex < waypoints.Count - 1)
				    {
					    _currentWaypointIndex++;
				    }
				    break;

			    case PatrolMode.Random:
				    int previousIndex = _currentWaypointIndex;
				    do
				    {
					    _currentWaypointIndex = Random.Range(0, waypoints.Count);
				    }
				    while (_currentWaypointIndex == previousIndex && waypoints.Count > 1);
				    break;
		    }
	    }
	    
	    private void GenerateWaypointsFromChildren()
	    {
		    waypoints.Clear();
		    foreach (Transform child in transform)
		    {
			    waypoints.Add(child);
		    }
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    /// <summary>
	    /// Gets the current waypoint
	    /// </summary>
	    public Transform GetCurrentWaypoint()
	    {
		    if (waypoints.Count == 0) return null;
		    return waypoints[_currentWaypointIndex];
	    }

	    /// <summary>
	    /// Gets the next waypoint and advances the index
	    /// </summary>
	    public Transform GetNextWaypoint()
	    {
		    if (waypoints.Count == 0) return null;

		    Transform current = waypoints[_currentWaypointIndex];
		    AdvanceWaypoint();
		    return current;
	    }

	    /// <summary>
	    /// Gets a specific waypoint by index
	    /// </summary>
	    public Transform GetWaypoint(int index)
	    {
		    if (index < 0 || index >= waypoints.Count) return null;
		    return waypoints[index];
	    }

	    /// <summary>
	    /// Gets the total number of waypoints
	    /// </summary>
	    public int GetWaypointCount()
	    {
		    return waypoints.Count;
	    }

	    /// <summary>
	    /// Gets the wait time for the current waypoint
	    /// </summary>
	    public float GetWaitTime()
	    {
		    if (randomizeWaitTime)
		    {
			    return Random.Range(minWaitTime, maxWaitTime);
		    }
		    return waitTimeAtWaypoint;
	    }

	    /// <summary>
	    /// Checks if a position is close to the current waypoint
	    /// </summary>
	    public bool IsAtWaypoint(Vector3 position)
	    {
		    Transform waypoint = GetCurrentWaypoint();
		    if (waypoint == null) return false;

		    return Vector3.Distance(position, waypoint.position) <= waypointRadius;
	    }

	    /// <summary>
	    /// Resets to the first waypoint
	    /// </summary>
	    public void ResetToStart()
	    {
		    _currentWaypointIndex = 0;
		    _isReversing = false;
	    }
	    
	    
	    
	    
	    // ---------------------------------------------- DEBUG --------------------------------------------------------
	    private void OnDrawGizmosSelected()
	    {
		    if (!showGizmos || waypoints.Count == 0) return;

		    // Draw waypoints
		    foreach (var waypoint in waypoints)
		    {
			    if (waypoint != null)
			    {
				    Gizmos.color = waypointColor;
				    Gizmos.DrawWireSphere(waypoint.position, waypointRadius);
			    }
		    }

		    // Draw path
		    Gizmos.color = pathColor;
		    for (int i = 0; i < waypoints.Count - 1; i++)
		    {
			    if (waypoints[i] != null && waypoints[i + 1] != null)
			    {
				    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
			    }
		    }

		    // Draw loop connection
		    if (patrolMode == PatrolMode.Loop && waypoints.Count > 1)
		    {
			    if (waypoints[waypoints.Count - 1] != null && waypoints[0] != null)
			    {
				    Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
			    }
		    }
	    }

    }
	
	public enum PatrolMode
	{
		Loop,       // Continuous loop through waypoints
		PingPong,   // Back and forth
		Once,       // One-time patrol then stop
		Random      // Random waypoint selection
	}
}