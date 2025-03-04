using UnityEngine;
using UnityEditor;

public class FurnitureGridTest : MonoBehaviour
{
    [SerializeField] private float m_GridSize;
    [SerializeField] private int m_NumCols;
    [SerializeField] private Transform m_PlaneTransform;
    [SerializeField] private Transform m_FurnitureTransform;
    public float GridSize => m_GridSize;

    public void PlaceFurniture()
    {
        m_FurnitureTransform.localScale = Vector3.one;
        m_FurnitureTransform.position = Vector3.zero;
        for (int i = 0; i < m_FurnitureTransform.childCount; ++i)
        {
            int rowNum = i / m_NumCols;
            int colNum = i % m_NumCols;
            m_FurnitureTransform.GetChild(i).position = new Vector3(rowNum * m_GridSize, 0, colNum * m_GridSize);
        }
        int totalRows = m_FurnitureTransform.childCount / m_NumCols;
        m_PlaneTransform.position = new Vector3(((float)totalRows) / 2 * m_GridSize, 0, ((float)m_NumCols) / 2 * m_GridSize);
        m_PlaneTransform.localScale = new Vector3(totalRows + 2 * m_GridSize / 10, 1, m_NumCols * m_GridSize / 2);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FurnitureGridTest))]
public class FurnitureGridEditor : Editor
{
    private FurnitureGridTest m_FurnitureGridTest;

    private void OnEnable()
    {
        m_FurnitureGridTest = (FurnitureGridTest) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(5f);

        if (GUILayout.Button("Space out furniture"))
        {
            m_FurnitureGridTest.PlaceFurniture();
        }
    }
}
#endif