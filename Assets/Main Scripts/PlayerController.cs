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

    // LUCAS - Jump Limit
    private int jumpCount = 0; // For tracking how many jumps
    private int maxJumps = 2; // Maximum allowed jumps (double jump)

    //Animation
    public Animator animator; // For accessing the animator component
    private bool isFacingRight = true; // to manage which way the animation is fancing

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground Check (Detects if the player is touching the ground)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Handle movement
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Handle animation flip
        HandleFlip(moveInput);

        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Handle jumping // LUCAS: let's see if the jump limit works, I also changed the jump key from spacebar to W
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
    }

    void PlayerJump()
    {
        rb.AddForce(new Vector2(rb.linearVelocity.x, jumpForce));
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        jumpCount++; // Increment jump count
        animator.SetBool("IsJumping", true); //  Trigger jumping anim
        Debug.Log("jump worked");
    }

    void StartDash(float moveDirection)
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;

        // Apply dash speed in the direction of movement
        float dashDirection = moveDirection != 0 ? moveDirection : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
    }

    //This checks if the player hit the ground ("Ground" tag), if so, the jump count goes back to 0
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; // Reset jumps when touching the ground
            animator.SetBool("IsJumping", false); // Reset jumping animation
            Debug.Log("Touched Ground, Jumps Reset");
        }
    }

    //for flipping the animation
    void HandleFlip(float moveInput) // checks the movement input and flips only when needed
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

    void Flip() // toggles the x scale to mirror the sprite
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Méthode pour donner la clé au joueur
    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Le joueur a reçu la clé !");
    }

    // Méthode pour vérifier si le joueur a la clé
    public bool HasKey()
    {
        return hasKey;
    }
}