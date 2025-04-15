using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : Character
{
    public AudioClip deathClip;
    private AudioSource audioSource;
    
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
        
        //handles player gunshot audio
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
            Debug.Log("Playing " + deathClip.name);
        }
    }
}
