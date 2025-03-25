using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Door : MonoBehaviour
{
    public string nextSceneName = "Level2"; // Nom de la scène suivante
    public float messageDisplayTime = 2f; // Temps d'affichage du message en secondes
    public TextMeshProUGUI messageText; // Référence au texte à afficher

    private bool _isPlayerInRange = false;
    private bool _hasKey = false;
    private float _messageTimer = 0f;
    private bool _isShowingMessage = false;

    void Start()
    {
        // Désactiver le texte au démarrage
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        // Vérifier si la scène existe
        if (!SceneExists(nextSceneName))
        {
            Debug.LogError($"La scène '{nextSceneName}' n'existe pas dans le Build Settings !");
        }
    }

    void Update()
    {
        // Si le joueur est dans la zone et appuie sur E
        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Touche E pressée. Le joueur a la clé : {_hasKey}");
            
            if (_hasKey)
            {
                Debug.Log($"Tentative de chargement de la scène : {nextSceneName}");
                // Téléporter le joueur au début du niveau suivant
                LoadNextScene();
            }
            else
            {
                // Afficher le message
                ShowMessage("You need to find the key to open this door");
            }
        }

        // Gérer le timer du message
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
            
            // Vérifier si le joueur a la clé
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

    void LoadNextScene()
    {
        Debug.Log($"Chargement de la scène : {nextSceneName}");
        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors du chargement de la scène : {e.Message}");
        }
    }

    // Vérifie si une scène existe dans le Build Settings
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

    // Méthode pour dessiner des gizmos dans l'éditeur
    void OnDrawGizmosSelected()
    {
        // Dessiner la zone de la porte
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 2f, 0f));
    }
} 