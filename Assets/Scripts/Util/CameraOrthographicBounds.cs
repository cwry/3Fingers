﻿using UnityEngine;
using System.Collections;

public static partial class CameraExtensions {
    public static Bounds orthographicBounds(this Camera camera) {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}
