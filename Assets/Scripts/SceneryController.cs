using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryController : MonoBehaviour {

    [System.Serializable]
    public struct Scenery
    {
        public float minSize;

        public Sprite background;

        public float backgroundScale;
    }

    public Scenery[] sceneries;

    public GameObject background;

    public int currentScenery = 0;

    SpriteRenderer backgRenderer;

    Background backg;

	// Use this for initialization
	void Start () {
        backgRenderer = background.GetComponent<SpriteRenderer>();
        backg = background.GetComponent<Background>();
	}

    void SwitchScenery(int s)
    {
        currentScenery = s;
        print("Switch");
        backgRenderer.transform.localScale = Vector2.one * sceneries[s].backgroundScale;
        backgRenderer.sprite = sceneries[s].background;
        backg.Reset();
    }

    // Update is called once per frame
    void Update () {
        if (currentScenery + 1 < sceneries.Length)
        {
            if (Chrome.main.size > sceneries[currentScenery + 1].minSize)
            {
                SwitchScenery(currentScenery + 1);
            }
        }
	}
}
