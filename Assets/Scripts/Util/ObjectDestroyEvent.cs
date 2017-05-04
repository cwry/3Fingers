using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ObjectDestroyEvent : MonoBehaviour {
    public new event Action Destroy;

    public static ObjectDestroyEvent Get(GameObject go) {
        var e = go.GetComponent<ObjectDestroyEvent>();
        if(e == null) {
            e = go.AddComponent<ObjectDestroyEvent>();
        }
        return e;
    }

    void OnDestroy() {
        if (Destroy != null) Destroy();
    }
}
