using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Item Data")]
    public string itemName;
    public string itemDescription;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public Sprite emptySprite;

    [SerializeField] int maxNumberOfItems = 9;
    
    [Header("Item Slot")]
    [SerializeField] public TMP_Text quantityText;
    [SerializeField] public Image itemImage;

    [Header("Item Description Slot")]
    [SerializeField] Image itemDescriptionImage;
    [SerializeField] TextMeshProUGUI itemDescriptionNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    
    public GameObject selectedShader;
    public bool itemSelected;
    
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }
    
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (isFull)
            return quantity;
        
        // Update item slot
        this.itemSprite = itemSprite;
        itemImage.sprite = this.itemSprite;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        
        // Update quantity
        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString("00");
            quantityText.gameObject.SetActive(true);
            isFull = true;
            
            // Return left over items
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }
        
        // Update quantity text
        quantityText.text = this.quantity.ToString("00");
        quantityText.gameObject.SetActive(true);

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Stop the event from propagating to other systems
        eventData.Use();
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    //======== DROP ITEM ==========//
    private void OnRightClick()
    {
        RemoveItem();
    }

    public void RemoveItem()
    {
        quantity -= 1;
        quantityText.text = quantity.ToString("00");
        if (quantity <= 0) EmptySlot();
    }

    //======== ITEM SLOT SELECTED ==========//
    private void OnLeftClick()
    {
        if (itemSelected)
        {
            bool usable = inventoryManager.UseItem(itemName);
            
            // Check if the item is usable
            if (usable)
            {
                quantity -= 1;
                quantityText.text = quantity.ToString("00");
                inventoryManager.PlayItemSound();
                
                if (quantity <= 0)
                {
                    EmptySlot();
                }
            }
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            itemSelected = true;
            itemDescriptionNameText.text = itemName;
            itemDescriptionImage.sprite = itemSprite;
            itemDescriptionText.text = itemDescription;
            if (itemDescriptionImage.sprite == null) 
                itemDescriptionImage.sprite = emptySprite;
        }
    }

    //======== EMPTY SLOT IF USED ALL ITEMS ==========//
    public void EmptySlot()
    {
        quantityText.gameObject.SetActive(false);
        itemImage.sprite = emptySprite;
        itemSprite = emptySprite;
        
        itemDescriptionNameText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        itemDescriptionText.text = "";
    }
}
