using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using RSG;

public static partial class TweenEnumerators{
    public static GameAction TweenPathByTime(MonoBehaviour behaviour, Vector2[] path, float time) {
        var promise = new Promise();
        var routine = TweenPathEnumerator(behaviour.gameObject, path, time, null, () => promise.Resolve());
        behaviour.StartCoroutine(routine);
        return GameAction.Create(promise, () => behaviour.StopCoroutine(routine));
    }

    public static GameAction TweenPathBySpeed(MonoBehaviour behaviour, Vector2[] path, float speed) {
        var promise = new Promise();
        var routine = TweenPathEnumerator(behaviour.gameObject, path, 0, speed, () => promise.Resolve());
        behaviour.StartCoroutine(routine);
        return GameAction.Create(promise, () => behaviour.StopCoroutine(routine));
    }

    static IEnumerator TweenPathEnumerator(GameObject go, Vector2[] path, float time, float? speed = null, Action onDone = null) {
        if (path == null || path.Length <= 1) {
            if(onDone != null) onDone();
            yield break;
        }

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
            int tweenIndex = Enumerable
                .Range(0, progresses.Length)
                .First(i => progress >= progresses[i] && progress < progresses[i + 1]);

            float delta = (progress - progresses[tweenIndex]) / (progresses[tweenIndex + 1] - progresses[tweenIndex]);
            Vector2 pos = Vector2.Lerp(path[tweenIndex], path[tweenIndex + 1], delta);
            go.transform.position = pos;

            progress += Time.deltaTime / time;
            yield return null;
        }
        go.transform.position = path[path.Length - 1];

        if (onDone != null) onDone();
    }
}
