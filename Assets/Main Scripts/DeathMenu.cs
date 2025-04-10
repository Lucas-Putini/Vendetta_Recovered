using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private GameObject deathMenuUI;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // Hide the death menu at start
        if (deathMenuUI != null)
            deathMenuUI.SetActive(false);
    }

    // Called when the player dies
    public void ShowDeathMenu()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    // Restart the current level
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Return to main menu
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(mainMenuSceneName);
    }
} 