
using UnityEngine;

public static class EasingFunctions{
    public static float Linear(float t) {
        return t;
    }

    public static float InQuad(float t) {
        return t * t;
    }

    public static float OutQuad(float t) {
        return -t * (t - 2);
    }

    public static float InOutQuad(float t) {
        t *= 2;
        if (t < 1) return 0.5f * t * t;
        t--;
        return -0.5f * (t * (t - 2) - 1);
    }

    public static float InOutSin(float t) {
        return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1);
    }
}
