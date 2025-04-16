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
        if (uiHealthItems.Count >= 4)
        {
            Debug.Log("Inventory full! Use an item to pick up more.");
            return;
        }

        // Add to inventory
        inventory.Add(item);

        // Create UI representation
        GameObject uiItem = new GameObject(item.itemName + "_UI");
        RectTransform rectTransform = uiItem.AddComponent<RectTransform>();
        uiItem.transform.SetParent(pickupSlots[uiHealthItems.Count], false);
        
        // Set the RectTransform properties
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(50, 50); // Set appropriate size
        rectTransform.localScale = Vector3.one;

        // Add UI Image component instead of SpriteRenderer
        UnityEngine.UI.Image image = uiItem.AddComponent<UnityEngine.UI.Image>();
        image.sprite = item.icon;
        image.preserveAspect = true;

        // Add to UI items list
        uiHealthItems.Add(uiItem);

        // Update counter
        UpdateInventoryUI();
        
        Debug.Log($"Picked up {item.itemName} - Total items: {uiHealthItems.Count}/4");
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

        if (inventory.Count == 0)
        {
            Debug.Log("No health items available!");
            return;
        }

        float healthPercent = currentHealth / maxHealth;
        int itemIndex = -1;
        HealthItem itemToUse = null;

        // Find appropriate item based on health percentage
        if (healthPercent <= 0.49f)
        {
            // Try to find red item (50% heal) first when health is low
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].healAmount >= 0.5f)
                {
                    itemToUse = inventory[i];
                    itemIndex = i;
                    break;
                }
            }
        }

        // If no red item found or health > 49%, try green item (25% heal)
        if (itemToUse == null)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].healAmount == 0.25f)
                {
                    itemToUse = inventory[i];
                    itemIndex = i;
                    break;
                }
            }
        }

        if (itemToUse != null)
        {
            // Apply healing
            Heal(itemToUse);
            
            // Remove from inventory
            inventory.RemoveAt(itemIndex);

            // Remove UI representation
            if (itemIndex >= 0 && itemIndex < uiHealthItems.Count)
            {
                // Destroy the UI object
                Destroy(uiHealthItems[itemIndex]);
                uiHealthItems.RemoveAt(itemIndex);

                // Reposition remaining items
                for (int i = 0; i < uiHealthItems.Count; i++)
                {
                    RectTransform rt = uiHealthItems[i].GetComponent<RectTransform>();
                    uiHealthItems[i].transform.SetParent(pickupSlots[i], false);
                    if (rt != null)
                    {
                        rt.anchoredPosition = Vector2.zero;
                        rt.sizeDelta = new Vector2(50, 50);
                        rt.localScale = Vector3.one;
                    }
                }
            }

            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("No appropriate medicine available!");
        }
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