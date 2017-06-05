using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractCanvas : MonoBehaviour {
    static InteractCanvas instance;
    public static InteractCanvas Instance {
        get {
            if (instance == null) {
                instance = (Instantiate(Resources.Load("InteractCanvas")) as GameObject).GetComponent<InteractCanvas>();
            }

            return instance;
        }
    }

    Object interactButtonPrefab;

    void Awake() {
        if (instance == null) instance = this;
        interactButtonPrefab = Resources.Load("InteractButton");
    }

    public Button SpawnButton(Vector2 pos) {
        GameObject btn = Instantiate(interactButtonPrefab) as GameObject;
        btn.transform.SetParent(transform);
        btn.transform.position = pos;
        return btn.GetComponent<Button>();
    }
}
