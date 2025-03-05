using UnityEngine;

public class FurnitureRelocator : FurniturePositionHandler
{
    [SerializeField] private LayerMask m_FurnitureLayerMask;

    private FurnitureInfo m_SelectedFurniture;
    private Vector3 m_PreviousLocalPosition;
    private Quaternion m_PreviousLocalRotation;
    private bool m_IsHolding = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_IsHolding = false;
    }

    protected override bool ShouldPreview()
    {
        return base.ShouldPreview() && m_IsHolding;
    }

    protected override FurnitureInfo GetFurnitureInfo()
    {
        return m_SelectedFurniture;
    }

    protected override GameObject GetFurniturePrefab()
    {
        return m_SelectedFurniture.FurniturePrefab;
    }

    protected override void HandleTapBeginInput()
    {
        base.HandleTapBeginInput();

        if (!GridHandler.Instance.PlacementPoseIsValid)
            return;

        Ray ray = Camera.main.ScreenPointToRay(InputHandler.Instance.TouchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_FurnitureLayerMask))
        {
            Furniture furniture = hit.collider.gameObject.GetComponentInParent<Furniture>();

            if (furniture)
            {
                m_SelectedFurniture = furniture.FurnitureInfo;
                m_PreviousLocalPosition = furniture.transform.localPosition;
                m_PreviousLocalRotation = furniture.transform.localRotation;
                Destroy(furniture.gameObject);
                m_IsHolding = true;
            }
        }
    }

    protected override bool ShouldPlaceFurniture()
    {
        return base.ShouldPlaceFurniture() && m_IsHolding;
    }

    protected override void HandlePlacementInput()
    {
        base.HandlePlacementInput();

        m_IsHolding = false;
    }

    protected override void OnCancelPreview()
    {
        base.OnCancelPreview();

        if (m_IsHolding)
            PlaceFurniture(m_PreviousLocalPosition, m_PreviousLocalRotation);

        m_IsHolding = false;
    }
}


