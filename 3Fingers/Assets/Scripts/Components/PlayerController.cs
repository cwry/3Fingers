using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;

public class PlayerController : MonoBehaviour {
    public TerrainLayer currentTerrainLayer;
    public float speed = 1f;

    void Update() {
        if(!GameActionHandler.Instance.IsBlocked && Input.GetMouseButtonDown(0)) {
            float inputX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            GameActionHandler.Instance.Execute(TweenEnumerators.TweenPathBySpeed(this, currentTerrainLayer.GetSubPath(transform.position.x, inputX), speed));
        }
    }
}
