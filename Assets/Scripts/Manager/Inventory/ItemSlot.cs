using TMPro;
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
    [SerializeField] TMP_Text quantityText;
    [SerializeField] Image itemImage;

    [Header("Item Description Slot")]
    [SerializeField] Image itemDescriptionImage;
    [SerializeField] TextMeshProUGUI itemDescriptionNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    
    public GameObject selectedShader;
    
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

    private void OnRightClick()
    {
        throw new System.NotImplementedException();
    }

    private void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        itemDescriptionNameText.text = itemName;
        itemDescriptionImage.sprite = itemSprite;
        itemDescriptionText.text = itemDescription;
        if (itemDescriptionImage.sprite == null) 
            itemDescriptionImage.sprite = emptySprite;
    }
}
