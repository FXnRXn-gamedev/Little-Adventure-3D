using UnityEngine;

/// <summary>
/// ScriptableObject defining enemy stats, behavior, and loot
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "TopDownRPG/AI/Enemy Data")]
public class EnemyData : ScriptableObject
{
	[Header("Basic Info")]
	public string enemyName;
	[TextArea(3, 5)]
	public string description;
	public Sprite icon;
	public GameObject enemyPrefab;
        
	[Header("Stats")]
	public float maxHealth = 100f;
	public float damage = 10f;
	public float defense = 5f;
	public float moveSpeed = 3f;
	public float attackSpeed = 1f;
        
	[Header("Combat")]
	public float attackRange = 2f;
	public float attackCooldown = 1.5f;
	public float criticalChance = 0.05f;
	public float criticalDamageMultiplier = 1.5f;
        
	[Header("Detection")]
	public float visionRange = 10f;
	public float visionAngle = 120f;
	public float hearingRange = 5f;
	public float aggroRange = 8f;
	public float deAggroRange = 15f;
        
	[Header("Behavior")]
	public EnemyType enemyType;
	public AggressionLevel aggressionLevel;
	public bool canFlee = false;
	public float fleeHealthThreshold = 0.2f; // 20% health
        
	[Header("Patrol")]
	public bool hasPatrolRoute = true;
	public float patrolSpeed = 2f;
	public float waitTimeAtWaypoint = 2f;
        
	[Header("Rewards")]
	public int experienceReward = 50;
	public int goldReward = 10;
	public LootTableReference lootTable;
        
	[Header("Special Abilities")]
	public EnemyAbility[] abilities;
}

[System.Serializable]
public class EnemyAbility
{
	public string abilityName;
	public float cooldown;
	public float damage;
	public float range;
	public GameObject abilityVFX;
}
    
[System.Serializable]
public class LootTableReference
{
	public string lootTableName;
	// Reference to actual loot table ScriptableObject
}
    
public enum EnemyType
{
	Melee,
	Ranged,
	Tank,
	Caster,
	Support,
	Boss
}
    
public enum AggressionLevel
{
	Passive,      // Won't attack unless attacked
	Defensive,    // Attacks when player gets close
	Aggressive,   // Actively seeks and attacks player
	VeryAggressive // Chases player relentlessly
}
