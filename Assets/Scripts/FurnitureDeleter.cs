using UnityEngine;

public class FurnitureDeleter : MonoBehaviour
{
    [SerializeField] private LayerMask m_FurnitureLayerMask;

    private void OnEnable()
    {
        InputHandler.Instance.TapBeginEvent += TryDelete;
    }

    private void OnDisable()
    {
        InputHandler.Instance.TapBeginEvent -= TryDelete;
    }

    private void TryDelete()
    {
        if (MenuButtonHandler.Instance.HadButtonPressThisFrame)
            return;

        if (!GridHandler.Instance.PlacementPoseIsValid)
            return;

        Ray ray = Camera.main.ScreenPointToRay(InputHandler.Instance.TouchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_FurnitureLayerMask))
        {
            Furniture furniture = hit.collider.gameObject.GetComponentInParent<Furniture>();

            if (furniture)
            {
                Destroy(furniture.gameObject);
            }
        }
    }
}





