using UnityEngine;

public class Key : MonoBehaviour
{
    public float rotationSpeed = 100f; // Vitesse de rotation en degrés par seconde
    public float floatHeight = 0.5f; // Hauteur du mouvement de flottement
    public float floatSpeed = 2f; // Vitesse du mouvement de flottement

    private Vector3 _startPosition;
    private float _timeOffset;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    void Start()
    {
        _startPosition = transform.position;
        _timeOffset = Random.Range(0f, 2f * Mathf.PI); // Offset aléatoire pour le flottement
        
        // S'assurer que la clé est visible
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            // Mettre la clé dans le layer "Items" s'il existe
            if (SortingLayer.NameToID("Items") != 0)
            {
                _spriteRenderer.sortingLayerName = "Items";
            }
            // S'assurer que la clé est au-dessus du background
            _spriteRenderer.sortingOrder = 1;
        }

        // Vérifier le collider
        _boxCollider = GetComponent<BoxCollider2D>();
        if (_boxCollider != null)
        {
            Debug.Log($"Le collider de la clé est en mode Trigger : {_boxCollider.isTrigger}");
            if (!_boxCollider.isTrigger)
            {
                Debug.LogWarning("Le collider de la clé n'est pas en mode Trigger !");
                _boxCollider.isTrigger = true;
            }
        }
        else
        {
            Debug.LogError("La clé n'a pas de BoxCollider2D !");
        }
    }

    void Update()
    {
        // Rotation de la clé
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Flottement de la clé
        float newY = _startPosition.y + Mathf.Sin((Time.time + _timeOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Test de collision avec le joueur
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _boxCollider.size, 0f);
        foreach (Collider2D collider in colliders)
        {
            Debug.Log($"Collision détectée avec : {collider.gameObject.name} (Tag: {collider.tag})");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger détecté avec : {other.gameObject.name} (Tag: {other.tag})");
        Debug.Log($"Le GameObject a un PlayerController : {other.GetComponent<PlayerController>() != null}");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Le tag Player a été détecté");
            
            // Récupérer le composant PlayerController du joueur
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("Le composant PlayerController a été trouvé");
                // Donner la clé au joueur
                playerController.GiveKey();
                
                // Désactiver la clé dans la scène
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Le composant PlayerController n'a pas été trouvé sur le joueur !");
            }
        }
        else
        {
            Debug.LogWarning($"Le GameObject n'a pas le tag Player. Tag actuel : {other.tag}");
        }
    }

    // Pour visualiser la zone de collision dans l'éditeur
    void OnDrawGizmos()
    {
        if (_boxCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _boxCollider.size);
        }
    }
} 