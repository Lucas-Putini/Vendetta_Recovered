using UnityEngine;

public class AIEnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;                    // Movement speed
    public float patrolDistance = 5f;               // Patrol distance from starting point
    public bool startMovingRight = true;            // Initial movement direction
    public Transform groundCheck;                   // Ground check point
    public float groundCheckYOffset = -0.5f;        // Y offset if no groundCheck transform is set
    public LayerMask groundLayer;                   // Layer used to define what is "ground"
    public float groundCheckRadius = 0.2f;          // Radius for ground check
    public float directionChangeDelay = 0.5f;       // Delay between allowed direction changes
    public float groundCheckDistance = 0.5f;        // Distance ahead to check for ground

    private Vector2 _startPosition;                 // Starting patrol position
    private bool _movingRight;                      // Current direction of movement
    private Rigidbody2D _rb;                        // Rigidbody2D reference
    private SpriteRenderer _sprite;                 // SpriteRenderer reference
    private bool _isAggressive = false;             // Is enemy in aggressive (combat) mode?
    private float _lastDirectionChangeTime = 0f;    // Time since last direction change
    private bool _isGrounded = true;                // Is enemy currently grounded?
    private bool _wasGrounded = true;               // Previous frame grounded status
    private int _groundedFrameCount = 0;            // How long has enemy been grounded
    private Animator _animator;                     // Animator reference

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        if (_rb == null)
        {
            Debug.LogError("Rigidbody2D not found on enemy!");
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 1f;
            _rb.freezeRotation = true;
        }

        if (_sprite == null)
        {
            Debug.LogError("SpriteRenderer not found on enemy!");
        }

        if (_animator == null)
        {
            Debug.LogError("Animator not found on enemy!");
        }

        _startPosition = transform.position;
        _movingRight = startMovingRight;

        FlipSprite();
    }

    void FixedUpdate()
    {
        if (_rb == null) return;

        if (_isAggressive)
        {
            // Stop patrol movement when aggressive
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        _isGrounded = IsGrounded();
        CheckDirectionChange();
        Move();
    }

    void CheckDirectionChange()
    {
        if (_isGrounded && !_wasGrounded)
        {
            _groundedFrameCount = 0;
            _wasGrounded = true;
            return;
        }

        if (_isGrounded)
        {
            _groundedFrameCount++;
        }

        if (_isGrounded && _groundedFrameCount > 5)
        {
            bool groundAhead = CheckGroundAhead();

            if (Time.time - _lastDirectionChangeTime > directionChangeDelay)
            {
                if (!groundAhead || ShouldChangeDirection())
                {
                    ChangeDirection();
                    _lastDirectionChangeTime = Time.time;
                }
            }
        }

        _wasGrounded = _isGrounded;
    }

    bool IsGrounded()
    {
        if (groundCheck != null)
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            Vector2 checkPosition = new Vector2(transform.position.x, transform.position.y + groundCheckYOffset);
            return Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
        }
    }

    bool CheckGroundAhead()
    {
        Vector2 startPos = groundCheck != null
            ? groundCheck.position
            : new Vector2(transform.position.x, transform.position.y + groundCheckYOffset);

        float direction = _movingRight ? 1f : -1f;
        Vector2 rayDirection = new Vector2(direction, -0.5f).normalized;

        RaycastHit2D hit = Physics2D.Raycast(startPos, rayDirection, groundCheckDistance, groundLayer);
        Debug.DrawRay(startPos, rayDirection * groundCheckDistance, hit.collider != null ? Color.green : Color.red, 0.1f);

        return hit.collider != null;
    }

    void Move()
    {
        if (_rb == null) return;

        float direction = _movingRight ? 1f : -1f;
        _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);
    }

    bool ShouldChangeDirection()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        if (_movingRight && distanceFromStart >= patrolDistance)
            return true;
        if (!_movingRight && distanceFromStart <= -patrolDistance)
            return true;

        return false;
    }

    void ChangeDirection()
    {
        _movingRight = !_movingRight;
        FlipSprite();
    }

    void FlipSprite()
    {
        if (_sprite != null)
        {
            _sprite.flipX = _movingRight;
        }
    }

    public void SetAggressive(bool aggressive)
    {
        _isAggressive = aggressive;

        if (_animator != null)
        {
            _animator.SetBool("isShooting", aggressive);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Vector3 leftLimit = Application.isPlaying
            ? new Vector3(_startPosition.x - patrolDistance, _startPosition.y, 0)
            : new Vector3(transform.position.x - patrolDistance, transform.position.y, 0);

        Vector3 rightLimit = Application.isPlaying
            ? new Vector3(_startPosition.x + patrolDistance, _startPosition.y, 0)
            : new Vector3(transform.position.x + patrolDistance, transform.position.y, 0);

        Gizmos.DrawLine(leftLimit, rightLimit);
        Gizmos.DrawSphere(leftLimit, 0.2f);
        Gizmos.DrawSphere(rightLimit, 0.2f);

        Vector3 groundCheckPos = groundCheck != null
            ? groundCheck.position
            : new Vector3(transform.position.x, transform.position.y + groundCheckYOffset, transform.position.z);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPos, groundCheckRadius);

        if (Application.isPlaying)
        {
            float direction = _movingRight ? 1f : -1f;
            Vector2 rayDirection = new Vector2(direction, -0.5f).normalized;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(groundCheckPos, rayDirection * groundCheckDistance);
        }
    }
}
