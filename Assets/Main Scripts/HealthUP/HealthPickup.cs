using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public HealthItem healthItem;
    public float finalScale = 0.5f;
    private bool isCollected = false;
    private static readonly float MIN_COLLECTION_INTERVAL = 0.1f;
    private static float lastCollectionTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        Player player = other.GetComponent<Player>();
        if (player != null && Time.time - lastCollectionTime >= MIN_COLLECTION_INTERVAL)
        {
            if (player.inventory.Count >= player.pickupSlots.Length)
            {
                Debug.Log("Inventory full! Cannot pick up more items.");
                return;
            }

            isCollected = true;
            lastCollectionTime = Time.time;

            // Add to inventory first
            player.AddHealthItem(healthItem);

            // Create UI item directly in slot
            CreateUIItem(player);

            // Destroy the pickup immediately
            Destroy(gameObject);
        }
    }

    private void CreateUIItem(Player player)
    {
        int index = player.inventory.Count - 1;
        if (index >= player.pickupSlots.Length)
            index = player.pickupSlots.Length - 1;

        RectTransform slotUI = player.pickupSlots[index] as RectTransform;
        if (slotUI == null)
        {
            Debug.LogError("Pickup slot is not a RectTransform!");
            return;
        }

        // Create UI element
        GameObject itemUI = new GameObject("HealthItemUI");
        RectTransform rectTransform = itemUI.AddComponent<RectTransform>();
        itemUI.transform.SetParent(player.uiCanvas.transform, false);

        // Add the UI image component
        UnityEngine.UI.Image itemImage = itemUI.AddComponent<UnityEngine.UI.Image>();
        itemImage.sprite = healthItem.icon;
        itemImage.preserveAspect = true;

        // Set size and scale
        rectTransform.sizeDelta = new Vector2(50, 50);
        rectTransform.localScale = Vector3.one * finalScale;

        // Set position directly to slot position
        itemUI.transform.position = slotUI.position;

        // Add to player's UI items list
        if (player.uiHealthItems == null)
            player.uiHealthItems = new List<GameObject>();
        player.uiHealthItems.Add(itemUI);
    }
}