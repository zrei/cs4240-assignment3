// The MIT License (MIT)
// Copyright (c) 2014 Brad Nelson and Play-Em Inc.
// CaptureScreenshot is based on Brad Nelson's MIT-licensed AnimationToPng: http://wiki.unity3d.com/index.php/AnimationToPNG
// AnimationToPng is based on Twinfox and bitbutter's Render Particle to Animated Texture Scripts.

using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;

public class TriggerCapture : MonoBehaviour {
    [SerializeField] private Transform m_FurnitureParent;
    [SerializeField] private string m_ScreenshotSaveFolder = "/FurnitureScreenshots";
    [SerializeField] private int m_Width = 640;
    [SerializeField] private int m_Height = 480;

    public bool UseSimple = false;
    
    public void TakeScreenshot() {
       var cam = Camera.main;
       // Set a mask to only draw only elements in this layer. e.g., capture your player with a transparent background.
       cam.cullingMask = LayerMask.GetMask("Furniture");

       string assetRelativeFilepath = m_ScreenshotSaveFolder + $"/{m_FurnitureParent.GetChild(0).name}.png";
       string filename = Application.dataPath + assetRelativeFilepath;

       if (UseSimple) {
           CaptureScreenshot.SimpleCaptureTransparentScreenshot(cam, m_Width, m_Height, filename, assetRelativeFilepath);
       }
       else {
           CaptureScreenshot.CaptureTransparentScreenshot(cam, m_Width, m_Height, filename, assetRelativeFilepath);
       }
    }

    public void SpawnObject(GameObject gameObject)
    {
        if (m_FurnitureParent.childCount > 0)
        {
            if (Application.IsPlaying(this))
            {
                foreach (Transform child in m_FurnitureParent)
                    Destroy(child.gameObject);
            }
            else
            {
                foreach (Transform child in m_FurnitureParent)
                    DestroyImmediate(child.gameObject);
            }
        }
       
        GameObject clone = Instantiate(gameObject, m_FurnitureParent);
        clone.name = gameObject.name;
        clone.transform.localPosition = Vector3.zero;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TriggerCapture))]
public class TriggerCaptureEditor : Editor
{
    private TriggerCapture m_TriggerCapture;
    private string m_FurniturePrefabParentFolder = "Assets/BigFurniturePack";
    private int m_SelectedFolderIndex;
    private int m_SelectedPrefabIndex;

    private void OnEnable()
    {
        m_TriggerCapture = (TriggerCapture) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_FurniturePrefabParentFolder = EditorGUILayout.TextField(m_FurniturePrefabParentFolder);

        string[] furniturePrefabFolders = AssetDatabase.GetSubFolders(m_FurniturePrefabParentFolder);
        
        if (furniturePrefabFolders.Length > 0)
        {
            m_SelectedFolderIndex = EditorGUILayout.Popup(m_SelectedFolderIndex, furniturePrefabFolders);
            string[] prefabGUIs = AssetDatabase.FindAssets("t:Prefab", new string[] {furniturePrefabFolders[m_SelectedFolderIndex]});
            
            string[] prefabNames = prefabGUIs.Select(x => GetPrefabName(AssetDatabase.GUIDToAssetPath(x))).ToArray();

            if (prefabGUIs.Length > 0)
            {
                m_SelectedPrefabIndex = EditorGUILayout.Popup(m_SelectedPrefabIndex, prefabNames);

                if (GUILayout.Button("Load Prefab"))
                {
                    m_TriggerCapture.SpawnObject((GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(prefabGUIs[m_SelectedPrefabIndex]), typeof(GameObject)));
                }
            }
        }

        if (Application.IsPlaying(m_TriggerCapture))
        {
            GUILayout.Space(10f);

            if (GUILayout.Button("Take Screenshot"))
                m_TriggerCapture.TakeScreenshot();
        }
    }

    private string GetPrefabName(string filePath)
    {
        string[] splitPath = filePath.Split("/");
        return splitPath[splitPath.Length - 1];
    }
}
#endif

public static class CaptureScreenshot {
    public static void CaptureTransparentScreenshot(Camera cam, int width, int height, string screengrabfile_path, string assetRelativeFilepath) {
        // This is slower, but seems more reliable.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_white = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var tex_black = new Texture2D(width, height, TextureFormat.ARGB32, false);
        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        cam.backgroundColor = Color.black;
        cam.Render();
        tex_black.ReadPixels(grab_area, 0, 0);
        tex_black.Apply();

        cam.backgroundColor = Color.white;
        cam.Render();
        tex_white.ReadPixels(grab_area, 0, 0);
        tex_white.Apply();

        // Create Alpha from the difference between black and white camera renders
        for (int y = 0; y < tex_transparent.height; ++y) {
            for (int x = 0; x < tex_transparent.width; ++x) {
                float alpha = tex_white.GetPixel(x, y).r - tex_black.GetPixel(x, y).r;
                alpha = 1.0f - alpha;
                Color color;
                if (alpha == 0) {
                    color = Color.clear;
                } 
                else {
                    color = tex_black.GetPixel(x, y) / alpha;
                }
                color.a = alpha;
                tex_transparent.SetPixel(x, y, color);
            }
        }

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(screengrabfile_path, pngShot);
        ImportFile(assetRelativeFilepath);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);

        Texture2D.Destroy(tex_black);
        Texture2D.Destroy(tex_white);
        Texture2D.Destroy(tex_transparent);
    }

    public static void SimpleCaptureTransparentScreenshot(Camera cam, int width, int height, string screengrabfile_path, string assetRelativeFilepath) {
        // Depending on your render pipeline, this may not work.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels(grab_area, 0, 0);
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(screengrabfile_path, pngShot);
        ImportFile(assetRelativeFilepath);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);

        Texture2D.Destroy(tex_transparent);
    }

    public static void ImportFile(string assetRelativeFilepath)
    {
        AssetDatabase.Refresh();

        string finalFilepath = "Assets" + assetRelativeFilepath;

        TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(finalFilepath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;

        AssetDatabase.ImportAsset(finalFilepath);
        AssetDatabase.Refresh();
    }
}