using System.Collections;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private float damagePerHit = 20f;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float initialDamageDelay = 0f;

    [Header("Optional Blood Effect")]
    [SerializeField] private GameObject bloodEffect; // Assign SawBlood here in Inspector

    private void Start()
    {
        // Ensure blood effect is off at start
        ActivateBloodEffect(false);
    }

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
                ActivateBloodEffect(true);
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
        if (initialDamageDelay > 0)
            yield return new WaitForSeconds(initialDamageDelay);

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
        ActivateBloodEffect(false);
    }

    private void OnDisable()
    {
        StopDamaging();
    }

    private void ActivateBloodEffect(bool isActive)
    {
        if (bloodEffect != null)
        {
            bloodEffect.SetActive(isActive);
        }
    }
}
