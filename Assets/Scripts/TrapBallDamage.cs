using UnityEngine;

public class TrapBallDamage : MonoBehaviour
{
    [Tooltip("Amount of damage this trap deals to the player.")]
    public float damageAmount = 35f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }
}
