using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureInfoSO", menuName = "ScriptableObject/FurnitureInfoSO")]
public class FurnitureInfo : ScriptableObject
{
    [Header("Core Assets")]
    public GameObject FurniturePrefab;
    public Sprite FurnitureSprite;
    
    [Header("Collision Settings")]
    public Vector3 collisionBoundsOverride = Vector3.zero;
    public float clearanceRadius = 0.2f;
    public LayerMask collisionCheckMask = -1;
}