using UnityEngine;

public class Furniture : MonoBehaviour
{
    public FurnitureInfo FurnitureInfo {get; private set;}
    
    public void Initialise(FurnitureInfo furnitureInfo)
    {
        FurnitureInfo = furnitureInfo;
    }
}
