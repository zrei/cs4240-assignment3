using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

public class FurnitureRelocator : MonoBehaviour
{
    private GameObject selectedFurniture;
    private bool isHolding = false;
    private ARRaycastManager arRaycastManager;
    private Camera mainCamera;
    private Vector2 touchPosition;

    void Start()
    {
        arRaycastManager = FindFirstObjectByType<ARRaycastManager>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (FurnitureManager.Instance.currentMode != FurnitureManager.Mode.Relocate)
            return;

        if (!isHolding)
        {
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                TrySelectFurniture(Mouse.current.position.ReadValue());
            }
            if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
            {
                TrySelectFurniture(Touchscreen.current.primaryTouch.position.ReadValue());
            }
        }
        else
        {
            MoveFurnitureWithTouch();
        }
    }

    void TrySelectFurniture(Vector2 screenPosition)
    {
        if (mainCamera == null)
        {
            Debug.LogError("❌ No Camera Assigned!");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        int furnitureLayerMask = LayerMask.GetMask("Furniture");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, furnitureLayerMask))
        {
            Debug.Log("🎯 Raycast Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Furniture"))
            {
                selectedFurniture = hit.collider.gameObject;
                isHolding = true;
                Debug.Log("✅ Picked up: " + selectedFurniture.name);
            }
        }
        else
        {
            Debug.Log("❌ Raycast hit nothing. Check layers and colliders.");
        }
    }

    void MoveFurnitureWithTouch()
    {
        if (selectedFurniture == null) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
            {
                Pose hitPose = hits[0].pose;
                selectedFurniture.transform.position = hitPose.position;
            }

            if (touch.phase == UnityEngine.TouchPhase.Ended)
            {
                PlaceFurniture();
            }
        }
    }

    public void PlaceFurniture()
    {
        if (selectedFurniture != null)
        {
            isHolding = false;
            Debug.Log("✅ Placed: " + selectedFurniture.name);
            selectedFurniture = null;
        }
    }
}


