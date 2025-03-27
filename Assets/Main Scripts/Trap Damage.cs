using System.Collections;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private float damagePerHit = 20f; // Damage per hit
    [SerializeField] private float damageInterval = 0.5f; // Time between damage ticks
    [SerializeField] private float initialDamageDelay = 0f; // Delay before first damage
    
    private bool isDamaging = false;
    private Player currentPlayer = null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isDamaging)
        {
            currentPlayer = collision.collider.GetComponent<Player>();
            if (currentPlayer != null)
            {
                isDamaging = true;
                StartCoroutine(DamagePlayer());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StopDamaging();
        }
    }

    private IEnumerator DamagePlayer()
    {
        // Initial delay before first damage
        if (initialDamageDelay > 0)
        {
            yield return new WaitForSeconds(initialDamageDelay);
        }

        // Continuous damage while player is in contact
        while (isDamaging && currentPlayer != null)
        {
            currentPlayer.TakeDamage(damagePerHit);
            yield return new WaitForSeconds(damageInterval);
        }
        
        StopDamaging();
    }

    private void StopDamaging()
    {
        isDamaging = false;
        currentPlayer = null;
        StopAllCoroutines();
    }

    // Optional: Stop damaging if the trap is disabled
    private void OnDisable()
    {
        StopDamaging();
    }
}