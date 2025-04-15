using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Add this for Button type

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuUI; // Reference to the death menu canvas
    public Button restartButton;   // Reference to the restart button
    public Button menuButton;      // Reference to the main menu button
    
    private static DeathMenu instance;
    public static DeathMenu Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"DeathMenu marked as DontDestroyOnLoad: {gameObject.name}");
        }
        else
        {
            Debug.Log($"Destroying duplicate DeathMenu: {gameObject.name}");
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
        // Find and setup UI elements in the new scene
        SetupUIReferences();
        HideDeathMenu();
    }

    private void SetupUIReferences()
    {
        // Find the death menu UI in the scene
        if (deathMenuUI == null)
        {
            deathMenuUI = GameObject.Find("DeathMenuUI");
            if (deathMenuUI == null)
            {
                Debug.LogError("DeathMenuUI not found in scene!");
                return;
            }
        }

        // Find and setup buttons
        Button[] buttons = deathMenuUI.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name.Contains("Restart"))
            {
                restartButton = button;
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartLevel);
            }
            else if (button.name.Contains("Menu"))
            {
                menuButton = button;
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(ReturnToMainMenu);
            }
        }

        if (restartButton == null || menuButton == null)
        {
            Debug.LogError("Buttons not found in DeathMenuUI!");
        }
    }

    private void Start()
    {
        SetupUIReferences();
        HideDeathMenu();
    }

    public void ShowDeathMenu()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    private void HideDeathMenu()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(false);
            Time.timeScale = 1f; // Resume normal time
        }
    }

    public void RestartLevel()
    {
        HideDeathMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        HideDeathMenu();
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your main menu scene name
    }
} 