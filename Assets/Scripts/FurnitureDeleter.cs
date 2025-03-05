using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureDeleter : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; 
    }

    void Update()
    {
        // if (FurnitureManager.Instance == null)
        // {
        //     Debug.LogError("FurnitureManager.Instance is NULL! Make sure it's assigned.");
        //     return;
        // }

        if (FurnitureManager.Instance.currentMode != FurnitureManager.Mode.Delete)
            return;


        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            TryDelete(Mouse.current.position.ReadValue());
        }

        if (Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            TryDelete(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    void TryDelete(Vector2 screenPosition)
    {

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        int furnitureLayerMask = LayerMask.GetMask("Furniture");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, furnitureLayerMask))
        {

            if (hit.collider.CompareTag("Furniture"))
            {
                Fruniture furniture = hit.collider.gameObject.GetComponentInParent<Fruniture>();

                if (furniture)
                {
                    Destroy(furniture.gameObject);
                    FurnitureManager.Instance.selectedFurniturePrefab = null;
                }
                
            }
        }

    }
}





