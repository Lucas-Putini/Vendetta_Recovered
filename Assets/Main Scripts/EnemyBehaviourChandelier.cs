using System.Collections;
using UnityEngine;

public class EnemyChandelierShooter : EnemyStats
{
    [Header("Detection")]
    public float detectionRange = 10f;
    public LayerMask playerLayer;

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootInterval = 1f;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f;
    public AudioClip gunshotSound;

    [Header("Chandelier")]
    public Transform chandelierTarget;
    public GameObject chandelierObject; // Object that has a Drop() method

    private bool hasTriggeredChandelier = false;
    private bool isShooting = false;

    private Transform player;
    private AudioSource audioSource;
    private Coroutine shootingRoutine;

    void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null)
        {
            TryDetectPlayer();
        }
        else
        {
            FlipSpriteTowards(player.position);

            if (!hasTriggeredChandelier)
            {
                StartCoroutine(ShootAtChandelierThenAggro());
            }
            else
            {
                HandleCombatBehavior();
            }
        }
    }

    void TryDetectPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, detectionRange, playerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            player = hit.collider.transform;
        }
    }

    IEnumerator ShootAtChandelierThenAggro()
    {
        hasTriggeredChandelier = true;

        yield return new WaitForSeconds(0.5f);

        if (chandelierTarget != null)
        {
            Vector2 direction = (chandelierTarget.position - firePoint.position).normalized;
            ShootBullet(direction);

            if (chandelierObject != null)
            {
                chandelierObject.SendMessage("Drop", SendMessageOptions.DontRequireReceiver);
            }
        }

        yield return new WaitForSeconds(1.5f);
    }

    void HandleCombatBehavior()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Move toward player if too far
        if (distanceToPlayer > 5f)
        {
            Vector2 moveDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(moveDir * Time.deltaTime * 2f);
        }

        // Begin or continue shooting
        if (!isShooting)
        {
            shootingRoutine = StartCoroutine(ShootAtPlayerRoutine());
        }
    }

    IEnumerator ShootAtPlayerRoutine()
    {
        isShooting = true;

        while (player != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            ShootBullet(direction);
            yield return new WaitForSeconds(shootInterval);
        }

        isShooting = false;
    }

    void ShootBullet(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();

        if (bulletBehaviour != null)
        {
            Vector2 bulletVelocity = direction * bulletSpeed;
            bulletBehaviour.Initialize(bulletVelocity);
        }

        Destroy(bullet, bulletLifetime);

        if (audioSource != null && gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
    }

    void FlipSpriteTowards(Vector3 targetPosition)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = targetPosition.x > transform.position.x;
        }
    }
}
