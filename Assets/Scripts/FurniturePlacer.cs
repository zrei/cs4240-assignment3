using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FurnitureDropdown m_Dropdown;
    [SerializeField] private LayerMask m_PlacementMask;

    [Header("Settings")]
    [SerializeField] private float m_RaycastDistance = 10f;
    [SerializeField] private Material m_ValidMaterial;
    [SerializeField] private Material m_InvalidMaterial;

    [SerializeField] private InputActionReference m_TouchPositionActionReference;
    [SerializeField] private InputActionReference m_InputActionReference;
    [SerializeField] private Transform m_GridTransform;

    public float GridSize => m_GridSize;
    private float m_GridSize;

    private GameObject m_CurrentPreview;
    private Renderer[] m_PreviewRenderers;
    private bool m_IsValidPosition = false;

    private bool m_HadPlacementInput = false;

    private Vector2 m_ScreenSpacePosition;

    void Update()
    {
        if (m_Dropdown.CurrSelectedFurnitureInfo == null) return;

        HandlePlacementInput();
        
        if (m_CurrentPreview != null && (!m_InputActionReference.action.IsInProgress() || !GridHandler.Instance.PlacementPoseIsValid))
        {
            ClearPreview();
            m_IsValidPosition = false;
        }
        else
        {
            UpdatePreviewPosition();
            CheckPlacementValidity();
            UpdatePreviewMaterials();
        }
    }

    void Start()
    {
        m_InputActionReference.action.performed += TapInput;
        m_TouchPositionActionReference.action.performed += TouchPositionInput;

        m_Dropdown.OnSwitchFurnitureEvent += OnSwitchFurniture;
    }

    void OnDestroy()
    {
        m_InputActionReference.action.performed -= TapInput;
        m_TouchPositionActionReference.action.performed -= TouchPositionInput;

        m_Dropdown.OnSwitchFurnitureEvent -= OnSwitchFurniture;
    }

    void TapInput(InputAction.CallbackContext callbackContext)
    {
        m_HadPlacementInput = true;
        Debug.Log("Tap and release");
    }

    void TouchPositionInput(InputAction.CallbackContext callbackContext)
    {
        m_ScreenSpacePosition = callbackContext.ReadValue<Vector2>();
        Debug.Log(m_ScreenSpacePosition.x + ", " + m_ScreenSpacePosition.y);
    }

    void HandlePlacementInput()
    {
        if (m_HadPlacementInput && GridHandler.Instance.PlacementPoseIsValid && m_IsValidPosition)
        {
            PlaceFurniture();
            ClearPreview();
        }
        
        m_HadPlacementInput = false;
    }

    void UpdatePreviewPosition()
    {
        if (m_CurrentPreview == null)
        {
            CreatePreviewInstance();
        }

        if (GridHandler.Instance.PlacementPoseIsValid)
        {
            Ray ray = Camera.main.ScreenPointToRay(m_ScreenSpacePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, m_RaycastDistance, m_PlacementMask))
            {
                m_CurrentPreview.transform.position = SnapToGrid(hit.point);
                Vector3 localPosition = m_CurrentPreview.transform.localPosition;
                m_CurrentPreview.transform.localPosition = new Vector3(localPosition.x, 0, localPosition.z);
            }
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
            renderer.material = GridHandler.Instance.PlacementPoseIsValid && m_IsValidPosition ? m_ValidMaterial : m_InvalidMaterial;
        }
    }

    void CreatePreviewInstance()
    {
        m_CurrentPreview = Instantiate(m_Dropdown.CurrSelectedFurnitureInfo.FurniturePrefab, m_GridTransform);
        m_PreviewRenderers = m_CurrentPreview.GetComponentsInChildren<Renderer>();
        SetPreviewMaterials();
    }

    void OnSwitchFurniture()
    {
        if (m_CurrentPreview == null)
            return;
        ClearPreview();
        CreatePreviewInstance();
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
        GameObject furniture = Instantiate(m_Dropdown.CurrSelectedFurnitureInfo.FurniturePrefab,
                  m_CurrentPreview.transform.position,
                  m_CurrentPreview.transform.rotation, m_GridTransform);
        furniture.transform.position = m_CurrentPreview.transform.position; 
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