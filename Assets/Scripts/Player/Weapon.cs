using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Projectile projectile;
    [SerializeField] Transform muzzlePoint;
    
    public void Shoot(WeaponSO weaponSO)
    {
        // Play muzzle flash
        muzzleFlash.Play();

        // Create projectile at the position of the muzzle point
        Projectile shotProjectile = Instantiate(projectile, muzzlePoint.position, muzzlePoint.rotation);
        shotProjectile.SetWeaponSO(weaponSO);
    }
}
