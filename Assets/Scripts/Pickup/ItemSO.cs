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
            return true;
        }
        else if (statToChange == StatToChange.ammo)
        {
            ActiveWeapon activeWeapon = FindFirstObjectByType<ActiveWeapon>();

            // If the magazine is full then do not add ammo
            if (activeWeapon.GetAmmo() >= activeWeapon.GetMagazineSize())
            {
                return false;
            }
            
            activeWeapon.AdjustAmmo(amountToChange);
            return true;
        }

        return false;
    }

    public enum StatToChange
    {
        none,
        health,
        speed,
        ammo
    }
}
