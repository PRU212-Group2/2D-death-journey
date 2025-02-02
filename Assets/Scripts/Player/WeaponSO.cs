using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public GameObject WeaponPrefab;
    public GameObject HitVFXPrefab;
    public int Damage = 1;
    public float FireRate = 0.5f;
    public bool isRifle = false;
    public int MagazineSize = 12;
}