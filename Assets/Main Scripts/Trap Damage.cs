using System.Collections;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] private float damagePerHit = 20f;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private float initialDamageDelay = 0f;

    [Header("Blood Effect Settings")]
    [SerializeField] private GameObject bloodEffect; // Assign SawBlood here in Inspector
    [SerializeField] private float bloodEffectDuration = 0.5f; // How long the blood effect stays visible
    [SerializeField] private float bloodScaleMultiplier = 1.2f; // Blood effect scale animation multiplier

    private void Start()
    {
        // Ensure blood effect is off at start
        if (bloodEffect != null)
        {
            bloodEffect.SetActive(false);
            // Set initial scale
            bloodEffect.transform.localScale = Vector3.one;
        }
    }

    private bool isDamaging = false;
    private Player currentPlayer = null;
    private Coroutine bloodEffectCoroutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isDamaging)
        {
            currentPlayer = collision.collider.GetComponent<Player>();
            if (currentPlayer != null)
            {
                isDamaging = true;
                ShowBloodEffect();
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
            ShowBloodEffect(); // Show blood effect each time damage is dealt
            yield return new WaitForSeconds(damageInterval);
        }

        StopDamaging();
    }

    private void StopDamaging()
    {
        isDamaging = false;
        currentPlayer = null;
        StopAllCoroutines();
        if (bloodEffect != null)
        {
            bloodEffect.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopDamaging();
    }

    private void ShowBloodEffect()
    {
        if (bloodEffect == null) return;

        // Stop any existing blood effect animation
        if (bloodEffectCoroutine != null)
        {
            StopCoroutine(bloodEffectCoroutine);
        }

        // Start new blood effect animation
        bloodEffectCoroutine = StartCoroutine(AnimateBloodEffect());
    }

    private IEnumerator AnimateBloodEffect()
    {
        bloodEffect.SetActive(true);
        
        // Animate scale up
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * bloodScaleMultiplier;
        
        while (elapsedTime < bloodEffectDuration * 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (bloodEffectDuration * 0.5f);
            bloodEffect.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        
        // Animate scale down
        elapsedTime = 0f;
        while (elapsedTime < bloodEffectDuration * 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (bloodEffectDuration * 0.5f);
            bloodEffect.transform.localScale = Vector3.Lerp(targetScale, startScale, progress);
            yield return null;
        }
        
        bloodEffect.transform.localScale = startScale;
        bloodEffect.SetActive(false);
    }
}
