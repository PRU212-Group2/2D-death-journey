[System.Serializable]
public class SaveData
{
    public int cash;
    public int ammo;
    public int currentLevel;
    public ItemData[] items;
    public PlayerData playerData;
    public string weaponName;
}

[System.Serializable]
public class PlayerData
{
    public float[] position;
    public int health;
    public string playerName;
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;
}