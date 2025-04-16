using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Door : MonoBehaviour
{
    public string nextSceneName = "Level2";
    public float messageDisplayTime = 2f;
    public TextMeshProUGUI messageText;

    private bool _isPlayerInRange = false;
    private bool _hasKey = false;
    private float _messageTimer = 0f;
    private bool _isShowingMessage = false;

    public AudioClip doorOpenSound;
    private AudioSource audioSource;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        if (!SceneExists(nextSceneName))
        {
            Debug.LogError($"La scène '{nextSceneName}' n'existe pas dans le Build Settings !");
        }

        // Initialize audio source if available
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component is missing on the Door object.");
        }
    }

    void Update()
    {
        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Touche E pressée. Le joueur a la clé : {_hasKey}");

            if (_hasKey)
            {
                Debug.Log($"Tentative de chargement de la scène : {nextSceneName}");
                // Play sound and load scene after sound finishes
                StartCoroutine(LoadNextSceneCoroutine());
            }
            else
            {
                ShowMessage("You need to find the key to open this door");
            }
        }

        if (_isShowingMessage)
        {
            _messageTimer += Time.deltaTime;
            if (_messageTimer >= messageDisplayTime)
            {
                HideMessage();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            Debug.Log("Le joueur est dans la zone de la porte");

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                _hasKey = playerController.HasKey();
                Debug.Log($"État de la clé du joueur : {_hasKey}");
            }
            else
            {
                Debug.LogError("Le composant PlayerController n'a pas été trouvé sur le joueur !");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
            Debug.Log("Le joueur est sorti de la zone de la porte");
        }
    }

    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            _isShowingMessage = true;
            _messageTimer = 0f;
        }
    }

    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
            _isShowingMessage = false;
        }
    }

    // Coroutine that plays the sound, waits, then loads the scene
    IEnumerator LoadNextSceneCoroutine()
    {
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
            yield return new WaitForSeconds(doorOpenSound.length);
        }
        else
        {
            Debug.LogWarning("AudioSource or doorOpenSound missing. Loading scene immediately.");
        }

        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors du chargement de la scène : {e.Message}");
        }
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 2f, 0f));
    }
}
