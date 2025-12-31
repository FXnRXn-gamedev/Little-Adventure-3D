using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Manages all weapon sockets on the player
	/// </summary>
    public class WeaponManager : MonoBehaviour
    {
	    public enum WeaponSlot
	    {
		    RightHand,
		    LeftHand,
		    Back,
		    Hip
	    }
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Socket References")]
	    [SerializeField] private WeaponSocket rightHandSocket;
	    [SerializeField] private WeaponSocket leftHandSocket;
	    [SerializeField] private WeaponSocket backSocket; // For sheathed weapons
        
	    [Title("Auto-Setup")]
	    [SerializeField] private GameObject playerSkeleton;
	    [SerializeField] private string rightHandBoneName = "RightHand";
	    [SerializeField] private string leftHandBoneName = "LeftHand";

	    
	    
	    private Dictionary<WeaponSlot, WeaponSocket> socketMap;
	    
	    

  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        public void Init()
        {
	        InitializeSockets();
	        SetupHandTransforms();
	        GenerateWeapon();
        }	


    	// ---------------------------------------- Public Properties --------------------------------------------------

	    public async UniTask GenerateWeapon()
	    {
		    if(Resources.Load<GameObject>("Weapons/Sword") == null) return;

		    await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
		    // Equip weapon to right hand
		    GameObject swordPrefab = Resources.Load<GameObject>("Weapons/Sword");
		    GameObject sword = Instantiate(swordPrefab, transform);
		    
		    EquipWeapon(sword, WeaponSlot.RightHand);
		    //TransferWeapon(WeaponSlot.Back, WeaponSlot.RightHand);
		    //TransferWeapon(WeaponSlot.RightHand, WeaponSlot.Back);
	    }
	    
	    
	    /// <summary>
	    /// Equip weapon to specified slot
	    /// </summary>
	    public void EquipWeapon(GameObject weapon, WeaponSlot slot)
	    {
		    if (socketMap.TryGetValue(slot, out WeaponSocket socket))
		    {
			    socket.AttachWeapon(weapon);
		    }

		    //SetWeaponVisibility(slot, true);
	    }
        
	    /// <summary>
	    /// Unequip weapon from specified slot
	    /// </summary>
	    public void UnequipWeapon(WeaponSlot slot)
	    {
		    if (socketMap.TryGetValue(slot, out WeaponSocket socket))
		    {
			    socket.DetachWeapon();
		    }
		    SetWeaponVisibility(slot, false);
	    }
        
	    /// <summary>
	    /// Transfer weapon between slots (e.g., from back to hand)
	    /// </summary>
	    public void TransferWeapon(WeaponSlot fromSlot, WeaponSlot toSlot)
	    {
		    if (socketMap.TryGetValue(fromSlot, out WeaponSocket fromSocket) &&
		        socketMap.TryGetValue(toSlot, out WeaponSocket toSocket))
		    {
			    GameObject weapon = fromSocket.GetCurrentWeapon();
			    if (weapon != null)
			    {
				    fromSocket.DetachWeapon();
				    toSocket.AttachWeapon(weapon);
			    }
		    }
	    }
        
	    /// <summary>
	    /// Get weapon in specified slot
	    /// </summary>
	    public GameObject GetWeaponInSlot(WeaponSlot slot)
	    {
		    if (socketMap.TryGetValue(slot, out WeaponSocket socket))
		    {
			    return socket.GetCurrentWeapon();
		    }
		    return null;
	    }
        
	    /// <summary>
	    /// Set weapon visibility in slot
	    /// </summary>
	    public void SetWeaponVisibility(WeaponSlot slot, bool visible)
	    {
		    if (socketMap.TryGetValue(slot, out WeaponSocket socket))
		    {
			    socket.SetWeaponVisibility(visible);
		    }
	    }


    	// ---------------------------------------- Private Properties -------------------------------------------------
	    
	    private void InitializeSockets()
	    {
		    socketMap = new Dictionary<WeaponSlot, WeaponSocket>();
            
		    if (rightHandSocket != null)
			    socketMap[WeaponSlot.RightHand] = rightHandSocket;
            
		    if (leftHandSocket != null)
			    socketMap[WeaponSlot.LeftHand] = leftHandSocket;
            
		    if (backSocket != null)
			    socketMap[WeaponSlot.Back] = backSocket;
	    }

	    private void SetupHandTransforms()
	    {
		    if (playerSkeleton == null) return;
            
		    // Find hand bones in animator hierarchy
		    Transform[] bones = playerSkeleton.GetComponentsInChildren<Transform>();
            
		    foreach (Transform bone in bones)
		    {
			    if (bone.name.Contains(rightHandBoneName) && rightHandSocket != null)
			    {
				    rightHandSocket.SetHandTransform(bone);
			    }
			    else if (bone.name.Contains(leftHandBoneName) && leftHandSocket != null)
			    {
				    leftHandSocket.SetHandTransform(bone);
			    }
		    }
	    }
	    
    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    

    }
	
	
	[System.Serializable]
	public class WeaponData : ScriptableObject
	{
		public GameObject weaponPrefab;
		public Vector3 rightHandPositionOffset;
		public Vector3 rightHandRotationOffset;
		public Vector3 backPositionOffset;
		public Vector3 backRotationOffset;
	}
}