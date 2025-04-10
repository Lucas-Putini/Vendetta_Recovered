using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI titleText;

    private DeathMenu deathMenu;

    private void Start()
    {
        // Get reference to death menu
        deathMenu = GetComponent<DeathMenu>();
        if (deathMenu == null)
            Debug.LogError("DeathMenu component not found!");

        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(deathMenu.RestartLevel);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(deathMenu.ReturnToMainMenu);

        // Set initial text
        if (titleText != null)
            titleText.text = "GAME OVER";
    }
} 