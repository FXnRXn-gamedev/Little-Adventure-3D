using UnityEngine;
using UnityEngine.InputSystem;


namespace FXnRXn
{
    public class KeyboardInputHandler : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

	    private void Update()
	    {
		    OnUpdateMove();
	    }
    	// ---------------------------------------- Public Properties --------------------------------------------------

	    public void OnUpdateMove()
	    {
		    
		    
		    float horizontal = 0f;
		    float vertical = 0f;

		    if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
		    if (Keyboard.current.dKey.isPressed) horizontal += 1f;
		    if (Keyboard.current.wKey.isPressed) vertical += 1f;
		    if (Keyboard.current.sKey.isPressed) vertical -= 1f;

		    Vector2 input = new Vector2(horizontal, vertical);
		    
		    
		    PlayerController.Instance.GetPlayerMovementController.SetMovementInput(input);

		    // Auto-run if joystick is pushed far enough
		    bool isRunning = input.magnitude > 0.7f;
		    PlayerController.Instance.GetPlayerMovementController.SetRunning(isRunning);
		    
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}