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
}
