using UnityEngine;

public class AIEnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;        // Vitesse de déplacement
    public float patrolDistance = 5f;   // Distance de patrouille de chaque côté
    public bool startMovingRight = true; // Direction initiale
    public Transform groundCheck;       // Point de vérification du sol
    public float groundCheckYOffset = -0.5f; // Offset Y pour la vérification du sol (si groundCheck est null)
    public LayerMask groundLayer;       // Layer du sol
    public float groundCheckRadius = 0.2f; // Rayon de vérification du sol
    public float directionChangeDelay = 0.5f; // Délai entre les changements de direction
    public float groundCheckDistance = 0.5f; // Distance horizontale pour vérifier le sol devant l'ennemi

    private Vector2 _startPosition;      // Position de départ
    private bool _movingRight;           // Direction actuelle
    private Rigidbody2D _rb;             // Référence au Rigidbody2D
    private SpriteRenderer _sprite;      // Référence au SpriteRenderer
    private bool _isAggressive = false;  // Indique si l'ennemi est en mode agressif
    private float _lastDirectionChangeTime = 0f; // Temps du dernier changement de direction
    private bool _isGrounded = true;     // Indique si l'ennemi est au sol
    private bool _wasGrounded = true;    // État précédent de l'ennemi (au sol ou non)
    private int _groundedFrameCount = 0; // Nombre de frames consécutives où l'ennemi est au sol

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        
        if (_rb == null)
        {
            Debug.LogError("Rigidbody2D non trouvé sur l'ennemi!");
            // Ajouter un Rigidbody2D si nécessaire
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 1f;
            _rb.freezeRotation = true;
        }
        
        if (_sprite == null)
        {
            Debug.LogError("SpriteRenderer non trouvé sur l'ennemi!");
        }
        
        _startPosition = transform.position;
        _movingRight = startMovingRight;
        
        // S'assurer que l'ennemi regarde dans la bonne direction au départ
        FlipSprite();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rb == null) return; // Vérification de sécurité
        
        // Si l'ennemi est en mode agressif, ne pas patrouiller
        if (_isAggressive)
        {
            // Arrêter le mouvement
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        // Vérifier si l'ennemi est au sol
        _isGrounded = IsGrounded();
        
        // Vérifier si l'ennemi doit changer de direction
        CheckDirectionChange();
        
        // Déplacer l'ennemi
        Move();
    }
    
    void CheckDirectionChange()
    {
        // Si l'ennemi vient de toucher le sol après avoir été en l'air, ne pas changer de direction immédiatement
        if (_isGrounded && !_wasGrounded)
        {
            _groundedFrameCount = 0;
            _wasGrounded = true;
            return;
        }
        
        // Incrémenter le compteur de frames au sol
        if (_isGrounded)
        {
            _groundedFrameCount++;
        }
        
        // Ne vérifier le changement de direction que si l'ennemi est au sol depuis plusieurs frames
        if (_isGrounded && _groundedFrameCount > 5)
        {
            // Vérifier s'il y a du sol devant l'ennemi
            bool groundAhead = CheckGroundAhead();
            
            // Si l'ennemi n'a pas de sol devant lui ou a atteint la limite de patrouille, changer de direction
            // mais seulement si assez de temps s'est écoulé depuis le dernier changement
            if (Time.time - _lastDirectionChangeTime > directionChangeDelay)
            {
                if (!groundAhead || ShouldChangeDirection())
                {
                    ChangeDirection();
                    _lastDirectionChangeTime = Time.time;
                }
            }
        }
        
        // Mettre à jour l'état précédent
        _wasGrounded = _isGrounded;
    }

    bool IsGrounded()
    {
        // Si groundCheck est assigné, l'utiliser
        if (groundCheck != null)
        {
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        // Sinon, utiliser la position du cube avec un offset
        else
        {
            Vector2 checkPosition = new Vector2(transform.position.x, transform.position.y + groundCheckYOffset);
            return Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
        }
    }
    
    bool CheckGroundAhead()
    {
        // Déterminer la position de départ du raycast
        Vector2 startPos;
        if (groundCheck != null)
        {
            startPos = groundCheck.position;
        }
        else
        {
            startPos = new Vector2(transform.position.x, transform.position.y + groundCheckYOffset);
        }
        
        // Déterminer la direction du raycast
        float direction = _movingRight ? 1f : -1f;
        Vector2 rayDirection = new Vector2(direction, -0.5f).normalized;
        
        // Effectuer le raycast
        RaycastHit2D hit = Physics2D.Raycast(startPos, rayDirection, groundCheckDistance, groundLayer);
        
        // Dessiner le raycast pour le débogage
        Debug.DrawRay(startPos, rayDirection * groundCheckDistance, hit.collider != null ? Color.green : Color.red, 0.1f);
        
        return hit.collider != null;
    }

    void Move()
    {
        if (_rb == null) return; // Vérification de sécurité
        
        // Calculer la vitesse en fonction de la direction
        float direction = _movingRight ? 1f : -1f;
        
        // Appliquer la vitesse
        _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);
    }

    bool ShouldChangeDirection()
    {
        // Calculer la distance par rapport à la position de départ
        float distanceFromStart = transform.position.x - _startPosition.x;
        
        // Vérifier si l'ennemi a atteint la limite de patrouille
        if (_movingRight && distanceFromStart >= patrolDistance)
        {
            return true;
        }
        else if (!_movingRight && distanceFromStart <= -patrolDistance)
        {
            return true;
        }
        
        return false;
    }

    void ChangeDirection()
    {
        // Inverser la direction
        _movingRight = !_movingRight;
        
        // Retourner le sprite
        FlipSprite();
    }

    void FlipSprite()
    {
        // Retourner le sprite en fonction de la direction
        if (_sprite != null)
        {
            _sprite.flipX = _movingRight;
        }
    }

    // Méthode publique pour définir l'état agressif
    public void SetAggressive(bool aggressive)
    {
        _isAggressive = aggressive;
    }

    // Dessiner des gizmos pour visualiser la zone de patrouille
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        
        // Dessiner la zone de patrouille
        Vector3 leftLimit = Application.isPlaying ? new Vector3(_startPosition.x - patrolDistance, _startPosition.y, 0) : new Vector3(transform.position.x - patrolDistance, transform.position.y, 0);
        Vector3 rightLimit = Application.isPlaying ? new Vector3(_startPosition.x + patrolDistance, _startPosition.y, 0) : new Vector3(transform.position.x + patrolDistance, transform.position.y, 0);
        
        Gizmos.DrawLine(leftLimit, rightLimit);
        Gizmos.DrawSphere(leftLimit, 0.2f);
        Gizmos.DrawSphere(rightLimit, 0.2f);
        
        // Dessiner le point de vérification du sol
        Vector3 groundCheckPos;
        if (groundCheck != null)
        {
            groundCheckPos = groundCheck.position;
        }
        else
        {
            groundCheckPos = new Vector3(transform.position.x, transform.position.y + groundCheckYOffset, transform.position.z);
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPos, groundCheckRadius);
        
        // Dessiner le raycast de vérification du sol devant
        if (Application.isPlaying)
        {
            float direction = _movingRight ? 1f : -1f;
            Vector2 rayDirection = new Vector2(direction, -0.5f).normalized;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(groundCheckPos, rayDirection * groundCheckDistance);
        }
    }
}
