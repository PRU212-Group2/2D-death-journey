using UnityEngine;

public class StoreArmor : StoreItem
{
    [SerializeField] PlayerSO playerSO;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }
    
    protected override void OnClick()
    {
        // Upgrade player
        playerMovement.SwitchPlayer(playerSO);
        playerHealth.SwitchPlayer(playerSO);
    }
}
