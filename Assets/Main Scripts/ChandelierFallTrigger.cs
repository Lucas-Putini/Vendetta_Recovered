using UnityEngine;

public class ChandelierDropper : MonoBehaviour
{
    public float damage = 30f;
    public float destroyDelay = 0.3f;

    private Rigidbody2D rb;
    private bool hasDropped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Drop()
    {
        if (!hasDropped)
        {
            hasDropped = true;
            rb.bodyType = RigidbodyType2D.Dynamic;

            
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Player player = other.collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                DestroyChandelier();
            }
        }
    }

    void DestroyChandelier()
    {
        // You can add a particle effect or sound here before destroying
        Destroy(gameObject, destroyDelay);
    }
}
