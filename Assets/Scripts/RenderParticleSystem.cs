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

    static bool color2A = true;

    static Color toA;

    Camera rendcam;

    [MenuItem("Window/Rendering/Render Particle System")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RenderParticleSystem));
        assetsFolder = Application.dataPath;
        rendering = false;
        backColor = Color.white;
        toA = Color.white;
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

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Color 2 Alpha:");
        color2A = GUILayout.Toggle(color2A, "");
        if (color2A)
            toA = EditorGUILayout.ColorField(toA);
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
        
        p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
            if(color2A)
            {
                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        Color c = tex.GetPixel(i, j);
                        tex.SetPixel(i, j, ColorToAlpha(c, toA));
                        /*if (c == toA)   //bad way
                        {
                            c.a = 0f;
                            tex.SetPixel(i, j, c);
                        }*/
                    }
                }
            }
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(assetsFolder + "/" + outputFolder + "/img" + im + ".png", bytes);
            im++;
            RenderTexture.active = null;
            t += Time.deltaTime;
            if (t > p.main.duration)
            {
                rendering = false;
                EndRendering();
            }
            DestroyImmediate(tex);
        }

    }

    //idea taken form GIMP source code
    static Color ColorToAlpha(Color src, Color c)
    {
        Color alpha;

        alpha.a = src.a;

        if (c.r < 0.0001f)
            alpha.r = src.r;
        else if (src.r > c.r)
            alpha.r = (src.r - c.r) / (1.0f - c.r);
        else if (src.r < c.r)
            alpha.r = (c.r - src.r) / c.r;
        else alpha.r = 0f;

        if (c.g < 0.0001f)
            alpha.g = src.g;
        else if (src.g > c.g)
            alpha.g = (src.g - c.g) / (1.0f - c.g);
        else if (src.g < c.g)
            alpha.g = (c.g - src.g) / c.g;
        else alpha.g = 0f;

        if (c.b < 0.0001f)
            alpha.b = src.b;
        else if (src.b > c.b)
            alpha.b = (src.b - c.b) / (1.0f - c.b);
        else if (src.b < c.b)
            alpha.b = (c.b - src.b) / c.b;
        else alpha.b = 0f;

        if(alpha.r > alpha.g)
        {
            if(alpha.r > alpha.b)
            {
                src.a = alpha.r;
            }
            else
            {
                src.a = alpha.b;
            }
        }
        else if(alpha.g > alpha.b)
        {
            src.a = alpha.g;
        } else
        {
            src.a = alpha.b;
        }

        if (src.a < 0.0001f)
            return src;

        src.r = (src.r - c.r) / src.a + c.r;
        src.g = (src.g - c.g) / src.a + c.g;
        src.b = (src.b - c.b) / src.a + c.b;

        src.a *= alpha.a;

        return src;
    }

}
