using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RenderParticleSystem : EditorWindow
{

    static GameObject ps;

    static RenderTexture renderTexture;

    static string outputFolder = "";

    static string assetsFolder;

    static Color backColor;

    Camera rendcam;

    [MenuItem("Window/Rendering/Render Particle System")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RenderParticleSystem));
        assetsFolder = Application.dataPath;
        rendering = false;
        backColor = Color.white;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Particle system:");
        ps = (GameObject)EditorGUILayout.ObjectField(ps, typeof(GameObject), true);
        if (ps != null && !ps.GetComponent<ParticleSystem>())
        {
            ps = null;
            Debug.LogWarning("Given object doesn't have ParticleSystem component");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Render Texture:");
        renderTexture = (RenderTexture)EditorGUILayout.ObjectField(renderTexture, typeof(RenderTexture), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Background Color:");
        backColor = EditorGUILayout.ColorField(backColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Output Folder:");
        outputFolder = EditorGUILayout.TextField(outputFolder);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Render") && ps && !string.IsNullOrEmpty(outputFolder) && renderTexture)
            RenderPS();
    }

    ParticleSystem p;
    static bool rendering = false;
    int oldLayer;
    Transform oldParent;

    void RenderPS()
    {

        if (!Directory.Exists(assetsFolder + "/" + outputFolder))
            Directory.CreateDirectory(assetsFolder + "/" + outputFolder);
        int lm = 512;
        //Debug.Log(lm);
        oldLayer = ps.layer;
        oldParent = ps.transform.parent;
        ps.transform.parent = null;
        p = ps.GetComponent<ParticleSystem>();
        rendcam = new GameObject().AddComponent<Camera>();
        //Debug.Log(rendcam);
        rendcam.cullingMask = lm;
        rendcam.orthographic = true;
        rendcam.transform.position = (Vector3)((Vector2)ps.transform.position) - Vector3.forward;
        rendcam.backgroundColor = backColor;
        rendcam.targetTexture = renderTexture;
        ps.layer = LayerMask.NameToLayer("RenderPS");
        Selection.activeGameObject = ps;

        rendering = true;
        t = 0f;
        im = 0;
        
        //p.Stop();
        //p.Play();

        /*
        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(assetsFolder + "/img.png", bytes);*/

    }

    void EndRendering()
    {
        RenderTexture.active = null;
        ps.layer = oldLayer;
        ps.transform.parent = oldParent;
        DestroyImmediate(rendcam.gameObject);
        Debug.Log("Rendered");
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    float t = 0f;
    int im = 0;

    void Update()
    {
        if (rendering)
        {
            if (!p.isPlaying)
                p.Play();
            //print("Rendering");
            rendcam.Render();
            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(assetsFolder + "/" + outputFolder + "/img" + im + ".png", bytes);
            im++;
            RenderTexture.active = null;
            if (t > p.main.duration)
            {
                rendering = false;
                EndRendering();
            }
            t += Time.deltaTime;
            DestroyImmediate(tex);
        }

    }

}
