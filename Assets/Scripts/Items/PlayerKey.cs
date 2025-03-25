using UnityEngine;

public class PlayerKey : MonoBehaviour
{
    public bool hasKey = false; // Indique si le joueur a la clé

    // Méthode pour donner la clé au joueur
    public void GiveKey()
    {
        hasKey = true;
        Debug.Log("Le joueur a maintenant la clé !");
    }
} 