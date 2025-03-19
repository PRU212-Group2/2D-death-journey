using UnityEngine;

public class StorePickupItem : StoreItem
{
    [SerializeField] string itemName;
    [TextArea]
    [SerializeField] string itemDescription;
    [SerializeField] int itemQuantity;
    [SerializeField] Sprite itemSprite;
    
    InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }
    
    protected override void OnClick()
    {
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(itemName, itemQuantity, itemSprite, itemDescription);
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }
    }
}
