using UnityEngine;
using UnityEngine.UI;

public class MeleeEnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar References")]
    [SerializeField] private RectTransform healthBarFill;
    [SerializeField] private Canvas healthBarCanvas;
    
    [Header("Health Bar Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f); // Offset above enemy
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color noHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30% health
    
    private Camera mainCamera;
    private EnemyStats enemyStats;
    private Image fillImage;
    private float originalWidth;
    private RectTransform backgroundRect;
    
    private void Start()
    {
        mainCamera = Camera.main;
        enemyStats = GetComponentInParent<EnemyStats>();
        fillImage = healthBarFill.GetComponent<Image>();
        backgroundRect = healthBarFill.parent as RectTransform;
        
        if (backgroundRect != null)
        {
            originalWidth = backgroundRect.rect.width;
            // Set the fill to start from the left edge
            healthBarFill.anchorMin = new Vector2(0, 0);
            healthBarFill.anchorMax = new Vector2(0, 1);
            healthBarFill.pivot = new Vector2(0, 0.5f);
        }
        
        if (enemyStats == null)
        {
            Debug.LogError("MeleeEnemyHealthBar: No EnemyStats component found in parent!");
            enabled = false;
            return;
        }
        
        // Initialize health bar
        UpdateHealthBar(enemyStats.maxHealth, enemyStats.maxHealth);
        
        if (hideWhenFull)
        {
            healthBarCanvas.enabled = false;
        }
    }
    
    private void LateUpdate()
    {
        if (enemyStats == null) return;
        
        // Update position to follow enemy
        transform.position = enemyStats.transform.position + offset;
        
        // Make health bar face camera
        transform.rotation = mainCamera.transform.rotation;
    }
    
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBarFill == null || fillImage == null) return;
        
        // Calculate fill amount (0 to 1)
        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        
        // Update fill width
        Vector2 sizeDelta = healthBarFill.sizeDelta;
        sizeDelta.x = originalWidth * fillAmount;
        healthBarFill.sizeDelta = sizeDelta;
        
        // Update fill color based on health percentage
        Color newColor = Color.Lerp(noHealthColor, fullHealthColor, fillAmount);
        fillImage.color = newColor;
        
        // Show/hide health bar based on health status
        if (hideWhenFull)
        {
            healthBarCanvas.enabled = currentHealth < maxHealth;
        }
    }
} 