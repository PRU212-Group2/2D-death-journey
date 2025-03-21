using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public RuntimeAnimatorController animator;
    public string Name = "Adam";
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public int playerHealth = 100;
    public int lowHealthThreshold = 30;
}