using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : Character
{
    public AudioClip deathClip;
    private AudioSource audioSource;
    public AudioClip hurtClip;
    
    protected override void Start()  // ✅ Correct override
    { 
        maxHealth = 100;   // Set maxHealth
        base.Start();      // ✅ Calls Character.Start() to set currentHealth = maxHealth
        audioSource = GetComponent<AudioSource>(); // ✅ Get the attached AudioSource
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log($"{characterName} took {damage} damage! Remaining HP: {currentHealth}");
        
        if (currentHealth > 0 && audioSource != null && hurtClip != null)
        {
            audioSource.PlayOneShot(hurtClip);
        }

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
