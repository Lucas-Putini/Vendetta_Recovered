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
        // Vérifie si le menu de mort est actif
        if (DeathMenu.Instance != null && DeathMenu.Instance.deathMenuUI != null && DeathMenu.Instance.deathMenuUI.activeSelf)
        {
            return; // Sort de la fonction si le menu de mort est actif
        }

        // Vérifie si le mode auto-aim est activé
        if (GameSettings.Instance != null && GameSettings.Instance.autoAimMode)
        {
            // En mode auto-aim, on ne fait pas pivoter l'arme
            // L'arme reste alignée avec la direction du personnage
            aimPivot.rotation = Quaternion.Euler(0, 0, transform.localScale.x > 0 ? 0 : 180);
            
            // Tirer avec la touche Espace
            if (Input.GetKeyDown(GameSettings.Instance.autoAimKey) && currentHealth > 0)
            {
                FireWeapon();
            }
        }
        else
        {
            // Mode de visée normal avec la souris
            AimWeapon();

            if (Input.GetMouseButtonDown(0) && currentHealth > 0)
            {
                FireWeapon();
            }
        }
    }

    private void FireWeapon()
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
