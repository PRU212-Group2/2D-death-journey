using System;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    const string playerString = "Player";
    
    [SerializeField] string pickupName;
    [TextArea]
    [SerializeField] string itemDescription;
    [SerializeField] int pickupQuantity;
    [SerializeField] Sprite pickupSprite;
    
    // Movement parameters
    [SerializeField] float bobSpeed = 1.0f;
    [SerializeField] float bobHeight = 0.2f;
    
    // To store the initial position
    private Vector3 startPosition;
    private bool positionInitialized = false;
    
    // Reference to the inventory manager
    InventoryManager inventoryManager;

    void Awake()
    {
        // Get the inventory manager reference
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }
    
    void Start()
    { 
        // Store the initial position
        startPosition = transform.position;
        positionInitialized = true;
    }
    
    void Update()
    {
        if (!positionInitialized)
        {
            // Ensure we have the correct starting position
            startPosition = transform.position;
            positionInitialized = true;
        }
        
        // Apply up/down movement
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerString))
        {
            if (inventoryManager != null)
            {
                int leftOverItems = inventoryManager.AddItem(pickupName, pickupQuantity, pickupSprite, itemDescription);

                if (leftOverItems <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    pickupQuantity = leftOverItems;
                }
            }
            else
            {
                Debug.LogWarning("InventoryManager not found!");
            }
            
            OnPickup();
        }
    }
    
    protected abstract void OnPickup();
}