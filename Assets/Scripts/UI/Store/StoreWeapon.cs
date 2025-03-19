using UnityEngine;

public class StoreWeapon : StoreItem
{
    [SerializeField] WeaponSO weaponSO;
    ActiveWeapon activeWeapon;
    PlayerMovement playerMovement;

    void Start()
    {
        activeWeapon = FindFirstObjectByType<ActiveWeapon>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }
    
    protected override void OnClick()
    {
        // Switch weapon
        activeWeapon.SwitchWeapon(weaponSO);
        playerMovement.SetNewWeapon();
        playerMovement.SetAnimationMode();
    }
}
