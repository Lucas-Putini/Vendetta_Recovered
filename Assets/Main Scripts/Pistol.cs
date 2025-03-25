using UnityEngine;

public class Pistol : RangedWeapon
{
    [Header("Pistol Settings")]
    public float range = 35f;
    public LineRenderer lineRenderer;
    private AudioSource audioSource;
    public AudioClip gunshotSound;  // Sound effect for shooting
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource for sound effects
    }

    public override void Fire()
    {
        if (Time.time - lastShotTime < 1f / fireRate) return; // Rate of fire check

        lastShotTime = Time.time;

        // Perform raycast in 2D
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, range);
        if (hit.collider != null)
        {
                // Check if the hit object has an EnemyStats component
                EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // Apply damage to the enemy
                }  
        }

        // Visual feedback with LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * range);
            lineRenderer.enabled = true;
            Invoke(nameof(HideLine), 0.1f); // Hide the line after 0.1 seconds
        }
        
        //handles player gunshot audio
        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
            Debug.Log("Playing " + gunshotSound.name);
        }
    }

    private void HideLine()
    {
        lineRenderer.enabled = false;
    }
}