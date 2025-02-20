using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FurnitureInfoCreator", menuName = "ScriptableObject/FurnitureInfoCreator")]
public class FurnitureInfoCreator : ScriptableObject
{
    public string FurnitureParentFolder = "Assets/BigFurniturePack";
    public string FurnitureSOFolder = "Assets/FurnitureScriptableObjects";
    public string FurnitureScreenshotsFolder = "Assets/FurnitureScreenshots";
    public FurnitureDropdown FurnitureDropdown;
}

#if UNITY_EDITOR
[CustomEditor(typeof(FurnitureInfoCreator))]
public class FurnitureInfoCreatorEditor : Editor
{
    private FurnitureInfoCreator m_FurnitureInfoCreator;
    private bool[] m_UseSubfolders;

    private void OnEnable()
    {
        m_FurnitureInfoCreator = (FurnitureInfoCreator) target;
    }

    private string GetSubfolder(string fullPath)
    {
        string[] splitPath = fullPath.Split("/");
        return splitPath[splitPath.Length - 1];
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        string[] subFolders = AssetDatabase.GetSubFolders(m_FurnitureInfoCreator.FurnitureParentFolder);
        
        if (subFolders.Length > 0)
        {
            if (GUILayout.Button("Create scriptable objects"))
            {
                string[] existingSubFolders = AssetDatabase.GetSubFolders(m_FurnitureInfoCreator.FurnitureSOFolder);
                AssetDatabase.DeleteAssets(existingSubFolders, new List<string>{});
                
                foreach (string subFolder in subFolders)
                {
                    string[] prefabGUIs = AssetDatabase.FindAssets("t:Prefab", new string[] {subFolder});

                    if (prefabGUIs.Length == 0)
                        continue;

                    string subFoldername = GetSubfolder(subFolder);
                    AssetDatabase.CreateFolder(m_FurnitureInfoCreator.FurnitureSOFolder, subFoldername);
                    
                    foreach (string prefab in prefabGUIs)
                    {
                        FurnitureInfo furnitureInfo = (FurnitureInfo) ScriptableObject.CreateInstance(typeof(FurnitureInfo));
                        GameObject prefabInstance =  (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefab), typeof(GameObject));
                        furnitureInfo.FurniturePrefab = prefabInstance;
                        string spritePath = m_FurnitureInfoCreator.FurnitureScreenshotsFolder + "/" + prefabInstance.name + ".png";
                        if (AssetDatabase.AssetPathExists(spritePath))
                        {
                            furnitureInfo.FurnitureSprite = (Sprite) AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite));
                        }
                        AssetDatabase.CreateAsset(furnitureInfo,  m_FurnitureInfoCreator.FurnitureSOFolder + "/" + subFoldername + "/" + prefabInstance.name + ".asset");
                    }
    
                }
            }
            AssetDatabase.Refresh();
        }

        GUILayout.Space(5f);

        string[] soSubfolders = AssetDatabase.GetSubFolders(m_FurnitureInfoCreator.FurnitureSOFolder);

        if (soSubfolders.Length > 0)
        {
            bool[] subfoldersCopy = m_UseSubfolders;

            m_UseSubfolders = new bool[soSubfolders.Length];
            if (subfoldersCopy != null)
            {
                for (int i = 0; i < subfoldersCopy.Length; ++i)
                {
                    m_UseSubfolders[i] = subfoldersCopy[i];
                }
            }

            for (int i = 0; i < soSubfolders.Length; ++i)
            {
                m_UseSubfolders[i] = GUILayout.Toggle(m_UseSubfolders[i], "Use prefabs in folder: " + soSubfolders[i]);
            }

            if (GUILayout.Button("Import to dropdown"))
            {
                List<FurnitureInfo> furnitureInfos = new();
                for (int i = 0; i < soSubfolders.Length; ++i)
                {
                    if (m_UseSubfolders[i])
                    {
                        string[] prefabGUIs = AssetDatabase.FindAssets("t:FurnitureInfo", new string[] {soSubfolders[i]});
                        foreach (string prefab in prefabGUIs)
                        {
                            furnitureInfos.Add((FurnitureInfo) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefab), typeof(FurnitureInfo)));
                        }
                    }
                }
                m_FurnitureInfoCreator.FurnitureDropdown.SetDropdownContent(furnitureInfos.ToArray());
            }
        }
    }
}
#endif