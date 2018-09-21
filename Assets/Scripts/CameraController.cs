using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float minViewSize = 5f;

    public float moveSpeed = 2f;

    public float smoothTime = 2f;

    public float sizeSmoothTime = .5f;

    Transform chrome;
    new Camera camera;

	void Start () {
        chrome = Chrome.main.transform;
        camera = GetComponent<Camera>();
	}

    Vector2 vel = Vector2.zero;
    float velo = 0f;

    void LateUpdate () {
        //camera.orthographicSize = Mathf.Clamp(minViewSize * chrome.localScale.x, minViewSize, 1e7f);
        camera.orthographicSize = Mathf.Clamp(Mathf.SmoothDamp(camera.orthographicSize, minViewSize * chrome.localScale.x, ref velo, sizeSmoothTime), minViewSize, 1e7f);
        transform.position = (Vector3)(Vector2.SmoothDamp(transform.position, chrome.position, ref vel, smoothTime, moveSpeed)) + Vector3.forward * -10f;
        if ((Vector2)transform.position == (Vector2)chrome.position)
        {
            vel = Vector2.zero;
        }
    }
}
