using System.Collections;
using UnityEngine;

public class EnemyShootScript : MonoBehaviour
{
    public GameObject bulletPrefab;  // Bullet prefab to instantiate
    public Transform firePoint;  // Where bullets spawn
    public float shootInterval = 1.0f;  // Time between shots
    public float bulletSpeed = 10f;  // Speed of bullets
    public float bulletLifetime = 4f;  // Time before bullets disappear
    public AudioClip gunshotSound;  // Sound effect for shooting

    private bool playerInRange = false;
    private Transform player;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource for sound effects
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            AimAtPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
            StartCoroutine(ShootAtPlayer());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopCoroutine(ShootAtPlayer());
        }
    }

    void AimAtPlayer()
    {
        if (firePoint == null || player == null) return;
        
        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator ShootAtPlayer()
    {
        while (playerInRange)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void Shoot()
    {
        if (player == null || firePoint == null) return; // Ensure player and firePoint exist

        // Get the direction to the player
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Instantiate and launch bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed; // Move bullet in direction of player
        }

        Destroy(bullet, bulletLifetime); // Destroy bullet after lifetime

        // Play gunshot sound
        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
    }
}

