using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FXnRXn
{
    public class PlayerStatSystem : MonoBehaviour, IDamageable
    {
	    // ------------------------------------------ Properties -------------------------------------------------------
	    
	    [Title("Hero Data")]
	    [field: SerializeField] private PlayerStatData playerStatData;
        
	    [Title("Current Level")]
	    [field: SerializeField] private int currentLevel = 1;
        
	    [Title("Debug")]
	    [field: SerializeField] private bool showDebugInfo = false;

	    // Current stats
	    private float _currentHealth;
	    private float _currentStamina;
	    private float _currentMana;
	    private float _maxHealth;
	    private float _maxStamina;
	    private float _maxMana;
	    
	    // Stat modifiers (from equipment, buffs, etc.)
	    private float _damageModifier = 1f;
	    private float _defenseModifier = 1f;
	    private float _attackSpeedModifier = 1f;
	    private float _moveSpeedModifier = 1f;


  	    // ---------------------------------------- Unity Callback -----------------------------------------------------
        public void Init()
        {
	        if (playerStatData == null) return;
	        InitializeStats();
        }

        public void UpdatePlayerStat()
        {
	        if (IsDead) return;
        }

        private void InitializeStats()
        {
	        _maxHealth = playerStatData.GetStatAtLevel(PlayerStat.Health, currentLevel);
	        _maxStamina = playerStatData.GetStatAtLevel(PlayerStat.Stamina, currentLevel);
	        _maxMana = playerStatData.GetStatAtLevel(PlayerStat.Mana, currentLevel);

	        _currentHealth = _maxHealth;
	        _currentStamina = _maxStamina;
	        _currentMana = _maxMana;

	        IsDead = false;
        }


        // ---------------------------------------- Public Properties --------------------------------------------------


    	// ------------------------------------- IDamageable Implementation --------------------------------------------
	    public void TakeDamage(MonoBehaviour damager, Vector3 direction, float damage, Vector3 hitPoint, Vector3 hitNormal)
	    {
		    throw new NotImplementedException();
	    }

	    public void Heal(float amount)
	    {
		    throw new NotImplementedException();
	    }

	    public void Stunned()
	    {
		    throw new NotImplementedException();
	    }

	    public void Die()
	    {
		    throw new NotImplementedException();
	    }


    	// ------------------------------------------ Helper Method ----------------------------------------------------
	    
	    // Events
	    public event Action<float, float> OnHealthChanged; // current, max
	    public event Action<float, float> OnStaminaChanged;
	    public event Action<float, float> OnManaChanged;
	    public event Action OnDeath;
	    public event Action<int> OnLevelUp;

	    // Properties
	    public float CurrentHealth => _currentHealth;
	    public float MaxHealth => _maxHealth;
	    public float CurrentStamina => _currentStamina;
	    public float MaxStamina => _maxStamina;
	    public float CurrentMana => _currentMana;
	    public float MaxMana => _maxMana;
	    public bool IsDead { get; private set; }
	    

	    public int CurrentLevel => currentLevel;
	    public PlayerStatData PlayerStats => playerStatData;

	    // Calculated stats
	    public float TotalDamage => playerStatData.GetStatAtLevel(PlayerStat.Damage, currentLevel) * _damageModifier;
	    public float TotalDefense => playerStatData.GetStatAtLevel(PlayerStat.Defense, currentLevel) * _defenseModifier;
	    public float AttackSpeed => playerStatData.baseAttackSpeed * _attackSpeedModifier;
	    public float MoveSpeed => playerStatData.baseMoveSpeed * _moveSpeedModifier;

    }
}