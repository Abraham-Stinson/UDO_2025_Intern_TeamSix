using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class TransparentScreenshotTool : EditorWindow
{
    private Camera captureCamera;
    private int width = 1024;
    private int height = 1024;

    [MenuItem("Tools/Take Transparent Screenshot")]
    public static void ShowWindow()
    {
        GetWindow<TransparentScreenshotTool>("Screenshot Tool");
    }

    private void OnGUI()
    {
        captureCamera = (Camera)EditorGUILayout.ObjectField("Camera", captureCamera, typeof(Camera), true);
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        if (GUILayout.Button("Capture Screenshot"))
        {
            if (captureCamera == null)
            {
                Debug.LogError("Assign a camera first!");
                return;
            }

            TakeTransparentScreenshot(captureCamera, width, height);
        }
    }

    public static void TakeTransparentScreenshot(Camera cam, int width, int height)
    {
        var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        // Backup settings
        Color originalColor = cam.backgroundColor;
        CameraClearFlags originalFlags = cam.clearFlags;

        // Transparent background
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        // Restore settings
        cam.clearFlags = originalFlags;
        cam.backgroundColor = originalColor;

        // Auto timestamped file name
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string path = Path.Combine(Application.dataPath, $"Screenshot_{timestamp}.png");

        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log("Saved screenshot: " + path);
    }
}
