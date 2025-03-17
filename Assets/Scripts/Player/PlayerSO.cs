using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public AnimatorController animator;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public int playerHealth = 100;
    public int lowHealthThreshold = 30;
}