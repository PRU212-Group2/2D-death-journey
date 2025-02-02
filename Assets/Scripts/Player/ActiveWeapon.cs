using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO startingWeapon;
    
    WeaponSO currentWeaponSO;
    Weapon currentWeapon;
    PlayerMovement player;
    Animator animator;

    float timeSinceLastShot = 0f;
    int currentAmmo;
    bool shootState;

    void Awake()
    {
        player = GetComponentInParent<PlayerMovement>();
        animator = GetComponentInParent<Animator>();
    }
    
    void Update()
    {
        HandleShoot();
    }
    
    void Start()
    {
        // Equip starting weapon and starting ammo at the beginning of the game
        SwitchWeapon(startingWeapon);
        AdjustAmmo(currentWeaponSO.MagazineSize);
        player.SetRifleMode(startingWeapon.isRifle);
    }

    public void AdjustAmmo(int amount)
    {
        currentAmmo += amount;

        // Restrict ammo to just magazine size
        if (currentAmmo > currentWeaponSO.MagazineSize)
        {
            currentAmmo = currentWeaponSO.MagazineSize;
        }
    }
    
    public void SwitchWeapon(WeaponSO weaponSO)
    {
        // Destroy the currently equipped weapon
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
        }
        
        // Create and equip new weapon
        Weapon newWeapon = Instantiate(weaponSO.WeaponPrefab, transform).GetComponent<Weapon>();
        currentWeapon = newWeapon;
        this.currentWeaponSO = weaponSO;
        
        // Switch magazine size
        AdjustAmmo(currentWeaponSO.MagazineSize);
    }
    
    public void HandleShootInput(bool isPressed)
    {
        shootState = isPressed;
    }
    
    void HandleShoot()
    {
        timeSinceLastShot += Time.deltaTime;
        
        if (!shootState) return;
        
        // Rate of fire (can not shoot until time since last shot is greater than fire rate)
        if (timeSinceLastShot >= currentWeaponSO.FireRate && currentAmmo > 0)
        {
            // Shoot the weapon
            currentWeapon.Shoot(currentWeaponSO);
            
            timeSinceLastShot = 0f;
            AdjustAmmo(-1);
        }

        // Determine if the weapon is rifle/automatic or not (mouse hold or mouse press)
        if (!currentWeaponSO.isRifle)
        {
            shootState = false;
        }
    }

}
