using System.Collections;
using UnityEngine;

public class EnemyShootScript : MonoBehaviour
{
    public GameObject bulletPrefab;  // Bullet prefab to instantiate
    public Transform firePoint;      // Where bullets spawn
    public float shootInterval = 1.0f;  // Time between shots
    public float bulletSpeed = 10f;     // Speed of bullets
    public float bulletLifetime = 3f;   // Time before bullets disappear
    public AudioClip gunshotSound;     // Sound effect for shooting

    private bool playerInRange = false;
    private Transform player;
    private AudioSource audioSource;
    private AIEnemyPatrol enemyPatrol; // Reference to patrol script

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource for sound effects
        enemyPatrol = GetComponent<AIEnemyPatrol>(); // Get patrol script
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            AimAtPlayer();

            // Check if the player is dead, and if so, stop shooting and resume patrolling
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null && playerScript.IsDead)
            {
                Debug.Log("Player is dead. Enemy stops shooting and resumes patrolling.");
                playerInRange = false;

                if (enemyPatrol != null)
                    enemyPatrol.SetAggressive(false); // Resume patrol

                StopAllCoroutines(); // Stop shooting loop
                player = null; // Forget the player
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;

            // Activate aggressive mode in patrol script
            if (enemyPatrol != null)
            {
                enemyPatrol.SetAggressive(true);
            }

            StartCoroutine(ShootAtPlayer());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Deactivate aggressive mode in patrol script
            if (enemyPatrol != null)
            {
                enemyPatrol.SetAggressive(false);
            }

            StopAllCoroutines(); // Stop shooting loop
        }
    }

    void AimAtPlayer()
    {
        if (player == null) return;

        // Determine if the player is to the left or right of the enemy
        bool playerIsOnRight = player.position.x > transform.position.x;

        // Flip the sprite if necessary
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = playerIsOnRight;
        }
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
        if (player == null) return; // Ensure player exists

        // Get the direction to the player
        Vector2 direction = (player.position - firePoint.position).normalized;
        Debug.Log($"Direction to player: {direction}");

        // Calculate angle for firePoint rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        // Instantiate and launch bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();

        if (bulletBehaviour != null)
        {
            Vector2 bulletVelocity = direction * bulletSpeed;
            bulletBehaviour.Initialize(bulletVelocity);
            Debug.Log($"Bullet position: {bullet.transform.position}");
            Debug.Log($"Bullet rotation: {bullet.transform.rotation.eulerAngles}");
            Debug.Log($"Bullet velocity: {bulletVelocity}");
        }
        else
        {
            Debug.LogError("BulletBehaviour is missing on the bullet!");
        }

        Destroy(bullet, bulletLifetime); // Destroy bullet after lifetime

        // Play gunshot sound
        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
    }
}
