using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO startingWeapon;
    
    WeaponSO currentWeaponSO;
    Weapon currentWeapon;

    float timeSinceLastShot = 0f;
    int currentAmmo;
    bool shootState;
    AudioPlayer audioPlayer;
    InventoryManager inventoryManager;
    InteractableStore store;
    UIGameplay UIGameplay;

    void Update()
    {
        if (inventoryManager.menuActivated) return;
        if (store.isInteracting) return;
        if (UIGameplay.isPaused) return;
        
        HandleShoot();
    }
    
    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        store = FindFirstObjectByType<InteractableStore>();
        UIGameplay = FindFirstObjectByType<UIGameplay>();

        if (currentWeapon == null)
        {
            // Equip starting weapon and starting ammo at the beginning of the game
            SwitchWeapon(startingWeapon);
        }
        
        AdjustAmmo(currentWeaponSO.MagazineSize);
    }

    public int GetAmmo()
    {
        return currentAmmo;
    }

    public int GetMagazineSize()
    {
        return currentWeaponSO.MagazineSize;
    }
    
    public bool IsRifle()
    {
        return currentWeaponSO.isRifle;
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
        currentWeaponSO = weaponSO;
        
        // Switch magazine size
        AdjustAmmo(currentWeaponSO.MagazineSize);
    }
    
    public void HandleShootInput(bool isPressed)
    {
        shootState = isPressed;
        
        // Stop rifle sound immediately when button is released
        if (!isPressed && currentWeaponSO.isRifle)
        {
            audioPlayer.StopRifleShootingSound();
        }
    }
    
    void HandleShoot()
    {
        timeSinceLastShot += Time.deltaTime;

        if (!shootState)
        {
            return;
        }

        // Start the rifle sound as soon as shoot is pressed for rifles
        if (shootState && currentWeaponSO.isRifle && currentAmmo > 0)
        {
            audioPlayer.StartRifleShootingSound();
        }

        // Rate of fire (can not shoot until time since last shot is greater than fire rate)
        if (timeSinceLastShot >= currentWeaponSO.FireRate && currentAmmo > 0)
        {
            // Shoot the weapon
            currentWeapon.Shoot(currentWeaponSO);
            
            timeSinceLastShot = 0f;
            AdjustAmmo(-1);
            
            // Play pistol shooting audio only for non-rifles
            if (!currentWeaponSO.isRifle)
            {
                audioPlayer.PlayPistolClip();
                shootState = false;
            }
        }
        
        // Stop rifle sound if we're out of ammo
        if (currentWeaponSO.isRifle && currentAmmo <= 0)
        {
            audioPlayer.StopRifleShootingSound();
        }
    }

    public void LoadAmmo(int amount)
    {
        currentAmmo = amount;
    }

    public string GetCurrentWeapon()
    {
        return currentWeaponSO.Name;
    }
}