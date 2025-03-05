using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance { get; private set; }

    public enum Mode { Place, Relocate, Delete }
    public Mode currentMode = Mode.Place;

    [Header("References")]
    public GameObject selectedFurniturePrefab; 
    public GameObject selectedFurniture;  
    public FurniturePlacer furniturePlacer;
    public FurnitureRelocator furnitureRelocator;
    public FurnitureDeleter furnitureDeleter;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetMode(int mode)
    {
        currentMode = (Mode)mode;
        Debug.Log("🔄 Mode changed to: " + currentMode);

        furniturePlacer.enabled = (currentMode == Mode.Place);
        furnitureRelocator.enabled = (currentMode == Mode.Relocate);
        furnitureDeleter.enabled = (currentMode == Mode.Delete);
    }

    public void SetSelectedFurniturePrefab(GameObject furniturePrefab)
    {
        selectedFurniturePrefab = furniturePrefab;
    }

    public void SetSelectedFurniture(GameObject furnitureInstance)
    {
        selectedFurniture = furnitureInstance;
    }
}




