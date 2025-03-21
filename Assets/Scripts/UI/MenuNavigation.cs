using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private List<Button> menuButtons = new List<Button>();
    [SerializeField] private Button backButton;
    [SerializeField] private int startingButtonIndex = 0;
    
    private int currentButtonIndex = -1;
    
    void Start()
    {
        // Initialize with first button selected
        if (menuButtons.Count > 0)
        {
            SelectButton(startingButtonIndex);
        }
    }
    
    void Update()
    {
        // Arrow key navigation
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            NavigateNext();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            NavigatePrevious();
        }
        
        // Enter key to click the selected button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (menuButtons.Count > 0 && currentButtonIndex >= 0 && currentButtonIndex < menuButtons.Count)
            {
                // Simulate click on current button
                ExecuteEvents.Execute(menuButtons[currentButtonIndex].gameObject, 
                    new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                
                // Play click sound
                menuButtons[currentButtonIndex].PlayClickSound();
            }
        }
        
        // Escape key to go back
        if (Input.GetKeyDown(KeyCode.Escape) && backButton != null)
        {
            // Simulate click on back button
            ExecuteEvents.Execute(backButton.gameObject, 
                new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                
            // Play back sound
            backButton.PlayBackSound();
        }
    }
    
    private void NavigateNext()
    {
        if (menuButtons.Count == 0)
            return;
        
        // Deselect current button
        DeselectCurrent();
        
        // Move to next button
        currentButtonIndex = (currentButtonIndex + 1) % menuButtons.Count;
        
        // Select new button
        SelectButton(currentButtonIndex);
    }
    
    private void NavigatePrevious()
    {
        if (menuButtons.Count == 0)
            return;
        
        // Deselect current button
        DeselectCurrent();
        
        // Move to previous button
        currentButtonIndex--;
        if (currentButtonIndex < 0)
            currentButtonIndex = menuButtons.Count - 1;
        
        // Select new button
        SelectButton(currentButtonIndex);
    }
    
    private void SelectButton(int index)
    {
        if (index < 0 || index >= menuButtons.Count)
            return;
            
        currentButtonIndex = index;
        
        // Set button as selected
        menuButtons[currentButtonIndex].SetSelected(true);
        
        // Trigger hover sound
        menuButtons[currentButtonIndex].PlayHoverSound();
    }
    
    private void DeselectCurrent()
    {
        if (currentButtonIndex >= 0 && currentButtonIndex < menuButtons.Count)
        {
            menuButtons[currentButtonIndex].SetSelected(false);
        }
    }
    
    // Handle mouse interaction
    void LateUpdate()
    {
        // If we're using mouse, handle when mouse exits the menu area
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            // Make sure we always have a button selected
            if (currentButtonIndex >= 0 && currentButtonIndex < menuButtons.Count)
            {
                menuButtons[currentButtonIndex].SetSelected(true);
            }
        }
    }
}