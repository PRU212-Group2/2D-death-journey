using UnityEngine;

public class MoonlightTrap : Hazard
{
    protected override void OnContact()
    {
        Debug.Log("Player touched the trap!");
    }
}
