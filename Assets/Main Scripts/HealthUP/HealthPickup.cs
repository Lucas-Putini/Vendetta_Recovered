using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public HealthItem healthItem;
    public LayerMask playerLayer;

    private void Start()
    {
        // Make sure we have a Collider2D
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }
        col.isTrigger = true;

        // Make sure we have a SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }
        if (healthItem != null && healthItem.icon != null)
        {
            sr.sprite = healthItem.icon;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered by {other.gameObject.name}"); // Debug log
        
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Found player, attempting pickup"); // Debug log
            player.AddHealthItem(healthItem);
            Destroy(gameObject);
        }
    }

    // Alternative pickup method using physics overlap
    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, playerLayer);
        foreach (Collider2D col in colliders)
        {
            Player player = col.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("Player in range, attempting pickup"); // Debug log
                player.AddHealthItem(healthItem);
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize pickup radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
} 