using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureInfoSO", menuName = "ScriptableObject/FurnitureInfoSO")]
public class FurnitureInfo : ScriptableObject
{
    public GameObject FurniturePrefab;
    public Sprite FurnitureSprite;
}