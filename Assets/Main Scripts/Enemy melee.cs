using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyMelee : MonoBehaviour
{
    private bool playerInRange = false;
    private Transform player;
    private EnemyStats enemyStats;

    [Header("Melee Attack Settings")]
    public float meleeDamage = 10f;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    public float attackRange = 1f;
    public float chaseSpeed = 2f;

    [Header("Movement & Physics")]
    public float jumpForce = 6f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    // Animator reference
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats component not found on EnemyMelee!");
        }
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance > attackRange)
            {
                ChasePlayer();
            }
            else if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        animator.SetBool("isJumping", !IsGrounded());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered detection range");
            playerInRange = true;
            player = other.transform;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        animator.SetBool("isRunning", true);

        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 0.5f, groundLayer);
        if (hit.collider != null && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
        }
    }

    IEnumerator AttackPlayer()
    {
        lastAttackTime = Time.time;

        // Trigger punch animation
        animator.SetTrigger("punch");

        yield return new WaitForSeconds(0.2f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + transform.right * 0.5f, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player playerScript = hit.GetComponent<Player>();
                if (playerScript != null && !playerScript.IsDead)
                {
                    playerScript.TakeDamage(meleeDamage);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage);
        }
        
        animator.SetTrigger("hurt");
        
        if (enemyStats != null && enemyStats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("die");
        // Disable enemy logic (colliders, movement, etc.)
        this.enabled = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * 0.5f, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
