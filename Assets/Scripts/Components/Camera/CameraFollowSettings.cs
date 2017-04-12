using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraFollowSettings : MonoBehaviour {
    public bool dampen = false;
    public float dampModifier = 1;

    public enum CameraSizeMethod {
        INHERIT = 0,
        VALUE = 1,
        COLLIDER_HEIGHT = 2,
        COLLIDER_WIDTH = 3
    }
    private Func<float, float>[] cameraSizeMethods = new Func<float, float>[4];
    public CameraSizeMethod cameraSizeMethod;
    public float cameraSizeModifier = 1;
    public float cameraSizeValue;

    public enum CameraPositionMethod {
        INHERIT = 0,
        VALUE = 1,
        CENTER = 2,
        COLLIDER_MIN = 3,
        COLLIDER_MAX = 4,
        COLLIDER = 5
    }
    private Func<float, float>[] cameraXPositionMethods = new Func<float, float>[6];
    public CameraPositionMethod cameraXPositionMethod;
    public float cameraXPositionValue;
    public bool cameraXPositionOverrideOffset = true;
    public float cameraXPositionOffset;

    private Func<float, float>[] cameraYPositionMethods = new Func<float, float>[6];
    public CameraPositionMethod cameraYPositionMethod;
    public float cameraYPositionValue;
    public bool cameraYPositionOverrideOffset = true;
    public float cameraYPositionOffset;

    public BoxCollider2D coll = null;

    void Awake() {
        if(coll == null) coll = GetComponent<BoxCollider2D>();
        cameraSizeMethods[(int)CameraSizeMethod.INHERIT] = sanitizeCameraSizeInherit;
        cameraSizeMethods[(int)CameraSizeMethod.VALUE] = sanitizeCameraSizeValue;
        cameraSizeMethods[(int)CameraSizeMethod.COLLIDER_HEIGHT] = sanitizeCameraSizeColliderHeight;
        cameraSizeMethods[(int)CameraSizeMethod.COLLIDER_WIDTH] = sanitizeCameraSizeColliderWidth;

        cameraXPositionMethods[(int)CameraPositionMethod.INHERIT] = sanitizePosInherit;
        cameraXPositionMethods[(int)CameraPositionMethod.VALUE] = sanitizeXPosValue;
        cameraXPositionMethods[(int)CameraPositionMethod.CENTER] = sanitizeXPosCenter;
        cameraXPositionMethods[(int)CameraPositionMethod.COLLIDER_MIN] = sanitizeXPosColliderMin;
        cameraXPositionMethods[(int)CameraPositionMethod.COLLIDER_MAX] = sanitizeXPosColliderMax;
        cameraXPositionMethods[(int)CameraPositionMethod.COLLIDER] = sanitizeXPosCollider;

        cameraYPositionMethods[(int)CameraPositionMethod.INHERIT] = sanitizePosInherit;
        cameraYPositionMethods[(int)CameraPositionMethod.VALUE] = sanitizeYPosValue;
        cameraYPositionMethods[(int)CameraPositionMethod.CENTER] = sanitizeYPosCenter;
        cameraYPositionMethods[(int)CameraPositionMethod.COLLIDER_MIN] = sanitizeYPosColliderMin;
        cameraYPositionMethods[(int)CameraPositionMethod.COLLIDER_MAX] = sanitizeYPosColliderMax;
        cameraYPositionMethods[(int)CameraPositionMethod.COLLIDER] = sanitizeYPosCollider;
    }


    public float sanitizeCameraSize(float size) {
        if(cameraSizeMethod == CameraSizeMethod.VALUE) return cameraSizeMethods[(int)cameraSizeMethod](size);
        return cameraSizeMethods[(int)cameraSizeMethod](size) * cameraSizeModifier;
    }

    public float sanitizeCameraXPos(float xPos) {
        if (cameraXPositionMethod == CameraPositionMethod.VALUE) return cameraXPositionMethods[(int)cameraXPositionMethod](xPos);
        if (
            cameraXPositionOverrideOffset && (
                cameraXPositionMethod == CameraPositionMethod.COLLIDER_MIN ||
                cameraXPositionMethod == CameraPositionMethod.COLLIDER_MAX ||
                cameraXPositionMethod == CameraPositionMethod.COLLIDER
            )
        ) {
            return cameraXPositionMethods[(int)cameraXPositionMethod](xPos + cameraXPositionOffset);
        }
        return cameraXPositionMethods[(int)cameraXPositionMethod](xPos) + cameraXPositionOffset;
    }

    public float sanitizeCameraYPos(float yPos) {
        if (cameraYPositionMethod == CameraPositionMethod.VALUE) return cameraYPositionMethods[(int)cameraYPositionMethod](yPos);
        if (
            cameraYPositionOverrideOffset && (
                cameraYPositionMethod == CameraPositionMethod.COLLIDER_MIN ||
                cameraYPositionMethod == CameraPositionMethod.COLLIDER_MAX ||
                cameraYPositionMethod == CameraPositionMethod.COLLIDER
            )
        ) {
            return cameraYPositionMethods[(int)cameraYPositionMethod](yPos + cameraYPositionOffset);
        }
        return cameraYPositionMethods[(int)cameraYPositionMethod](yPos) + cameraYPositionOffset;
    }

    float sanitizeCameraSizeValue(float size) {
        return cameraSizeValue;
    }

    float sanitizeCameraSizeInherit(float size) {
        return size;
    }

    float sanitizeCameraSizeColliderHeight(float size) {
        return coll.size.y * 0.5f;
    }

    float sanitizeCameraSizeColliderWidth(float size) {
        return coll.size.x / Camera.main.aspect * 0.5f;
    }

    float sanitizePosInherit(float pos) {
        return pos;
    }

    float sanitizeXPosValue(float xPos) {
        return cameraXPositionValue;
    }

    float sanitizeYPosValue(float yPos) {
        return cameraYPositionValue;
    }

    float sanitizeXPosCenter(float xPos) {
        return coll.bounds.center.x;
    }

    float sanitizeYPosCenter(float yPos) {
        return coll.bounds.center.y;
    }

    float sanitizeXPosColliderMin(float xPos) {
        Bounds colliderBounds = coll.bounds;
        Bounds cameraBounds = Camera.main.orthographicBounds();
        cameraBounds.center = new Vector3(xPos, cameraBounds.center.y, cameraBounds.center.z);
        float left = colliderBounds.min.x - cameraBounds.min.x;
        if (left > 0) return xPos + left;
        return xPos;
    }

    float sanitizeYPosColliderMin(float yPos) {
        Bounds colliderBounds = coll.bounds;
        Bounds cameraBounds = Camera.main.orthographicBounds();
        cameraBounds.center = new Vector3(cameraBounds.center.x, yPos, cameraBounds.center.z);
        float bottom = colliderBounds.min.y - cameraBounds.min.y;
        if (bottom > 0) return yPos + bottom;
        return yPos;
    }

    float sanitizeXPosColliderMax(float xPos) {
        Bounds colliderBounds = coll.bounds;
        Bounds cameraBounds = Camera.main.orthographicBounds();
        cameraBounds.center = new Vector3(xPos, cameraBounds.center.y, cameraBounds.center.z);
        float right = colliderBounds.max.x - cameraBounds.max.x;
        if (right < 0) return xPos + right;
        return xPos;
    }

    float sanitizeYPosColliderMax(float yPos) {
        Bounds colliderBounds = coll.bounds;
        Bounds cameraBounds = Camera.main.orthographicBounds();
        cameraBounds.center = new Vector3(cameraBounds.center.x, yPos, cameraBounds.center.z);
        float top = colliderBounds.max.y - cameraBounds.max.y;
        if (top < 0) return yPos + top;
        return yPos;
    }

    float sanitizeXPosCollider(float xPos) {
        Bounds colliderBounds = coll.bounds;
        Bounds cameraBounds = Camera.main.orthographicBounds();
        if (colliderBounds.size.x <= cameraBounds.size.x) return colliderBounds.center.x;
        cameraBounds.center = new Vector3(xPos, cameraBounds.center.y, cameraBounds.center.z);
        float left = colliderBounds.min.x - cameraBounds.min.x;
        float right = colliderBounds.max.x - cameraBounds.max.x;
        if (left > 0 && right < 0) return colliderBounds.center.x;
        if (left > 0) return xPos + left;
        if (right < 0) return xPos + right;
        return xPos;
    }

    float sanitizeYPosCollider(float yPos) {
        Bounds colliderBounds = coll.bounds;
        Bounds cameraBounds = Camera.main.orthographicBounds();
        if (colliderBounds.size.y <= cameraBounds.size.y) return colliderBounds.center.y;
        cameraBounds.center = new Vector3(cameraBounds.center.x, yPos, cameraBounds.center.z);
        float bottom = colliderBounds.min.y - cameraBounds.min.y;
        float top = colliderBounds.max.y - cameraBounds.max.y;
        if (bottom > 0 && top < 0) return colliderBounds.center.y;
        if (bottom > 0) return yPos + bottom;
        if (top < 0) return yPos + top;
        return yPos;
    }
}