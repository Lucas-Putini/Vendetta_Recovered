using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private Vector2 velocity;
    private float damage = 10f;

    public void Initialize(Vector2 initialVelocity)
    {
        velocity = initialVelocity;
        Debug.Log($"Initialisation de la balle avec vélocité: {velocity}");
    }

    void Update()
    {
        // Déplacer la balle dans la direction de sa vélocité
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Balle a touché le joueur! Dégâts: {damage}");
            }
            Destroy(gameObject);
        }
    }
}

