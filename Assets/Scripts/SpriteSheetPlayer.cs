using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetPlayer : MonoBehaviour {

    public int xn = 128, yn = 128;

    public int first = 0, last = 30;

    public float FPS = 30f;

    Renderer rend;

    Material mat;

    int w, h, cnt, c = 0;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        if(rend)
        {
            mat = rend.material;
            if(mat)
            {
                w = mat.mainTexture.width;
                h = mat.mainTexture.height;
                cnt = (w / xn) * (h / yn);
                if (last > cnt)
                    last = cnt;
                mat.SetTextureScale("_MainTex", new Vector2((float)xn / w, (float)yn / h));
                c = first;
            }
        }
	}

    float t = 0f;

	// Update is called once per frame
	void Update () {
		if(mat)
        {
            int col, line;
            col = c % (w / xn);
            line = c / (h / yn);
            print("(c, col, line): " + new Vector3(c, col, line));
            mat.SetTextureOffset("_MainTex", new Vector2(col / ((float)w / xn), line / ((float)h / yn)));
            c++;
            c = c % last;
            if (c == 0)
                c = first;
            //Debug.Break();
        }
	}
}
