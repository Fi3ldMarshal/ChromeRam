using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    Transform camt;
    float dist = 0f;

    void Start()
    {
        camt = Camera.main.transform;
        dist = (transform.localScale.x)/* * 2f*/;
    }

    float modf(float a, float b)
    {
        return b * (a / b - Mathf.Floor(a / b));
    }
    
    float sign(float a)
    {
        return a < 0f ? -1 : a > 0f ? 1f : 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (Mathf.Abs(camt.position.x - transform.position.x) > dist || Mathf.Abs(camt.position.y - transform.position.y) > dist)
        {
            transform.position = transform.position + new Vector3(sign(camt.position.x - transform.position.x) * dist, sign(camt.position.y - transform.position.y) * dist, 0f);
        }
	}
}
