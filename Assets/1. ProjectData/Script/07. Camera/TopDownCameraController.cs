using System;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Top-down angled follow camera with smoothing and obstacle avoidance
	/// </summary>
    public class TopDownCameraController : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    public static TopDownCameraController Instance { get; private set; }
	    
	    [TriInspector.Title("Refference")]
	    [field: SerializeField] private Camera					mainCamera;
	    
	    [TriInspector.Title("Camera Settings")]
	    [field: SerializeField] private Transform				playerTarget;
	    

	    [Space(10)] 
	    public bool												isFollowPlayer;
	    [field: SerializeField] private bool					invertCamera;
	    [field: SerializeField] private float					mouseSensitivity			= 5f;
	    [field: SerializeField] private float					cameraDistance;
	    [field: SerializeField] private float					cameraHeightOffset;
	    [field: SerializeField] private float					cameraHorizontalOffset;
	    [field: SerializeField] private Vector3					cameraTiltOffset;
	    
	    private Vector2											cameraTiltBounds			= new Vector2(-10f, 45f);
	    private float											positionalCameraLag			= 2f;
	    private float											rotationalCameraLag			= 2f;
	    private float											cameraInversion;
	    private float											lastAngleX;
	    private float											lastAngleY;
	    private Vector3											lastPosition;
	    private float											newAngleX;
	    private float											newAngleY;
	    private Vector3											newPosition;
	    private float											rotationX;
	    private float											rotationY;
	    private Transform										gameCamera;
        
        
        
	    private const int										LAG_DELTA_TIME_ADJUSTMENT = 20;
	    
	    

  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        private void Awake()
        {
	        if (Instance == null) Instance = this;
	        if(mainCamera == null) mainCamera =GetComponentInChildren<Camera>();
	        if(gameCamera == null) gameCamera = mainCamera.transform;
        }
		
        private void Start()
        {
	        cameraInversion = invertCamera ? 1 : -1;
	        playerTarget = PlayerController.Instance.transform;
			
	        transform.position = playerTarget.position;
	        transform.rotation = playerTarget.rotation;
	        lastPosition = transform.position;
			
	        gameCamera.localPosition = new Vector3(cameraHorizontalOffset, cameraHeightOffset, cameraDistance * -1);
	        gameCamera.localEulerAngles = cameraTiltOffset;
	        
        }
        private void LateUpdate()
        {
	        if (isFollowPlayer)
	        {
		        HandleCameraFollowPlayer();
	        }
			
        }

        // ---------------------------------------- Public Properties --------------------------------------------------


    	// ---------------------------------------- Private Properties -------------------------------------------------
	    
	    private void HandleCameraFollowPlayer()
	    {
		    if(playerTarget ==null) return;
			
		    float positionalFollowSpeed = 1 / (positionalCameraLag / LAG_DELTA_TIME_ADJUSTMENT);
		    float rotationalFollowSpeed = 1 / (rotationalCameraLag / LAG_DELTA_TIME_ADJUSTMENT);
			
		    newAngleX += rotationX;
		    newAngleX = Mathf.Clamp(newAngleX, cameraTiltBounds.x, cameraTiltBounds.y);
		    newAngleX = Mathf.Lerp(lastAngleX, newAngleX, rotationalFollowSpeed * Time.deltaTime);

		    newAngleY += rotationY;
		    newAngleY = Mathf.Lerp(lastAngleY, newAngleY, rotationalFollowSpeed * Time.deltaTime);

		    newPosition = playerTarget.position;
		    newPosition = Vector3.Lerp(lastPosition, newPosition, positionalFollowSpeed * Time.deltaTime);

		    transform.position = newPosition;
		    transform.eulerAngles = new Vector3(newAngleX, newAngleY, 0);
			
		    // Calculate target camera local position
		    Vector3 targetLocalPosition = new Vector3(cameraHorizontalOffset, cameraHeightOffset, cameraDistance * -1);
		    // Calculate target camera local Euler angles
		    Vector3 targetLocalEulerAngles = cameraTiltOffset;

		    gameCamera.localPosition = new Vector3(cameraHorizontalOffset, cameraHeightOffset, cameraDistance * -1);
		    gameCamera.localEulerAngles = cameraTiltOffset;

		    lastPosition = newPosition;
		    lastAngleX = newAngleX;
		    lastAngleY = newAngleY;
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    public Camera GetMainCamera => mainCamera;
	    public Transform GetMainCameraTransform => mainCamera.transform;
	    public Vector3 GetMainCameraForward => GetMainCameraTransform.forward;
	    public Vector3 GetMainCameraRight => GetMainCameraTransform.right;
	    public Vector3 GetMainCameraForwardNormalize => GetMainCameraTransform.forward.normalized;
	    public Vector3 GetMainCameraRightNormalize => GetMainCameraTransform.right.normalized;

    }
}