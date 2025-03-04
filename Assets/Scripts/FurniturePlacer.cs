using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FurnitureDropdown m_Dropdown;
    [SerializeField] private LayerMask m_PlacementMask;

    [Header("Settings")]
    [SerializeField] private float m_RaycastDistance = 10f;
    [SerializeField] private Material m_ValidMaterial;
    [SerializeField] private Material m_InvalidMaterial;

    public float GridSize => m_GridSize;
    private float m_GridSize;

    private GameObject m_CurrentPreview;
    private Renderer[] m_PreviewRenderers;
    private bool m_IsValidPosition;

    void Update()
    {
        if (m_Dropdown.CurrSelectedFurniturePrefab == null) return;

        HandlePlacementInput();
        UpdatePreviewPosition();
        CheckPlacementValidity();
        UpdatePreviewMaterials();
    }

    void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0) && m_IsValidPosition)
        {
            PlaceFurniture();
            ClearPreview();
        }
    }

    void UpdatePreviewPosition()
    {
        if (m_CurrentPreview == null)
        {
            CreatePreviewInstance();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, m_RaycastDistance, m_PlacementMask))
        {
            m_CurrentPreview.transform.position = SnapToGrid(hit.point);
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / m_Dropdown.GridSize) * m_Dropdown.GridSize,
            0,
            Mathf.Round(position.z / m_Dropdown.GridSize) * m_Dropdown.GridSize
        );
    }
    void CheckPlacementValidity()
    {
        if (m_CurrentPreview == null) return;

        FurnitureInfo info = m_Dropdown.GetCurrentFurnitureInfo();
        Vector3 center = m_CurrentPreview.transform.position;

        // Combined checks
        bool clearSphere = Physics.OverlapSphere(center,
            info.clearanceRadius,
            info.collisionCheckMask).Length == 0;

        bool clearBox = Physics.OverlapBox(center,
            GetPreviewBounds().extents,
            m_CurrentPreview.transform.rotation,
            info.collisionCheckMask).Length == 0;

        m_IsValidPosition = clearSphere && clearBox;
    }
    void UpdatePreviewMaterials()
    {
        if (m_PreviewRenderers == null) return;

        foreach (Renderer renderer in m_PreviewRenderers)
        {
            renderer.material = m_IsValidPosition ? m_ValidMaterial : m_InvalidMaterial;
        }
    }

    void CreatePreviewInstance()
    {
        m_CurrentPreview = Instantiate(m_Dropdown.CurrSelectedFurniturePrefab);
        m_PreviewRenderers = m_CurrentPreview.GetComponentsInChildren<Renderer>();
        SetPreviewMaterials();
    }

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

    void PlaceFurniture()
    {
        Instantiate(m_Dropdown.CurrSelectedFurniturePrefab,
                  m_CurrentPreview.transform.position,
                  m_CurrentPreview.transform.rotation);
    }

    void ClearPreview()
    {
        Destroy(m_CurrentPreview);
        m_CurrentPreview = null;
        m_PreviewRenderers = null;
    }

    Bounds GetPreviewBounds()
    {
        Collider collider = m_CurrentPreview.GetComponentInChildren<Collider>();
        return collider != null ? collider.bounds :
            m_CurrentPreview.GetComponentInChildren<Renderer>().bounds;
    }
}