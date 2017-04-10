using UnityEditor;
using UnityEngine;

public class Terrain : MonoBehaviour {
    public Vector2[] points;

    void OnDrawGizmos(){
        if(points != null){
            Gizmos.color = Color.red;
            for(int i = 1; i < points.Length; i++){
                Vector2 from = points[i - 1];
                Vector2 to = points[i];
                Gizmos.DrawLine(from, to);
            }


            foreach(var p in points){
                Handles.PositionHandle(p, Quaternion.identity);
            }
        }
    }
}
