using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;

public static class AnimationUtil{
    public static GameAction PlayOneShot(GameObject go, AnimationClip clip, Dictionary<string, Action> animationEventHandlers = null) {
        var promise = new Promise();
        clip.wrapMode = WrapMode.Once;
        var animation = go.AddComponent<Animation>();
        var animationEventHandlerComponent = go.AddComponent<AnimationEventHandler>();
        animationEventHandlerComponent.AnimationEvent += (e) => {
            Debug.Log("AnimationEvent " + e + " fired");
            if (
                animationEventHandlers != null &&
                animationEventHandlers.ContainsKey(e) && 
                animationEventHandlers[e] != null
            ) {
                animationEventHandlers[e]();
            }
        };
        animation.AddClip(clip, clip.name);
        animation.clip = animation.GetClip(clip.name);
        animation.Play();
        GlobalCoroutine.OnGameObject(go).StartCoroutine(WaitForAnimation(animation, () => {
            UnityEngine.Object.Destroy(animation);
            promise.Resolve();
        }));
        return GameAction.Create(promise, () => animation.Stop());
    }

    static IEnumerator WaitForAnimation(Animation animation, Action onDone) {
        while (animation.isPlaying) {
            yield return null;
        }
        if (onDone != null) onDone();
    }
}
