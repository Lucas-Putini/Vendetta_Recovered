using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused = false; // Tracks whether the game is paused

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Press Escape to toggle pause
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused; // Flip pause state
        Time.timeScale = isPaused ? 0f : 1f; // 0 stops time, 1 resumes normal time
    }
}