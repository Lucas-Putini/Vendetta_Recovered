using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    private float Move;
    private Rigidbody2D rb;
    private float jump;
    // public bool isJumping; //if the player is on the floor, it can jump. Otherwise it cannot. **Chatgpt took this off to make the double jump**
    private int jumpCount; // New variable to track jumps
    private int maxJumps = 2; // Allow up to two jumps

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 7f;
        jump = 180f;
        jumpCount = 0; // Start with 0 jumps
    }

    // Update is called once per frame
    void Update()
    {
        Move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(speed * Move, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.W) && jumpCount < maxJumps)
        {
            rb.AddForce(new Vector2(rb.linearVelocity.x, jump));
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            jumpCount++; // Increment jump count
            Debug.Log("jump worked");
        }
        if (Mathf.Abs(rb.linearVelocity.x) < speed || Move == 0)
        {
            rb.linearVelocity = new Vector2(speed * Move, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; // Reset jumps when touching the ground
            Debug.Log("Touched Ground, Jumps Reset");
        }

    }
}
