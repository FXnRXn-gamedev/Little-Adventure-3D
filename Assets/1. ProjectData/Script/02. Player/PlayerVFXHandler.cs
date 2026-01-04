using TriInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace FXnRXn
{
    public class PlayerVFXHandler : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    [Title("VFX Refference")] 
	    [field: SerializeField] private VisualEffect footStepVFX;
	    [field: SerializeField] private VisualEffect healVFX;
	    [field: SerializeField] private ParticleSystem weaponSlashVFX_RL;
	    [field: SerializeField] private ParticleSystem weaponSlashVFX_LR;
	    [field: SerializeField] private ParticleSystem weaponSlashVFX_Charged;


	    private bool _isFootStepPlaying = false;

	    // ---------------------------------------- Unity Callback -----------------------------------------------------
	    public void Init()
	    {
		    if(footStepVFX != null) footStepVFX.Stop();
		    if(healVFX != null) healVFX.Stop();
		    if(weaponSlashVFX_RL != null) weaponSlashVFX_RL.Stop();
		    if(weaponSlashVFX_LR != null) weaponSlashVFX_LR.Stop();
		    if(weaponSlashVFX_Charged != null) weaponSlashVFX_Charged.Stop();
	    }

	    // ---------------------------------------- Public Properties --------------------------------------------------

	    public void UpdateVFX()
	    {
		    if (_isFootStepPlaying ^ PlayerController.Instance.GetPlayerMovementController.IsMoving)
		    {
			    Update_FootStepVFX(PlayerController.Instance.GetPlayerMovementController.IsMoving);
			    _isFootStepPlaying = !_isFootStepPlaying;
		    }
		    
		    
	    }

	    public void PlayAttackVFX(string state)
	    {
		    switch (state)
		    {
			    case "RL":
				    if(weaponSlashVFX_RL != null) weaponSlashVFX_RL.Play();
				    break;
			    case "LR":
				    if(weaponSlashVFX_LR != null) weaponSlashVFX_LR.Play();
				    break;
			    case "Full":
				    if(weaponSlashVFX_Charged != null) weaponSlashVFX_Charged.Play();
				    break;
			    default:
				    if(weaponSlashVFX_RL != null) weaponSlashVFX_RL.Stop();
				    if(weaponSlashVFX_LR != null) weaponSlashVFX_LR.Stop();
				    if(weaponSlashVFX_Charged != null) weaponSlashVFX_Charged.Stop();
				    break;
		    }
		    
	    }


	    // ---------------------------------------- Private Properties -------------------------------------------------

	    private void Update_FootStepVFX(bool state)
	    {
		    if(footStepVFX == null) return;

		    if (state)
		    {
			    footStepVFX.Play();
		    }
		    else
		    {
			    footStepVFX.Stop();
		    }
		    
		    
		    
	    }


	    // ------------------------------------------ Helper Method ----------------------------------------------------

    }
}