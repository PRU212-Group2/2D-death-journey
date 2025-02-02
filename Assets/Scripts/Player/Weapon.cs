using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform muzzlePoint;

    public void Shoot(WeaponSO weaponSO)
    {
        // Play muzzle flash
        muzzleFlash.Play();
            
        // Create projectile at the position of the gun
        Instantiate(projectile, muzzlePoint.position, transform.rotation);
    }
}
