using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RAMusageTXT : MonoBehaviour {

    public int startSize = 128;

    public int maxSize = 2048;
    
    Chrome player;

    Text txt;

    float playerStartSize = 1f;

    float oldSize = 0f;

    void Start () {
        player = Chrome.main;
        playerStartSize = player.size;
        txt = GetComponent<Text>();
        oldSize = playerStartSize;
	}

	void Update () {
        if (oldSize != player.size)
        {
            txt.text = ((int)((player.size / playerStartSize) * startSize)).ToString() + "/" + maxSize;
        }
	}
}
