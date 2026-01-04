using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "TopDownRPG/Player/PlayerStatData")]
public class PlayerStatData : ScriptableObject
{
	[Title("Base Stats")]
	public float baseHealth = 100f;
	public float baseStamina = 100f;
	public float baseMana = 100f;
	public float baseDamage = 10f;
	public float baseDefense = 5f;
	public float baseAttackSpeed = 1f;
	public float baseMoveSpeed = 5f;
	
	[Title("Stat Growth Per Level")]
	public float healthPerLevel = 10f;
	public float staminaPerLevel = 5f;
	public float manaPerLevel = 10f;
	public float damagePerLevel = 2f;
	public float defensePerLevel = 1f;
        
	[Title("Resource Regeneration")]
	public float healthRegenPerSecond = 1f;
	public float staminaRegenPerSecond = 10f;
	public float manaRegenPerSecond = 5f;
        
	[Title("Combat")]
	public float criticalChance = 0.1f; // 10%
	public float criticalDamageMultiplier = 2f;
	public float attackRange = 2f;
	
	[Title("Progression")]
	public int maxLevel = 50;
	public AnimationCurve experienceCurve;
	
	/// <summary>
	/// Calculates stat value at a specific level
	/// </summary>
	public float GetStatAtLevel(PlayerStat stat, int level)
	{
		level = Mathf.Clamp(level, 1, maxLevel);
            
		switch (stat)
		{
			case PlayerStat.Health:
				return baseHealth + (healthPerLevel * (level - 1));
			case PlayerStat.Stamina:
				return baseStamina + (staminaPerLevel * (level - 1));
			case PlayerStat.Mana:
				return baseMana + (manaPerLevel * (level - 1));
			case PlayerStat.Damage:
				return baseDamage + (damagePerLevel * (level - 1));
			case PlayerStat.Defense:
				return baseDefense + (defensePerLevel * (level - 1));
			default:
				return 0f;
		}
	}
	
	/// <summary>
	/// Gets experience required for a specific level
	/// </summary>
	public int GetExperienceForLevel(int level)
	{
		if (experienceCurve == null || experienceCurve.length == 0)
		{
			// Default linear progression
			return level * 100;
		}
            
		float normalizedLevel = (float)level / maxLevel;
		return Mathf.RoundToInt(experienceCurve.Evaluate(normalizedLevel) * 10000);
	}
	
}

[System.Serializable]
public class AbilityData
{
	public string abilityName;
	public Sprite icon;
	public float cooldown;
	public float manaCost;
	public float damage;
	public float range;
	[TextArea(2, 4)]
	public string description;
}
[System.Serializable]
public enum PlayerClass
{
	Warrior,
	Mage,
	Rogue,
	Ranger,
	Tank,
	Support
}
[System.Serializable]
public enum PlayerStat
{
	Health,
	Stamina,
	Mana,
	Damage,
	Defense,
	AttackSpeed,
	MoveSpeed
}
