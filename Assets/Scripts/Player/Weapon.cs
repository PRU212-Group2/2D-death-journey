using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform muzzlePoint;

    // The Pool Instance
    private ObjectPool<Projectile> _bulletPool;

    // Cache the player transform so we don't have to Find it every frame
    private Transform _playerTransform;

    private void Start()
    {
        // Initialize the Pool with 20 bullets
        _bulletPool = new ObjectPool<Projectile>(projectilePrefab, 20, transform);

        // Find player once (or get it from parent if weapon is attached to player)
        // Assuming Weapon is child of Player, otherwise keep your Find logic here
        _playerTransform = transform.root;
    }

    public void Shoot(WeaponSO weaponSO)
    {
        if (muzzleFlash != null) muzzleFlash.Play();

        // GET from pool (Instead of Instantiate)
        Projectile shotProjectile = _bulletPool.Get();

        // Set Position/Rotation (Since pool objects retain old positions)
        shotProjectile.transform.position = muzzlePoint.position;
        shotProjectile.transform.rotation = muzzlePoint.rotation;

        // INIT the bullet (Pass the logic here)
        // We get direction from the player's scale (facing right vs left)
        float direction = _playerTransform.localScale.x;

        shotProjectile.Init(_bulletPool, direction, weaponSO.Damage);
    }
}
