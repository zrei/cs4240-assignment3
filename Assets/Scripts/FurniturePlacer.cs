using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    public Transform spawnPoint; 

    public void PlaceFurniture()
    {
        if (FurnitureManager.Instance.selectedFurniturePrefab != null)
        {
            GameObject newFurniture = Instantiate(
                FurnitureManager.Instance.selectedFurniturePrefab, 
                spawnPoint.position, 
                Quaternion.identity);
            newFurniture.AddComponent<Fruniture>();

        }

    }


}

