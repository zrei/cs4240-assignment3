using Unity.VisualScripting;
using UnityEngine;

public abstract class FurniturePositionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask m_PlacementMask;

    [Header("Settings")]
    [SerializeField] private float m_RaycastDistance = 10f;
    [SerializeField] private Material m_ValidMaterial;
    [SerializeField] private Material m_InvalidMaterial;

    [SerializeField] private Transform m_GridTransform;
    [SerializeField] private float m_MaxGrid;

    [SerializeField] private float m_GridSize;

    #region Preview
    protected GameObject m_CurrentPreview;
    private Renderer[] m_PreviewRenderers;
    private bool m_IsValidPosition = false;
    #endregion

    private bool m_IsTouching = false;

    protected virtual void OnEnable()
    {
        InputHandler.Instance.TapBeginEvent += HandleTapBeginInput;
        InputHandler.Instance.TapCompleteEvent += HandlePlacementInput;

        m_IsValidPosition = false;
        m_IsTouching = false;
    }

    protected virtual void OnDisable()
    {
        InputHandler.Instance.TapBeginEvent -= HandleTapBeginInput;
        InputHandler.Instance.TapCompleteEvent -= HandlePlacementInput;

        OnCancelPreview();
    }

    protected virtual void HandleTapBeginInput() 
    {
        if (MenuButtonHandler.Instance.HadButtonPressThisFrame)
            return;

        m_IsTouching = true;
    }

    private void Update()
    {
        if (!ShouldPreview())
        {
            OnCancelPreview();
            return;
        }

        UpdatePreviewPosition();
        CheckPlacementValidity();
        UpdatePreviewMaterials();
    }

    protected virtual bool ShouldPreview()
    {
        return m_IsTouching && GridHandler.Instance.PlacementPoseIsValid;
    }

    protected virtual void OnCancelPreview()
    {
        if (m_CurrentPreview != null)
            ClearPreview();
    }

    protected virtual void HandlePlacementInput()
    {
        if (ShouldPlaceFurniture())
        {
            PlaceFurniture(SnapToGrid(m_CurrentPreview.transform.localPosition), m_CurrentPreview.transform.localRotation);
            ClearPreview();
        }
        
        m_IsValidPosition = false;
        m_IsTouching = false;
    }

    protected virtual bool ShouldPlaceFurniture()
    {
        return GridHandler.Instance.PlacementPoseIsValid && m_IsValidPosition && m_IsTouching;
    }

    private void UpdatePreviewPosition()
    {
        if (m_CurrentPreview == null)
        {
            CreatePreviewInstance();
        }

        if (GridHandler.Instance.PlacementPoseIsValid)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputHandler.Instance.TouchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, m_RaycastDistance, m_PlacementMask))
            {
                Vector3 localPosition = m_GridTransform.InverseTransformPoint(hit.point);
                m_CurrentPreview.transform.localPosition = SnapToGrid(localPosition);
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 localPosition)
    {
        return new Vector3(
            Mathf.Round(localPosition.x / m_GridSize) * m_GridSize,
            0,
            Mathf.Round(localPosition.z / m_GridSize) * m_GridSize
        );
    }

    private void CheckPlacementValidity()
    {
        FurnitureInfo info = GetFurnitureInfo();
        Vector3 center = m_CurrentPreview.transform.position;
    
        // Combined checks
        bool clearSphere = Physics.OverlapSphere(center,
            m_GridSize / 2 /*info.clearanceRadius*/,
            info.collisionCheckMask).Length == 0;

        bool clearBox = Physics.OverlapBox(center,
            GetPreviewBounds().extents,
            m_CurrentPreview.transform.rotation,
            info.collisionCheckMask).Length == 0;

        m_IsValidPosition = clearSphere /*&& clearBox*/ && Mathf.Abs(m_CurrentPreview.transform.localPosition.x) <= m_MaxGrid && Mathf.Abs(m_CurrentPreview.transform.localPosition.z) <= m_MaxGrid;
    }

    void UpdatePreviewMaterials()
    {
        if (m_PreviewRenderers == null) return;

        foreach (Renderer renderer in m_PreviewRenderers)
        {
            renderer.material = GridHandler.Instance.PlacementPoseIsValid && m_IsValidPosition ? m_ValidMaterial : m_InvalidMaterial;
        }
    }

    protected void CreatePreviewInstance()
    {
        m_CurrentPreview = Instantiate(GetFurniturePrefab(), m_GridTransform);
        m_PreviewRenderers = m_CurrentPreview.GetComponentsInChildren<Renderer>();
        SetPreviewMaterials();
        SetPreviewLayers(m_CurrentPreview.transform);
    }

    private void SetPreviewLayers(Transform transform)
    {
        if (transform == null)
            return;

        transform.gameObject.layer = LayerMask.NameToLayer("Default");

        foreach (Transform child in transform)
            SetPreviewLayers(child);
    }

    protected abstract GameObject GetFurniturePrefab();

    protected abstract FurnitureInfo GetFurnitureInfo();

    void SetPreviewMaterials()
    {
        foreach (Renderer renderer in m_PreviewRenderers)
        {
            Material[] materials = new Material[renderer.materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = m_ValidMaterial;
            }
            renderer.materials = materials;
        }
    }

    protected void PlaceFurniture(Vector3 localPosition, Quaternion localRotation)
    {
        GameObject furniture = Instantiate(GetFurniturePrefab(), m_GridTransform);
        Furniture furnitureComponent = furniture.AddComponent<Furniture>();
        furnitureComponent.Initialise(GetFurnitureInfo());
        furniture.transform.localPosition = localPosition;
        furniture.transform.localRotation = localRotation;
    }

    protected void ClearPreview()
    {
        Destroy(m_CurrentPreview);
        m_CurrentPreview = null;
        m_PreviewRenderers = null;
    }

    private Bounds GetPreviewBounds()
    {
        Collider collider = m_CurrentPreview.GetComponentInChildren<Collider>();
        return collider != null ? collider.bounds :
            m_CurrentPreview.GetComponentInChildren<Renderer>().bounds;
    }
}
