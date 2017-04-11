using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainLayer))]
public class TerrainLayerEditor : Editor {
    void OnSceneGUI() {
        var tar = (TerrainLayer)target;
        if (tar.points != null) {
            for (int i = 0; i < tar.points.Length; i++) {
                Vector2 original = tar.points[i];
                Vector2 handle = Handles.PositionHandle(original, Quaternion.identity);
                
                if(handle != original) {
                    Undo.RecordObject(target, "Change Terrainlayer Position");
                    tar.points[i] = handle;
                    break;
                }
            }
        }
    }
}
