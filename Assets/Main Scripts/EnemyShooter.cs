using System.Collections;
using UnityEngine;

public class EnemyShootScript : MonoBehaviour
{
    public GameObject bulletPrefab;  // Bullet prefab to instantiate
    public Transform firePoint;  // Where bullets spawn
    public float shootInterval = 1.0f;  // Time between shots
    public float bulletSpeed = 10f;  // Speed of bullets
    public float bulletLifetime = 3f;  // Time before bullets disappear
    public AudioClip gunshotSound;  // Sound effect for shooting

    private bool playerInRange = false;
    private Transform player;
    private AudioSource audioSource;
    private AIEnemyPatrol enemyPatrol; // Référence au script de patrouille

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource for sound effects
        enemyPatrol = GetComponent<AIEnemyPatrol>(); // Récupérer le script de patrouille
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
            // Activer le mode agressif dans le script de patrouille
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
            // Désactiver le mode agressif dans le script de patrouille
            if (enemyPatrol != null)
            {
                enemyPatrol.SetAggressive(false);
            }
            StopCoroutine(ShootAtPlayer());
        }
    }

    void AimAtPlayer()
    {
        if (player == null) return;

        // Déterminer si le joueur est à gauche ou à droite de l'ennemi
        bool playerIsOnRight = player.position.x > transform.position.x;
        
        // Retourner le sprite si nécessaire
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
        Debug.Log($"Direction vers le joueur: {direction}");

        // Calculer l'angle pour la rotation du point de tir
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        // Instantiate and launch bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();

        if (bulletBehaviour != null)
        {
            // La direction est déjà normalisée, on multiplie juste par la vitesse
            Vector2 bulletVelocity = direction * bulletSpeed;
            bulletBehaviour.Initialize(bulletVelocity);
            Debug.Log($"Position de la balle: {bullet.transform.position}");
            Debug.Log($"Rotation de la balle: {bullet.transform.rotation.eulerAngles}");
            Debug.Log($"Vélocité de la balle: {bulletVelocity}");
        }
        else
        {
            Debug.LogError("BulletBehaviour manquant sur la balle!");
        }

        Destroy(bullet, bulletLifetime); // Destroy bullet after lifetime

        // Play gunshot sound
        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
    }
}


