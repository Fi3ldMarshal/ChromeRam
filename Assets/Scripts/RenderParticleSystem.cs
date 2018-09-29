using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RenderParticleSystem : EditorWindow
{

    static ParticleSystem p;

    static RenderTexture renderTexture;

    static string outputFolder = "";

    static string assetsFolder;

    static Color backColor;

    static bool color2A = true;

    static Color toA;

    static bool useRenderedMesage = false;

    static float cameraOrthoSize = 4f;

    static int FPS = 30;

    public enum OutputType
    {
        SeparateImages,
        Combine
    }

    static OutputType outType = OutputType.SeparateImages;

    Camera rendcam;

    [MenuItem("Window/Rendering/Render Particle System")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RenderParticleSystem));
        assetsFolder = Application.dataPath;
        backColor = Color.white;
        toA = Color.white;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Particle system:");
        p = (ParticleSystem)EditorGUILayout.ObjectField(p, typeof(ParticleSystem), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Render Texture:");
        renderTexture = (RenderTexture)EditorGUILayout.ObjectField(renderTexture, typeof(RenderTexture), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("FPS:");
        FPS = EditorGUILayout.IntField(FPS);
        if (FPS < 1)
            FPS = 30;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Background Color:");
        backColor = EditorGUILayout.ColorField(backColor);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Camera Orthographic Size:");
        cameraOrthoSize = EditorGUILayout.FloatField(cameraOrthoSize);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Output Type:");
        outType = (OutputType)EditorGUILayout.EnumPopup("", outType);
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

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Send message when done:");
        useRenderedMesage = GUILayout.Toggle(useRenderedMesage, "");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Render") && p && !string.IsNullOrEmpty(outputFolder) && renderTexture)
            RenderPS();
    }

    //static bool rendering = false;
    int oldLayer;
    Transform oldParent;

    void RenderPS()
    {

        if (!Directory.Exists(assetsFolder + "/" + outputFolder))
            Directory.CreateDirectory(assetsFolder + "/" + outputFolder);
        int lm = 512;
        //Debug.Log(lm);
        oldLayer = p.gameObject.layer;
        oldParent = p.transform.parent;
        p.transform.parent = null;
        rendcam = new GameObject().AddComponent<Camera>();
        //Debug.Log(rendcam);
        rendcam.cullingMask = lm;
        rendcam.orthographicSize = cameraOrthoSize;
        rendcam.orthographic = true;
        rendcam.transform.position = (Vector3)((Vector2)p.transform.position) - Vector3.forward;
        rendcam.backgroundColor = backColor;
        rendcam.targetTexture = renderTexture;
        p.gameObject.layer = LayerMask.NameToLayer("RenderPS");
        //Selection.activeGameObject = ps;

        //rendering = true;
        int im = 0;
        
        p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        //p.Simulate(0.00001f, true, true, false);

        int n = (int)(FPS * p.main.duration);
        if (n < 1) n = 1;

        Texture2D texc = null;

        if (outType == OutputType.Combine)
        {
            int s = (int)Mathf.Sqrt(n);
            if (s != Mathf.Sqrt(n) || s == 0)
                s++;
            texc = new Texture2D(s * renderTexture.width, s * renderTexture.height, TextureFormat.ARGB32, false);
        }

        for (int k = 0; k <= n; k++)
        {

            //Debug.Log("k: " + k + " t: " + (((float)k / n) * p.main.duration));

            p.Simulate(((float)k / n) * p.main.duration, true, true, false);

            rendcam.Render();
            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
            if (color2A)
            {
                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        Color c = tex.GetPixel(i, j);
                        tex.SetPixel(i, j, ColorToAlpha(c, toA));
                    }
                }
            }
            //output -> tex
            switch (outType)
            {
                case OutputType.SeparateImages:
                    var bytes = tex.EncodeToPNG();
                    File.WriteAllBytes(assetsFolder + "/" + outputFolder + "/img" + im + ".png", bytes);
                    im++;
                    break;
                case OutputType.Combine:
                    AddInTex(texc, tex, k);
                    if(k + 1 > n)
                        File.WriteAllBytes(assetsFolder + "/" + outputFolder + "/img.png", texc.EncodeToPNG());
                    break;
            }
            RenderTexture.active = null;

        }

        EndRendering();

    }

    void EndRendering()
    {
        RenderTexture.active = null;
        p.gameObject.layer = oldLayer;
        p.transform.parent = oldParent;
        DestroyImmediate(rendcam.gameObject);
        if (useRenderedMesage)
            Debug.Log("Rendered");
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        System.GC.Collect();
    }

    void AddInTex(Texture2D src, Texture2D plus, int pos)
    {
        if (plus.width > src.width || plus.height > src.height)
            throw new System.Exception("Plus texutre cannot be larger than added one");
        if (pos < 0)
            throw new System.Exception("Position cannot be negative");
        int s = pos % (src.width / plus.height),
            r = (pos * plus.width) / src.width;
        //Debug.Log("Pos: " + pos + " (s,r): " + new Vector2(s, r));
        for (int i = 0; i < plus.width; i++)
        {
            for (int j = 0;j < plus.height; j++)
            {
                src.SetPixel(i + s * plus.width, src.height - (j + r * plus.height), plus.GetPixel(i, plus.height - j));
            }
        }

    }

    /*void Update()
    {
        if (rendering)
        {
            if (!p.isPlaying)
            {
                p.Play(true);
            }
            //print("Rendering");
            rendcam.Render();
            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(2, 2);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
            if(color2A)
            {
                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        Color c = tex.GetPixel(i, j);
                        tex.SetPixel(i, j, ColorToAlpha(c, toA));
                        //if (c == toA)   //bad way
                        //{
                        //    c.a = 0f;
                        //    tex.SetPixel(i, j, c);
                        //}
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

    }*/

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
