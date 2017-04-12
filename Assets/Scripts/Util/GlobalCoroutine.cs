using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCoroutine : MonoBehaviour {
    static GlobalCoroutine instance;
    public static GlobalCoroutine Instance {
        get {
            if(instance == null) instance = new GameObject("Global Coroutine").AddComponent<GlobalCoroutine>();
            return instance;
        }
    }

    public static GlobalCoroutine OnGameObject(GameObject go) {
        var gc = go.GetComponent<GlobalCoroutine>();
        if (gc == null) gc = go.AddComponent<GlobalCoroutine>();
        return gc;
    }
}
