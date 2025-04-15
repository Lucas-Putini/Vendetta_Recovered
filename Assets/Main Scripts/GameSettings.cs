using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("High Contrast Settings")]
    public bool highContrastMode = false;
    public Color highContrastBackgroundColor = Color.black;
    public Color highContrastForegroundColor = Color.white;
    public Color highContrastAccentColor = Color.yellow;

    [Header("Accessibility Settings")]
    public bool autoAimMode = false;
    public KeyCode autoAimKey = KeyCode.Space;

    [Header("UI References")]
    public GameObject settingsMenuUI;
    public GameObject mainMenuUI;
    public Toggle highContrastToggle;
    public Toggle autoAimToggle;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Réinitialiser les références UI et cacher le menu des paramètres
        SetupUIReferences();
        HideSettingsMenu();
    }

    private void Start()
    {
        SetupUIReferences();
        HideSettingsMenu();
    }

    private void SetupUIReferences()
    {
        // Trouver les éléments UI dans la scène
        if (settingsMenuUI == null)
        {
            settingsMenuUI = GameObject.Find("SettingsMenuUI");
        }
        
        if (mainMenuUI == null)
        {
            mainMenuUI = GameObject.Find("MainMenuUI");
        }

        // Configurer les toggles
        if (highContrastToggle == null && settingsMenuUI != null)
        {
            highContrastToggle = settingsMenuUI.GetComponentInChildren<Toggle>();
        }

        if (autoAimToggle == null && settingsMenuUI != null)
        {
            // Chercher le toggle d'auto-aim
            Toggle[] toggles = settingsMenuUI.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                if (toggle.gameObject.name.Contains("AutoAim"))
                {
                    autoAimToggle = toggle;
                    break;
                }
            }
        }

        // Configurer les listeners
        if (highContrastToggle != null)
        {
            highContrastToggle.isOn = highContrastMode;
            highContrastToggle.onValueChanged.RemoveAllListeners();
            highContrastToggle.onValueChanged.AddListener(SetHighContrastMode);
        }

        if (autoAimToggle != null)
        {
            autoAimToggle.isOn = autoAimMode;
            autoAimToggle.onValueChanged.RemoveAllListeners();
            autoAimToggle.onValueChanged.AddListener(SetAutoAimMode);
        }
    }

    public void ShowSettingsMenu()
    {
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(true);
        
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);
    }

    public void HideSettingsMenu()
    {
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
        
        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        HideSettingsMenu();
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

    public void SetAutoAimMode(bool enabled)
    {
        autoAimMode = enabled;
    }
} 