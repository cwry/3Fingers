using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using RSG;

public static partial class TweenUtil{
    public static GameAction TweenPathByTime(MonoBehaviour behaviour, Vector2[] path, float time, Func<float, float> ease = null) {
        var promise = new Promise();
        var routine = TweenPathEnumerator(behaviour.gameObject, path, time, null, ease, () => promise.Resolve());
        behaviour.StartCoroutine(routine);
        return GameAction.Create(promise, () => {
            if (behaviour != null) behaviour.StopCoroutine(routine);
        });
    }

    public static GameAction TweenPathBySpeed(MonoBehaviour behaviour, Vector2[] path, float speed, Func<float, float> ease = null) {
        var promise = new Promise();
        var routine = TweenPathEnumerator(behaviour.gameObject, path, 0, speed, ease, () => promise.Resolve());
        behaviour.StartCoroutine(routine);
        return GameAction.Create(promise, () => {
            if (behaviour != null) behaviour.StopCoroutine(routine);
        });
    }

    static IEnumerator TweenPathEnumerator(GameObject go, Vector2[] path, float time, float? speed = null, Func<float, float> ease = null, Action onDone = null) {
        if (path == null || path.Length <= 1) {
            if(onDone != null) onDone();
            yield break;
        }

        if (ease == null) ease = EasingFunctions.Linear;

        float[] distances = Enumerable
            .Range(0, path.Length)
            .Select(i => {
                if (i == 0) return 0;
                return (path[i] - path[i - 1]).magnitude;
            })
            .Scan((state, distance) => state + distance, 0f)
            .Skip(1)
            .ToArray();

        float totalDistance = distances[distances.Length - 1];

        if(speed != null) time = totalDistance / (float)speed;

        float[] progresses = distances
            .Select(distance => distance / totalDistance)
            .ToArray();

        float progress = 0;

        while(progress < 1) {
            float easedProgress = Mathf.Clamp01(ease(progress));
            int tweenIndex = Enumerable
                .Range(0, progresses.Length)
                .First(i => easedProgress >= progresses[i] && easedProgress <= progresses[i + 1]);

            float delta = (easedProgress - progresses[tweenIndex]) / (progresses[tweenIndex + 1] - progresses[tweenIndex]);
            Vector2 pos = Vector2.Lerp(path[tweenIndex], path[tweenIndex + 1], delta);
            go.transform.position = pos;

            progress += Time.deltaTime / time;
            yield return null;
        }
        go.transform.position = path[path.Length - 1];

        if (onDone != null) onDone();
    }
}
