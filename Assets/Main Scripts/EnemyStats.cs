using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : Character
{
    protected override void Start()  // ✅ Correct override
    { 
        maxHealth = 100;   // Set maxHealth
        base.Start();      // ✅ Calls Character.Start() to set currentHealth = maxHealth
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"{characterName} took {damage} damage! Remaining HP: {currentHealth}");
    }


    protected override void Die()
    {
        Debug.Log($"{characterName} has died!");
        Destroy(gameObject);
    }
}
