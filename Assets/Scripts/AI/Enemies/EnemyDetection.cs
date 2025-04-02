using UnityEngine;
using System.Collections;

public class EnemyDetection : MonoBehaviour
{
    public float detectionRadius = 5f; // Rayon de détection
    public float fieldOfViewAngle = 90f; // Angle du champ de vision en degrés
    public LayerMask playerLayer; // Layer du joueur
    public LayerMask obstacleLayer; // Layer des obstacles pour bloquer la vision

    private bool _playerDetected = false;
    private Transform _player;
    private Renderer _enemyRenderer;
    private Color _originalColor;
    private AIEnemyPatrol _enemyPatrol; // Référence au script de patrouille
    private SpriteRenderer _spriteRenderer; // Référence au SpriteRenderer

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Trouver le joueur avec le tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'Player' n'a été trouvé!");
        }

        // Récupérer le renderer pour changer la couleur
        _enemyRenderer = GetComponent<Renderer>();
        if (_enemyRenderer != null)
        {
            _originalColor = _enemyRenderer.material.color;
        }
        else
        {
            Debug.LogError("Aucun Renderer n'a été trouvé sur cet objet!");
        }

        // Récupérer le SpriteRenderer
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("Aucun SpriteRenderer n'a été trouvé sur cet objet!");
        }

        // Récupérer le script de patrouille
        _enemyPatrol = GetComponent<AIEnemyPatrol>();
        if (_enemyPatrol == null)
        {
            Debug.LogError("Aucun script AIEnemyPatrol n'a été trouvé sur cet objet!");
        }

        // Démarrer la coroutine de détection
        StartCoroutine(DetectionRoutine());
    }

    IEnumerator DetectionRoutine()
    {
        while (true)
        {
            DetectPlayer();
            yield return new WaitForSeconds(0.2f); // Vérifier toutes les 0.2 secondes
        }
    }

    void DetectPlayer()
    {
        if (_player == null) 
        {
            Debug.LogError("Le joueur est null!");
            return;
        }

        // Calculer la distance entre l'ennemi et le joueur
        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        // Vérifier si le joueur est dans le rayon de détection
        if (distanceToPlayer <= detectionRadius)
        {
            // Calculer la direction vers le joueur
            Vector2 directionToPlayer = (_player.position - transform.position).normalized;
            
            // Déterminer la direction de l'ennemi en fonction du sprite
            Vector2 facingDirection = GetFacingDirection();
            
            // Calculer l'angle entre la direction de l'ennemi et la direction vers le joueur
            float angleToPlayer = Vector2.Angle(facingDirection, directionToPlayer);

            // Vérifier si le joueur est dans le champ de vision
            if (angleToPlayer <= fieldOfViewAngle / 2)
            {
                // Vérifier s'il y a un obstacle entre l'ennemi et le joueur
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

                // Si aucun obstacle n'est détecté, le joueur est visible
                if (hit.collider == null)
                {
                    if (!_playerDetected)
                    {
                        _playerDetected = true;
                        OnPlayerDetected();
                    }
                    return;
                }
            }
        }

        // Si le joueur n'est pas détecté, réinitialiser l'état
        if (_playerDetected)
        {
            _playerDetected = false;
            OnPlayerLost();
        }
    }

    // Obtenir la direction dans laquelle l'ennemi regarde
    Vector2 GetFacingDirection()
    {
        // Par défaut, utiliser la direction droite
        Vector2 facingDirection = Vector2.right;
        
        // Si le sprite est retourné, l'ennemi regarde à gauche
        if (_spriteRenderer != null && _spriteRenderer.flipX)
        {
            facingDirection = Vector2.left;
        }
        
        return facingDirection;
    }

    void OnPlayerDetected()
    {
        // Changer la couleur de l'ennemi en rouge
        if (_enemyRenderer != null)
        {
            _enemyRenderer.material.color = Color.red;
        }

        // Activer le mode agressif dans le script de patrouille
        if (_enemyPatrol != null)
        {
            _enemyPatrol.SetAggressive(true);
        }

        // Faire en sorte que l'ennemi regarde vers le joueur
        if (_player != null && _spriteRenderer != null)
        {
            // Déterminer si le joueur est à gauche ou à droite de l'ennemi
            bool playerIsOnRight = _player.position.x > transform.position.x;
            
            // Retourner le sprite si nécessaire
            _spriteRenderer.flipX = playerIsOnRight;
        }
    }

    void OnPlayerLost()
    {
        // Rétablir la couleur d'origine
        if (_enemyRenderer != null)
        {
            _enemyRenderer.material.color = _originalColor;
        }

        // Désactiver le mode agressif dans le script de patrouille
        if (_enemyPatrol != null)
        {
            _enemyPatrol.SetAggressive(false);
        }
    }

    // Dessiner le champ de vision dans l'éditeur pour faciliter le débogage
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Déterminer la direction de l'ennemi
        Vector2 facingDirection = Vector2.right;
        
        // Si nous sommes en mode Play, utiliser la direction réelle
        if (Application.isPlaying && _spriteRenderer != null)
        {
            facingDirection = GetFacingDirection();
        }
        // Sinon, essayer de déterminer la direction à partir du SpriteRenderer
        else
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.flipX)
            {
                facingDirection = Vector2.left;
            }
        }
        
        // Calculer les limites du champ de vision en fonction de la direction
        Vector3 leftBoundary = Quaternion.Euler(0, 0, fieldOfViewAngle / 2) * (Vector3)facingDirection * detectionRadius;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -fieldOfViewAngle / 2) * (Vector3)facingDirection * detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        // Dessiner la direction de l'ennemi
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (Vector3)facingDirection * 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // Nous utilisons la coroutine DetectionRoutine à la place
    }
}
