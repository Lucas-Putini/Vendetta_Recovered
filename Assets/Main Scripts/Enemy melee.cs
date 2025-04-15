using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyMelee : MonoBehaviour
{
    
    private bool playerInRange = false; // Flag to check if the player is in range
    private Transform player;

    [Header("Melee Attack Settings")] 
    public float meleeDamage = 10f; // Damage dealt by the melee attack
    public float attackCooldown = 1f; // Time between attacks
    private float lastAttackTime; // Time of the last attack
    public float attackRange = 1f; // Range of the melee attack
    public float chaseSpeed = 2f; // Speed of the enemy when chasing the player�

    [Header("Movement & Physics")]
    public float jumpForce = 6f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rb;
    private bool isFacingRight = true;


    // detect player by tag if they enter the co
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
   


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
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered detection range");
        playerInRange = true;
        player = other.transform; // <-- you were missing this in the copy you just posted!
    }

      void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        // Flip enemy sprite based on direction
        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();

        // Check if we should jump over an obstacle
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 0.5f, groundLayer);
        if (hit.collider != null && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    IEnumerator AttackPlayer()
    {
        lastAttackTime = Time.time;

        // Delay to match animation hit frame if needed
        yield return new WaitForSeconds(0.2f);

        // Check for collision with player using Physics2D.OverlapCircle (you can replace this with a melee hitbox)
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
        // Visualize melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * 0.5f, attackRange);

        // Ground check gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}


  

    
    
