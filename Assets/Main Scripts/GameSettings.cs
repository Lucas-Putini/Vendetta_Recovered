using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("High Contrast Settings")]
    public bool highContrastMode = false;
    public Color highContrastBackgroundColor = Color.black;
    public Color highContrastForegroundColor = Color.white;
    public Color highContrastAccentColor = Color.yellow;

    [Header("UI References")]
    public GameObject settingsMenuUI;
    public GameObject mainMenuUI;
    public Toggle highContrastToggle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize UI elements
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
        
        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);

        // Setup toggle listener
        if (highContrastToggle != null)
        {
            highContrastToggle.isOn = highContrastMode;
            highContrastToggle.onValueChanged.AddListener(SetHighContrastMode);
        }
    }

    public void ShowSettingsMenu()
    {
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(true);
        
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
        
        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);
    }

    public void SetHighContrastMode(bool enabled)
    {
        highContrastMode = enabled;
        ApplyHighContrastSettings();
    }

    private void ApplyHighContrastSettings()
    {
        // Find all UI elements that need to be updated
        var allUIElements = FindObjectsOfType<Graphic>();
        
        foreach (var element in allUIElements)
        {
            if (element is Text || element is Image)
            {
                if (highContrastMode)
                {
                    // Apply high contrast colors based on element type
                    if (element is Text)
                    {
                        element.color = highContrastForegroundColor;
                    }
                    else if (element is Image)
                    {
                        // Check if it's a background or foreground element
                        if (element.gameObject.name.Contains("Background"))
                        {
                            element.color = highContrastBackgroundColor;
                        }
                        else
                        {
                            element.color = highContrastForegroundColor;
                        }
                    }
                }
                else
                {
                    // Reset to default colors
                    if (element is Text)
                    {
                        element.color = Color.white;
                    }
                    else if (element is Image)
                    {
                        element.color = Color.white;
                    }
                }
            }
        }
    }
} 