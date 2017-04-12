#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TerrainLayer : MonoBehaviour {
    public Vector2[] points;

    void Update() {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode) {
            points = points
                .OrderBy(v => v.x)
                .ToArray();
        }
#endif
    }

    void OnDrawGizmos() {
        if (points != null) {
            Gizmos.color = Color.red;
            for (int i = 1; i < points.Length; i++) {
                Vector2 from = points[i - 1];
                Vector2 to = points[i];
                Gizmos.DrawLine(from, to);
            }
        }
    }

    public Vector2? Sample(float x) {
        if (points == null) return null;
        if (points.Length == 1 || x <= points[0].x) return points[0];
        for (int i = 0; i < points.Length - 1; i++) {
            if (x >= points[i].x && x <= points[i + 1].x) {
                float delta = points[i + 1].x - points[i].x;
                float alpha = (x - points[i].x) / delta;
                return Vector2.Lerp(points[i], points[i + 1], alpha);
            }
        }
        return points[points.Length - 1];
    }

    public Vector2[] GetSubPath(float xO, float xG) {
        if (points == null || points.Length <= 1) return null;
        Vector2 origin = (Vector2)Sample(xO);
        Vector2 goal = (Vector2)Sample(xG);
        if (origin == goal) return null;

        var subPath = new List<Vector2>();
        subPath.Add(origin);

        int direction = (int)Mathf.Sign(goal.x - origin.x);
        for (
            int i = direction == 1 ? 0 : points.Length - 1;

            i >= 0 && i < points.Length &&
            (
                (direction == 1 && xG > points[i].x) ||
                (direction == -1 && xG < points[i].x)
            );

            i += direction
        ) {
            if (
                (direction == 1 && xO < points[i].x) ||
                (direction == -1 && xO > points[i].x)
            ) {
                subPath.Add(points[i]);
            }
        }

        subPath.Add(goal);
        return subPath.ToArray();
    }
}
