using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = new StatToChange();
    public int amountToChange;
    
    public bool UseItem()
    {
        if (statToChange == StatToChange.health)
        {
            PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
            
            // Prevent healing if the player is already at full health
            if (playerHealth.GetHealth() == playerHealth.GetMaxHealth())
            {
                return false;
            }
            
            playerHealth.Heal(amountToChange);
            return true;
        }
        else if (statToChange == StatToChange.speed)
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            
            playerMovement.ApplySpeedBoost();
        }
        else if (statToChange == StatToChange.pistolAmmo)
        {
            ActiveWeapon activeWeapon = FindFirstObjectByType<ActiveWeapon>();
            
            // Prevent adding pistol ammo if the player currently does not equip pistol
            if (activeWeapon.IsRifle()) return false;
            activeWeapon.AdjustAmmo(amountToChange);
        }
        else if (statToChange == StatToChange.rifleAmmo)
        {
            ActiveWeapon activeWeapon = FindFirstObjectByType<ActiveWeapon>();
            
            // Prevent adding rifle ammo if the player currently does not equip rifle
            if (!activeWeapon.IsRifle()) return false;
            activeWeapon.AdjustAmmo(amountToChange);
        }

        return false;
    }

    public enum StatToChange
    {
        none,
        health,
        speed,
        pistolAmmo,
        rifleAmmo
    }
}
