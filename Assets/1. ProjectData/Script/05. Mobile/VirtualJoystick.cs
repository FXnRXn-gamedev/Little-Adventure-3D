using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Virtual joystick for mobile touch controls
	/// </summary>
	
    public class VirtualJoystick : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Joystick Settings")]
	    [field: SerializeField] private RectTransform joystickBackground;
	    [field: SerializeField] private RectTransform joystickHandle;
	    [field: SerializeField] private RectTransform joystickArrow;
	    [field: SerializeField] private float handleRange = 50f;
	    [field: SerializeField] private float deadZone = 0.1f;
	    
	    [Title("Behavior")]
	    [field: SerializeField] private bool dynamicJoystick = true;
	    [field: SerializeField] private bool hideOnRelease = true;
	    [field: SerializeField] private bool showArrow = true;
        
	    [Title("Debug")]
	    [field: SerializeField] private bool showDebugInfo = false;

	    private Vector2				_input = Vector2.zero;
	    private Vector2				_joystickPosition = Vector2.zero;
	    private int					_touchId = -1;
	    private bool				_isActive = false;
	    private Canvas				_canvas;
	    private CanvasGroup			_canvasGroup;
	    


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        _canvas = GetComponentInParent<Canvas>();
	        _canvasGroup = GetComponent<CanvasGroup>();
	        
	        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
	        if (hideOnRelease) _canvasGroup.alpha = 0f;
	        if(joystickArrow != null &&  showArrow) joystickArrow.gameObject.SetActive(false);
        }

        private void Update()
        {
	        HandleInput();
        }
        
        
        // ---------------------------------------- Public Properties --------------------------------------------------
        private void HandleInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
	        // Mouse input for testing in editor
	        if (UnityEngine.Input.GetMouseButtonDown(0))
	        {
		        OnPointerDown(UnityEngine.Input.mousePosition);
	        }
	        else if (UnityEngine.Input.GetMouseButton(0) && _isActive)
	        {
		        OnDrag(UnityEngine.Input.mousePosition);
	        }
	        else if (UnityEngine.Input.GetMouseButtonUp(0))
	        {
		        OnPointerUp();
	        }
#else
			// Touch input for mobile
            if (UnityEngine.Input.touchCount > 0)
            {
                for (int i = 0; i < UnityEngine.Input.touchCount; i++)
                {
                    Touch touch = UnityEngine.Input.GetTouch(i);

                    if (touch.phase == TouchPhase.Began && _touchId == -1)
                    {
                        OnPointerDown(touch.position);
                        _touchId = touch.fingerId;
                    }
                    else if (touch.fingerId == _touchId)
                    {
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            OnDrag(touch.position);
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            OnPointerUp();
                            _touchId = -1;
                        }
                    }
                }
            }
#endif
        }

    	// ---------------------------------------- Private Properties -------------------------------------------------
	    
	    private void OnPointerDown(Vector2 position)
	    {
		    // Check if touch is within joystick bounds
		    if (!IsTouchWithinJoystick(position))
		    {
			    return; // Don't activate if touch is outside joystick area
		    }
		    _isActive = true;

		    if (dynamicJoystick)
		    {
			    // Position joystick at touch location
			    RectTransformUtility.ScreenPointToLocalPointInRectangle(
				    _canvas.transform as RectTransform,
				    position,
				    _canvas.worldCamera,
				    out _joystickPosition);

			    joystickBackground.anchoredPosition = _joystickPosition;
		    }

		    if (hideOnRelease && _canvasGroup != null) _canvasGroup.alpha = 1f;
		    if(joystickArrow != null && showArrow) joystickArrow.gameObject.SetActive(true);
		    
	    }

	    private void OnDrag(Vector2 position)
	    {
		    if (!_isActive) return;

		    Vector2 localPoint;
		    RectTransformUtility.ScreenPointToLocalPointInRectangle(
			    joystickBackground,
			    position,
			    _canvas.worldCamera,
			    out localPoint);

		    // Calculate input
		    Vector2 offset = localPoint;
		    Vector2 direction = offset.magnitude > handleRange 
			    ? offset.normalized 
			    : offset / handleRange;

		    // Apply dead zone
		    if (direction.magnitude < deadZone)
		    {
			    direction = Vector2.zero;
		    }

		    _input = direction;

		    // Update handle position
		    joystickHandle.anchoredPosition = direction * handleRange;
		    
		    // Update arrow rotation to point in the direction of input
		    if (joystickArrow != null && showArrow && direction.magnitude > 0.01f)
		    {
			    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			    joystickArrow.rotation = Quaternion.Euler(0f, 0f, angle);
		    }

		    if (showDebugInfo)
		    {
			    DebugSystem.Info($"[VirtualJoystick] Input: {_input}");
		    }
	    }
	    
	    private void OnPointerUp()
	    {
		    _isActive = false;
		    _input = Vector2.zero;
		    joystickHandle.anchoredPosition = Vector2.zero;

		    if (hideOnRelease && _canvasGroup != null) _canvasGroup.alpha = 0f;
		    if(joystickArrow != null && showArrow) joystickArrow.gameObject.SetActive(false);
	    }
	    
	    /// <summary>
	    /// Checks if the touch position is within the joystick background area
	    /// </summary>
	    private bool IsTouchWithinJoystick(Vector2 screenPosition)
	    {
		    // Convert screen position to local point in joystick background
		    Vector2 localPoint;
		    RectTransformUtility.ScreenPointToLocalPointInRectangle(
			    joystickBackground,
			    screenPosition,
			    _canvas.worldCamera,
			    out localPoint);

		    // Check if the local point is within the rect bounds
		    return joystickBackground.rect.Contains(localPoint);
	    }

    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    /// <summary>
	    /// Gets the normalized input direction
	    /// </summary>
	    public Vector2 GetInputDirection() => _input;
	    
	    /// <summary>
	    /// Gets the input magnitude
	    /// </summary>
	    public float GetInputMagnitude() => _input.magnitude;

    }
}