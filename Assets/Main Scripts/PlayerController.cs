using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 100f;

    [Header("Dash Settings")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Key System")]
    public bool hasKey = false;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime;

    [Header("Crouch Settings")]
    public float crouchSpeedMultiplier = 0.5f;
    private bool isCrouching = false;

    [Header("Crouch Collider Settings")]
    public CapsuleCollider2D capsuleCollider;
    public Vector2 standingSize = new Vector2(1f, 2f);
    public Vector2 crouchingSize = new Vector2(1f, 1f);
    public Vector2 standingOffset = new Vector2(0f, 0f);
    public Vector2 crouchingOffset = new Vector2(0f, -0.5f);

    // LUCAS - Jump Limit
    private int jumpCount = 0;
    private int maxJumps = 2;

    // Animation
    public Animator animator;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Vérifie si le menu de mort est actif
        if (DeathMenu.Instance != null && DeathMenu.Instance.deathMenuUI != null && DeathMenu.Instance.deathMenuUI.activeSelf)
        {
            // Arrête tout mouvement si le menu de mort est actif
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Handle movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;

        // Handle animation flip
        HandleFlip(moveInput);

        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.W) && (jumpCount < maxJumps))
        {
            PlayerJump();
        }

        // Handle dashing
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(1)) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash(moveInput);
        }

        // Handle dash duration
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                dashTimeLeft -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
            }
        }

        // Handle crouching
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCrouch();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            StopCrouch();
        }
    }

    void PlayerJump()
    {
        rb.AddForce(new Vector2(rb.linearVelocity.x, jumpForce));
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        jumpCount++;
        animator.SetBool("IsJumping", true);
        Debug.Log("jump worked");
    }

    void StartDash(float moveDirection)
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;

        float dashDirection = moveDirection != 0 ? moveDirection : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            animator.SetBool("IsJumping", false);
            Debug.Log("Touched Ground, Jumps Reset");
        }
    }

    void HandleFlip(float moveInput)
    {
        // Ne pas retourner le personnage si le menu de mort est actif
        if (DeathMenu.Instance != null && DeathMenu.Instance.deathMenuUI != null && DeathMenu.Instance.deathMenuUI.activeSelf)
        {
            return;
        }

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Le joueur a reçu la clé !");
    }

    public bool HasKey()
    {
        return hasKey;
    }

    void StartCrouch()
    {
        isCrouching = true;
        animator.SetBool("IsCrouching", true);

        if (capsuleCollider != null)
        {
            capsuleCollider.size = crouchingSize;
            capsuleCollider.offset = crouchingOffset;
        }
    }

    void StopCrouch()
    {
        isCrouching = false;
        animator.SetBool("IsCrouching", false);

        if (capsuleCollider != null)
        {
            capsuleCollider.size = standingSize;
            capsuleCollider.offset = standingOffset;
        }
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
}
