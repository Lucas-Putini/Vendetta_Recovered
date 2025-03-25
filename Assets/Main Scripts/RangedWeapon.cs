using UnityEngine;

public abstract class RangedWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName;
    public float damage;
    public float fireRate;
    public Transform firePoint;

    protected float lastShotTime;

    public abstract void Fire();
}