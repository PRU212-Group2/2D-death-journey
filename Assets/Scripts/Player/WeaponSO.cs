using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public GameObject WeaponPrefab;
    public string Name = "Pistol";
    public int Damage = 1;
    public float FireRate = 0.5f;
    public bool isRifle = false;
    public bool isLaser = false;
    public int MagazineSize = 50;
}