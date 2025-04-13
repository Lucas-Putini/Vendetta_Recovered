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
    public float crouchSpeedMultiplier = 0.5f; // Slower movement when crouching
    private bool isCrouching = false;
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;

    // LUCAS - Jump Limit
    private int jumpCount = 0; // For tracking how many jumps
    private int maxJumps = 2; // Maximum allowed jumps (double jump)

    // Animation
    public Animator animator; // For accessing the animator component
    private bool isFacingRight = true; // To manage which way the animation is facing

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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

        if (standingCollider != null && crouchingCollider != null)
        {
            standingCollider.enabled = false;
            crouchingCollider.enabled = true;
        }
    }

    void StopCrouch()
    {
        isCrouching = false;
        animator.SetBool("IsCrouching", false);

        if (standingCollider != null && crouchingCollider != null)
        {
            crouchingCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }
}
