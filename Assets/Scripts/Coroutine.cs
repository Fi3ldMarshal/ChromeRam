/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Coroutine {

    IEnumerator method;

    bool running;

    public IEnumerator Method
    {
        get
        {
            return method;
        }
        set
        {
            if(value == null && running)
            {
                Stop();
            }
            method = value;
        }
    }

    public bool Running
    {
        get
        {
            return running;
        }
        set
        {
            if(!value)
            {
                Stop();
            }
        }
    }

    public Coroutine()
    {
        method = null;
        running = false;
    }

    public Coroutine(IEnumerator method)
    {
        this.method = method;
        running = false;
    }

    public void Start()
    {
        if(!running)
        {
                if(method != null)
                {
                    running = true;
                    CoroutineController.StartCoroutine(this);
                } else
                {
                    throw new System.Exception("Coroutine needs a Method in order to start");
                }
        }
    }

    public void Stop()
    {
        if (running)
        {
            CoroutineController.StopCoroutine(this);
            running = false;
        }
    }
	
}
*/