using UnityEngine;

[CreateAssetMenu(fileName = "HealthItem", menuName = "Items/Health Item")]
public class HealthItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [Range(0f, 1f)] public float healAmount;
}
