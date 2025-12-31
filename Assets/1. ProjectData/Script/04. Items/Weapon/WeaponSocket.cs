using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FXnRXn
{
	/// <summary>
	/// Manages weapon attachment to player hand
	/// </summary>
    public class WeaponSocket : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Socket Configuration")]
	    [field: SerializeField] private Transform handTransform; // Reference to the hand bone
	    [field: SerializeField] private Vector3 positionOffset = Vector3.zero;
	    [field: SerializeField] private Vector3 rotationOffset = Vector3.zero;
	    
	    [Title("Current Weapon")]
	    [ReadOnly, SerializeField] private GameObject currentWeapon;
        
	    private Transform weaponTransform;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        private void LateUpdate()
        {
	        // LateUpdate ensures this runs after animation updates
	        if (weaponTransform != null && handTransform != null)
	        {
		        UpdateWeaponTransform();
	        }
        }


        // ---------------------------------------- Weapon Equip/Detach ------------------------------------------------
        /// <summary>
        /// Attach a weapon to this socket
        /// </summary>
        public void AttachWeapon(GameObject weapon)
        {
	        if (weapon == null) return;
            
	        // Detach current weapon first
	        DetachWeapon();
            
	        currentWeapon = weapon;
	        weaponTransform = weapon.transform;
            
	        // Parent to hand transform
	        weaponTransform.SetParent(handTransform);
            
	        // Apply offsets
	        UpdateWeaponTransform();
            
	        // Enable weapon mesh
	        SetWeaponVisibility(true);
        }

        /// <summary>
        /// Detach current weapon from socket
        /// </summary>
        public void DetachWeapon()
        {
	        if (currentWeapon == null) return;
            
	        weaponTransform.SetParent(null);
	        currentWeapon = null;
	        weaponTransform = null;
        }

        /// <summary>
        /// Set weapon visibility
        /// </summary>
        public void SetWeaponVisibility(bool visible)
        {
	        if (currentWeapon == null) return;
            
	        Renderer[] renderers = currentWeapon.GetComponentsInChildren<Renderer>();
	        foreach (var renderer in renderers)
	        {
		        renderer.enabled = visible;
	        }
        }
        
        
    	// ---------------------------------------- Weapon Socket Follow -----------------------------------------------
	    private void UpdateWeaponTransform()
	    {
		    // Apply position and rotation offsets
		    weaponTransform.localPosition = positionOffset;
		    weaponTransform.localRotation = Quaternion.Euler(rotationOffset);
	    }
        
	    /// <summary>
	    /// Set the hand transform reference (useful for runtime setup)
	    /// </summary>
	    public void SetHandTransform(Transform hand)
	    {
		    handTransform = hand;
	    }
        
	    /// <summary>
	    /// Set position offset for weapon placement
	    /// </summary>
	    public void SetPositionOffset(Vector3 offset)
	    {
		    positionOffset = offset;
		    if (weaponTransform != null)
			    UpdateWeaponTransform();
	    }
        
	    /// <summary>
	    /// Set rotation offset for weapon placement
	    /// </summary>
	    public void SetRotationOffset(Vector3 offset)
	    {
		    rotationOffset = offset;
		    if (weaponTransform != null)
			    UpdateWeaponTransform();
	    }


	    // ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    /// <summary>
	    /// Get current attached weapon
	    /// </summary>
	    public GameObject GetCurrentWeapon() => currentWeapon;

    }
}