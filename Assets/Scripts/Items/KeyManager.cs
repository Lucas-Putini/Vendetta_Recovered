using UnityEngine;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    private HashSet<string> _collectedKeys = new HashSet<string>();

    public void AddKey(string keyID)
    {
        _collectedKeys.Add(keyID);
        Debug.Log($"Clé {keyID} collectée !");
    }

    public bool HasKey(string keyID)
    {
        return _collectedKeys.Contains(keyID);
    }

    public void RemoveKey(string keyID)
    {
        _collectedKeys.Remove(keyID);
        Debug.Log($"Clé {keyID} utilisée !");
    }

    public int GetKeyCount()
    {
        return _collectedKeys.Count;
    }

    // Méthode pour afficher les clés collectées dans la console (utile pour le débogage)
    public void DisplayCollectedKeys()
    {
        Debug.Log("Clés collectées :");
        foreach (string key in _collectedKeys)
        {
            Debug.Log($"- {key}");
        }
    }
} 