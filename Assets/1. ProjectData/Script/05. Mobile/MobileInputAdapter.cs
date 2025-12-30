using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


namespace FXnRXn
{
	/// <summary>
	/// Mobile input adapter that integrates virtual controls with the player controller
	/// </summary>
    public class MobileInputAdapter : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    #region Singleton
	    public static MobileInputAdapter Instance { get; private set; }
	    #endregion
	    
	    [Title("Virtual Controls")]
	    [field: SerializeField] private VirtualJoystick movementJoystick;
	    [field: SerializeField] private Button rollButton;
	    [field: SerializeField] private Button dashButton;
	    [field: SerializeField] private Button attackButton;
	    [field: SerializeField] private Button[] abilityButtons;
        
	    [Title("Settings")]
	    [field: SerializeField] private bool enableMobileControls = true;
        
	    [Title("Debug")]
	    [field: SerializeField] private bool showDebugInfo = false;

  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        // Auto-detect platform
// #if UNITY_ANDROID || UNITY_IOS
// 	        enableMobileControls = true;
// #else
// 	        enableMobileControls = false;
// #endif
	        
	        if (Instance == null) Instance = this;
	        if (movementJoystick == null) movementJoystick = FindFirstObjectByType<VirtualJoystick>();

	        SetupButtons();
        }


    	// ---------------------------------------- Public Properties --------------------------------------------------


    	// ---------------------------------------- Private Properties -------------------------------------------------

	    private void SetupButtons()
	    {
		    if(rollButton != null) rollButton.onClick.AddListener(OnRollPressed);
	    }

	    private void OnRollPressed()
	    {
		    PlayerController.Instance.GetPlayerMovementController.RollStartInputPressed();
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    public Vector2 GetInputDirection() => movementJoystick.GetInputDirection();
	    public float GetInputMagnitude() => movementJoystick.GetInputMagnitude();
	    public bool EnableMobileControls() => enableMobileControls;

    }
}