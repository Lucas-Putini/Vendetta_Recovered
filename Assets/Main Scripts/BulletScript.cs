using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damage = 10;  // Bullet damage

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy bullet after hitting player
        }
        else if (!other.CompareTag("Enemy")) // Prevent bullets from destroying on enemy collision
        {
            Destroy(gameObject); // Destroy bullet on collision with walls or objects
        }
    }
}