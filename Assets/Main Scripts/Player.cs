using UnityEngine;
using MagicPigGames;  // Ensure that any base classes like Character are accessible

public class Player : Character
{
    public RangedWeapon equippedWeapon;
    public Transform aimPivot; // Pivot point for aiming (e.g., the player's hand or gun base)
    public ProgressBar healthBarUI; // Assign this from the Inspector

    // Shooting and death animations
    public Animator animator;

    // So the enemy knows when to stop shooting
    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        maxHealth = 100f;
        currentHealth = maxHealth;

        if (equippedWeapon == null)
            Debug.LogError("No weapon equipped!");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentHealth > 0)
        {
            equippedWeapon.Fire();

            // Check if crouching
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null && controller.IsCrouching())
            {
                animator.SetTrigger("CrouchShoot");
            }
            else
            {
                animator.SetTrigger("Shoot");
            }
        }

        AimWeapon(); // Don't forget to keep this active!
    }

    private void AimWeapon()
    {
        // Get mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure it's in 2D space

        // Calculate direction from the aim pivot to the mouse position
        Vector3 direction = (mousePosition - aimPivot.position).normalized;

        // Calculate the angle for rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the aim pivot to face the mouse
        aimPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBarUI != null)
        {
            float progress = currentHealth / maxHealth;
            healthBarUI.SetProgress(progress);
        }

        if (currentHealth <= 0)
        {
            animator.SetBool("IsDead", true);
            Die();
            return;
        }

        animator.SetTrigger("Hurt");
        Debug.Log("Player Health: " + currentHealth);
    }

    protected override void Die()
    {
        animator.SetBool("IsDead", true);
        this.enabled = false;
        Destroy(gameObject, 2f); // Adjust time to match your animation duration
        DeathMenu.Instance.ShowDeathMenu();
    }
}
