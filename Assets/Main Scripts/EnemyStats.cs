using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : Character
{
    public AudioClip deathClip;
    private AudioSource audioSource;
    public AudioClip hurtClip;
    private MonoBehaviour healthBar; // Changed to MonoBehaviour to support both health bar types
    
    protected override void Start()
    {
        maxHealth = 100;   // Set maxHealth
        base.Start();      // Calls Character.Start() to set currentHealth = maxHealth
        audioSource = GetComponent<AudioSource>();
        
        // Try to get either type of health bar
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<MeleeEnemyHealthBar>();
        }
        
        if (healthBar == null)
        {
            Debug.LogWarning("No health bar component found on enemy!");
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"{characterName} took {damage} damage! Remaining HP: {currentHealth}");
        
        // Update health bar
        if (healthBar != null)
        {
            if (healthBar is EnemyHealthBar standardBar)
            {
                standardBar.UpdateHealthBar(currentHealth, maxHealth);
            }
            else if (healthBar is MeleeEnemyHealthBar meleeBar)
            {
                meleeBar.UpdateHealthBar(currentHealth, maxHealth);
            }
        }
        
        if (currentHealth > 0 && audioSource != null && hurtClip != null)
        {
            audioSource.PlayOneShot(hurtClip);
        }
    }

    protected override void Die()
    {
        Debug.Log($"{characterName} has died!");
        
        //handles player gunshot audio
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
            Debug.Log("Playing " + deathClip.name);
        }
        
        Destroy(gameObject);
    }
}
