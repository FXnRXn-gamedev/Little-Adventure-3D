using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace FXnRXn
{
    public class KeyboardInputHandler : MonoBehaviour
    {
	    // ------------------------------------------- Singleton -------------------------------------------------------
	    
	    public static KeyboardInputHandler Instance { get; private set; }


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        private void Awake()
        {
	        if (Instance == null) Instance = this;
        }
		
    	// ---------------------------------------- Public Properties --------------------------------------------------

	    public Vector2 OnKeyboardUpdateMove()
	    {
		    float horizontal = 0f;
		    float vertical = 0f;

		    if (Keyboard.current.aKey.isPressed) horizontal -= 1f;
		    if (Keyboard.current.dKey.isPressed) horizontal += 1f;
		    if (Keyboard.current.wKey.isPressed) vertical += 1f;
		    if (Keyboard.current.sKey.isPressed) vertical -= 1f;

		    Vector2 input = new Vector2(horizontal, vertical);
		    return input;
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}