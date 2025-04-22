using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string characterName;
    public float maxHealth;
    protected float currentHealth;
    
    // Property to access currentHealth
    public float CurrentHealth => currentHealth;
    
    protected Rigidbody2D rb;
    
    protected virtual void Start()  // Add this!
    {
        currentHealth = maxHealth; // Ensure health is properly initialized
    }

    
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}