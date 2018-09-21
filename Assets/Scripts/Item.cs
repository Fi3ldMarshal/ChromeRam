using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public float sizeGain = 0.2f;

    public float liveTime = 5f;

    public Renderer rend;

    public static float fadeOffPercent = 0.9f, fadeOffEnd = .99f;

    float timeInited = 0f;

    void Start()
    {
        timeInited = Time.time;
    }

    void OnEnable()
    {
        timeInited = Time.time;
        Color c = rend.material.GetColor("_Color");
        c.a = 1f;
        rend.material.SetColor("_Color", c);
    }

    void Update()
    {
        if (Time.time - timeInited >= liveTime * fadeOffPercent)
        {
            Color c = rend.material.GetColor("_Color");
            c.a = 1f - (Time.time - timeInited - liveTime * fadeOffPercent) / ((liveTime - liveTime * fadeOffPercent) * fadeOffEnd);
            rend.material.SetColor("_Color", c);
        }
        if (Time.time - timeInited >= liveTime)
        {
            gameObject.SetActive(false);
        }
    }

}
