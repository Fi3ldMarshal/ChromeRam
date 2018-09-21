using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject[] Items;

    public float spawnInterval = 2f;

    public float startForce = 5f, startTorque = 3f;

    public List<List<GameObject>> objs;

    int[] indexes;

    System.Random rand;

    float time = 0f;

    GameObject GetObject(int ItemIndex)
    {
        if (ItemIndex >= Items.Length || ItemIndex < 0)
            throw new System.Exception("ItemIndex is out of range");
        GameObject o = objs[ItemIndex][indexes[ItemIndex]];
        indexes[ItemIndex] = (indexes[ItemIndex] + 1) % objs[ItemIndex].Count;
        return o;
    }

    void Spawn()
    {
        GameObject item = GetObject(rand.Next(0, Items.Length));
        item.SetActive(true);
        item.transform.position = transform.position;
        item.transform.rotation = Quaternion.identity;
        Rigidbody2D iphy = item.GetComponent<Rigidbody2D>();
        iphy.AddForce(new Vector2((float)(rand.NextDouble() * 2f - 1f), (float)(rand.NextDouble() * 2f - 1f)).normalized * startForce);
        iphy.AddTorque(startTorque);
    }

    void Awake()
    {
        rand = new System.Random();
        objs = new List<List<GameObject>>(Items.Length);
        for (int i = 0; i < Items.Length; i++)
        {
            int s = (int)Mathf.Ceil(Items[i].GetComponent<Item>().liveTime / spawnInterval);
            objs.Add(new List<GameObject>(s));
            for (int j = 0; j < s; j++)
            {
                GameObject obj = Instantiate(Items[i]);
                objs[i].Add(obj);
                obj.SetActive(false);
                obj.transform.parent = Scene.Root.transform;
            }
        }
        indexes = new int[Items.Length];
    }

	void Start () {
		
	}
	
	void Update () {
        time += Time.deltaTime;
        if(time >= spawnInterval)
        {
            Spawn();
            time = 0f;
        }
	}
}
