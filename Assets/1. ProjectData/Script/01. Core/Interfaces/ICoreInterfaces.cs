using UnityEngine;

namespace FXnRXn
{
	/// <summary>
	/// Interface for objects that can take damage
	/// </summary>
    public interface IDamageable
    {
	    float CurrentHealth { get; }
	    float MaxHealth { get; }
	    bool IsDead { get; }

	    void TakeDamage(MonoBehaviour damager, Vector3 direction, float damage, Vector3 hitPoint, Vector3 hitNormal);
	    void Heal(float amount);

	    void Stunned();
	    void Die();

    }
}