using System.Collections.Generic; // <-- Added for inventory
using UnityEngine;
using MagicPigGames;  // Ensure that any base classes like Character are accessible
using TMPro;

public class Player : Character
{
    public RangedWeapon equippedWeapon;
    public Transform aimPivot; // Pivot point for aiming (e.g., the player's hand or gun base)
    public ProgressBar healthBarUI; // Assign this from the Inspector

    // Shooting and death animations
    public Animator animator;

    // So the enemy knows when to stop shooting
    public bool IsDead => currentHealth <= 0;

    // ✅ INVENTORY FIELDS (new)
    public List<HealthItem> inventory = new List<HealthItem>();
    public Transform[] pickupSlots; // Assign 4 empty slot positions in the Inspector
    public Canvas uiCanvas; // This will hold the reference to your screen-space camera canvas

    // Add this new field to track UI items
    public List<GameObject> uiHealthItems = new List<GameObject>();

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

        // ✅ Healing input
        if (Input.GetKeyDown(KeyCode.H))
        {
            TryHeal();
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

    public void AddHealthItem(HealthItem item)
    {
        if (inventory.Count >= 4)
        {
            Debug.Log("Inventory full! Use an item to pick up more.");
            return;
        }

        inventory.Add(item);
        Debug.Log($"Picked up {item.itemName}");

    }
    public TextMeshProUGUI inventoryCounter;

    private void UpdateInventoryUI()
    {
        if (inventoryCounter != null)
            inventoryCounter.text = $"x{inventory.Count}";
    }


    // ✅ Healing logic
    private void TryHeal()
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("Health is already full!");
            return;
        }

        float healthPercent = currentHealth / maxHealth;

        HealthItem red = inventory.Find(i => i.healAmount >= 0.5f);
        HealthItem orange = inventory.Find(i => i.healAmount >= 0.25f && i.healAmount < 0.5f);

        int itemIndex = -1;
        if (healthPercent <= 0.49f && red != null)
        {
            itemIndex = inventory.IndexOf(red);
            Heal(red);
            inventory.Remove(red);
        }
        else if (healthPercent <= 0.6f && orange != null)
        {
            itemIndex = inventory.IndexOf(orange);
            Heal(orange);
            inventory.Remove(orange);
        }
        else if (orange != null)
        {
            itemIndex = inventory.IndexOf(orange);
            Heal(orange);
            inventory.Remove(orange);
        }
        else if (red != null)
        {
            itemIndex = inventory.IndexOf(red);
            Heal(red);
            inventory.Remove(red);
        }
        else
        {
            Debug.Log("No medicine available!");
            return;
        }

        // Remove the UI item
        if (itemIndex >= 0 && itemIndex < uiHealthItems.Count)
        {
            Destroy(uiHealthItems[itemIndex]);
            uiHealthItems.RemoveAt(itemIndex);

            // Reposition remaining items to their proper slots
            for (int i = 0; i < uiHealthItems.Count; i++)
            {
                if (i < pickupSlots.Length)
                {
                    uiHealthItems[i].transform.position = pickupSlots[i].position;
                }
            }
        }

        UpdateInventoryUI();
    }

    // ✅ Healing helper
    private void Heal(HealthItem item)
    {
        float amount = item.healAmount * maxHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (healthBarUI != null)
        {
            healthBarUI.SetProgress(currentHealth / maxHealth);
        }

        Debug.Log($"Used {item.itemName} and healed {amount}. Current Health: {currentHealth}");
    }
}







/*******

protected override void Die()
{
    animator.SetBool("IsDead", true);
    this.enabled = false;
    Destroy(gameObject, 2f); // Adjust time to match your animation duration
    DeathMenu.Instance.ShowDeathMenu();
}
}
***/