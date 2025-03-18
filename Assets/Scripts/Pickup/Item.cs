using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : MonoBehaviour
{
    const string playerString = "Player";
    
    [SerializeField] string itemName;
    [TextArea]
    [SerializeField] string itemDescription;
    [SerializeField] int itemQuantity;
    [SerializeField] Sprite itemSprite;
    
    // Movement parameters
    [SerializeField] float bobSpeed = 1.0f;
    [SerializeField] float bobHeight = 0.2f;
    
    // To store the initial position
    Vector3 startPosition;
    bool positionInitialized = false;
    AudioPlayer audioPlayer;
    
    // Reference to the inventory manager
    InventoryManager inventoryManager;

    void Awake()
    {
        // Get the inventory manager reference
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        
        // Get audio player reference
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
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
            OnPickup();
        }
    }

    void OnPickup()
    {
        audioPlayer.PlayInventoryClip();

        if (inventoryManager != null)
        {
            int leftOverItems = inventoryManager.AddItem(itemName, itemQuantity, itemSprite, itemDescription);

            if (leftOverItems <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                itemQuantity = leftOverItems;
            }
        }
        else
        {
            Debug.LogWarning("InventoryManager not found!");
        }
    }
}