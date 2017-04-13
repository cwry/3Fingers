using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;

public static class AnimationUtil{
    public static AnimationPromiseDictionary PlayOneShot(GameObject go, AnimationClip clip, Dictionary<string, Action> animationEventHandlers = null) {
        var promise = new Promise();
        clip.wrapMode = WrapMode.Once;
        var animation = go.AddComponent<Animation>();
        var animationEventHandlerComponent = go.AddComponent<AnimationEventHandler>();
        var animationInformation = new AnimationPromiseDictionary(GameAction.Create(promise, () => animation.Stop()));
        animationEventHandlerComponent.AnimationEvent += (e) => {
            Debug.Log("AnimationEvent " + e + " fired");
            animationInformation.FireAnimationEvent(e);
        };

        animation.AddClip(clip, clip.name);
        animation.clip = animation.GetClip(clip.name);
        animation.Play();

        var destroyCheckRoutine = CheckForObjectDestroyed(go, () => {
            promise.Reject(new Exception("Object Destroyed before Animation could finish"));
        });

        GlobalCoroutine.Instance.StartCoroutine(destroyCheckRoutine);

        GlobalCoroutine.OnGameObject(go).StartCoroutine(WaitForAnimation(animation, () => {
            UnityEngine.Object.Destroy(animation);
            UnityEngine.Object.Destroy(animationEventHandlerComponent);
            GlobalCoroutine.Instance.StopCoroutine(destroyCheckRoutine);
            promise.Resolve();
        }));

        return animationInformation;
    }

    static IEnumerator WaitForAnimation(Animation animation, Action onDone) {
        while (animation.isPlaying) {
            yield return null;
        }
        if (onDone != null) onDone();
    }

    static IEnumerator CheckForObjectDestroyed(GameObject go, Action onDestroyed) {
        while(go != null) {
            yield return null;
        }
        if(onDestroyed != null) onDestroyed();
    }
}

public class AnimationPromiseDictionary {
    public GameAction gameAction;

    event Action<string> AnimationEvent;
    event Action AnimationEnded;

    public AnimationPromiseDictionary(GameAction gameAction) {
        this.gameAction = gameAction;
        gameAction
            .Done(() => {
                if (AnimationEnded != null) AnimationEnded();
            });
    }

    internal void FireAnimationEvent(string name) {
        if (AnimationEvent != null) AnimationEvent(name);
    }

    public Promise this[string name] {
        get {
            if (name == null) return gameAction;
            var promise = new Promise();
            AnimationEvent += (e) => {
                if (name == e) {
                    promise.Resolve();
                }
            };

            AnimationEnded += () => {
                promise.Reject(new Exception("Animation Event " + name + " was subscribed to but never fired"));
            };

            return promise;
        }
    }
}
