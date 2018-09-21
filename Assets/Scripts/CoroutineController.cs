/*
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

public class CoroutineController : MonoBehaviour {

    [System.Serializable]
    class TimedCoroutine
    {
        Coroutine c;
        float t;

        public TimedCoroutine(Coroutine cor, float time)
        {
            c = cor;
            t = time;
        }

    }

    public static List<Coroutine> RunningCoroutines = new List<Coroutine>(32);

    public static CoroutineController main;

    static List<TimedCoroutine> TimedCoroutines = new List<TimedCoroutine>(32);

    void Awake()
    {
        main = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if(RunningCoroutines.Count > 0)
        {
            for (int i = 0; i < RunningCoroutines.Count; i++)
            {
                if (!RunningCoroutines[i].Method.MoveNext())
                {
                    RunningCoroutines.RemoveAt(i);
                    i--;
                    print("Coroutine stopped");
                }
                else
                {
                    object o = RunningCoroutines[i].Method.Current;
                    if (o is WaitForSeconds)
                    {
                        var bytes = new byte[4];
                        var ptr = Marshal.AllocHGlobal(4);
                        Marshal.StructureToPtr(o, ptr, false);
                        Marshal.Copy(ptr, bytes, 0, 4);
                        Marshal.FreeHGlobal(ptr);
                        float time = System.BitConverter.ToSingle(bytes, 0);
                        print(time);
                    }
                }
            }
        }
    }

    public static void StartCoroutine(Coroutine c)
    {
        RunningCoroutines.Add(c);
    }

    public static void StopCoroutine(Coroutine c)
    {
        RunningCoroutines.Remove(c);
    }

    public static void PauseCoroutine(Coroutine c)
    {

    }

    public static void ContunueCoroutine(Coroutine c)
    {

    }

}
*/