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
        Debug.Log($"[GameSettings] OnSceneLoaded called for scene: {scene.name}");
        
        // Réinitialiser les références UI et cacher le menu des paramètres
        SetupUIReferences();
        FindAndSetupButtons();
        HideSettingsMenu();
    }

    private void Start()
    {
        Debug.Log("[GameSettings] Start called");
        SetupUIReferences();
        FindAndSetupButtons();
        HideSettingsMenu();
    }

    private void FindAndSetupButtons()
    {
        Debug.Log("[GameSettings] Finding and setting up buttons");
        
        // Trouver tous les boutons dans la scène
        Button[] allButtons = FindObjectsOfType<Button>();
        
        foreach (Button button in allButtons)
        {
            // Vérifier le nom du bouton pour déterminer son rôle
            string buttonName = button.gameObject.name.ToLower();
            
            if (buttonName.Contains("settings") && !buttonName.Contains("close"))
            {
                Debug.Log($"[GameSettings] Found Settings button: {button.gameObject.name}");
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ShowSettingsMenu);
                Debug.Log("[GameSettings] Added ShowSettingsMenu listener to Settings button");
            }
            else if (buttonName.Contains("close") || buttonName.Contains("return"))
            {
                Debug.Log($"[GameSettings] Found Close/Return button: {button.gameObject.name}");
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HideSettingsMenu);
                Debug.Log("[GameSettings] Added HideSettingsMenu listener to Close/Return button");
            }
        }
    }

    private void SetupUIReferences()
    {
        Debug.Log("[GameSettings] SetupUIReferences called");
        
        // Trouver les éléments UI par tag
        if (settingsMenuUI == null)
        {
            GameObject settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
            Debug.Log($"[GameSettings] Found settings menu by tag: {settingsMenu != null}");
            if (settingsMenu != null)
            {
                settingsMenuUI = settingsMenu;
                Debug.Log("[GameSettings] SettingsMenuUI assigned successfully");
            }
            else
            {
                Debug.LogError("[GameSettings] SettingsMenuUI not found by tag. Make sure the Canvas has the 'SettingsMenu' tag");
                // List all objects with tags for debugging
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                Debug.Log("[GameSettings] All objects with tags:");
                foreach (GameObject obj in allObjects)
                {
                    if (!string.IsNullOrEmpty(obj.tag))
                    {
                        Debug.Log($"[GameSettings] Object: {obj.name}, Tag: {obj.tag}");
                    }
                }
            }
        }
        
        if (mainMenuUI == null)
        {
            GameObject mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
            Debug.Log($"[GameSettings] Found main menu by tag: {mainMenu != null}");
            if (mainMenu != null)
            {
                mainMenuUI = mainMenu;
                Debug.Log("[GameSettings] MainMenuUI assigned successfully");
            }
            else
            {
                Debug.LogError("[GameSettings] MainMenuUI not found by tag. Make sure the Canvas has the 'MainMenu' tag");
            }
        }

        // Configurer les toggles
        if (highContrastToggle == null && settingsMenuUI != null)
        {
            highContrastToggle = settingsMenuUI.GetComponentInChildren<Toggle>();
            if (highContrastToggle != null)
            {
                Debug.Log("High contrast toggle found");
            }
            else
            {
                Debug.LogWarning("High contrast toggle not found in settings menu");
            }
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
                    Debug.Log("Auto-aim toggle found");
                    break;
                }
            }
            
            if (autoAimToggle == null)
            {
                Debug.LogWarning("Auto-aim toggle not found. Make sure it contains 'AutoAim' in its name");
            }
        }

        // Configurer les listeners
        if (highContrastToggle != null)
        {
            highContrastToggle.isOn = highContrastMode;
            highContrastToggle.onValueChanged.RemoveAllListeners();
            highContrastToggle.onValueChanged.AddListener(SetHighContrastMode);
            Debug.Log("High contrast toggle listener set up");
        }

        if (autoAimToggle != null)
        {
            autoAimToggle.isOn = autoAimMode;
            autoAimToggle.onValueChanged.RemoveAllListeners();
            autoAimToggle.onValueChanged.AddListener(SetAutoAimMode);
            Debug.Log("Auto-aim toggle listener set up");
        }
    }

    public void ShowSettingsMenu()
    {
        Debug.Log("[GameSettings] ShowSettingsMenu called");
        
        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(true);
            Debug.Log("[GameSettings] Settings menu activated");
        }
        else
        {
            Debug.LogError("[GameSettings] Settings menu UI reference is null!");
            // Try to find it again
            SetupUIReferences();
            if (settingsMenuUI != null)
            {
                settingsMenuUI.SetActive(true);
                Debug.Log("[GameSettings] Settings menu activated after retry");
            }
        }
        
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
            Debug.Log("[GameSettings] Main menu deactivated");
        }
        else
        {
            Debug.LogError("[GameSettings] Main menu UI reference is null!");
        }
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