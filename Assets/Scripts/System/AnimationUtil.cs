using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;
using Spine;
using Spine.Unity;

public static class AnimationUtil{
    public static AnimationEventDispatcher PlaySpine(GameObject go, SpineAnimationDescription spineAnimation, Func<float> updateTimeScale = null) {
        var skeletonAnimation = go.GetComponentInChildren<SkeletonAnimation>();
        if(skeletonAnimation == null) {
            Debug.LogError("No SkeletonAnimation found in GameObject " + go.name);
            return null;
        }

        var promise = new Promise();
        var animationEventDispatcher = new AnimationEventDispatcher(GameAction.Create(promise, () => skeletonAnimation.name = null));

        skeletonAnimation.loop = spineAnimation.loop;
        skeletonAnimation.timeScale = spineAnimation.timeScale;

        var destroyCheckRoutine = CheckForObjectDestroyed(go, () => {
            promise.Reject(new Exception("Object Destroyed before Animation could finish"));
        });

        GlobalCoroutine.Instance.StartCoroutine(destroyCheckRoutine);

        promise.Done(() => GlobalCoroutine.Instance.StopCoroutine(destroyCheckRoutine));

        Spine.AnimationState.TrackEntryEventDelegate onEvent = (entry, e) => {
            animationEventDispatcher.FireAnimationEvent(e.data.name);
        };

        skeletonAnimation.state.Event += onEvent;

        Spine.AnimationState.TrackEntryDelegate onEnd = null;
        onEnd = (entry) => {
            skeletonAnimation.state.Event -= onEvent;
            skeletonAnimation.state.End -= onEnd;
            promise.Resolve();
        };

        skeletonAnimation.state.End += onEnd;

        if (skeletonAnimation.AnimationName != spineAnimation.animation) skeletonAnimation.AnimationName = spineAnimation.animation;
        return animationEventDispatcher;
    }

    public static AnimationEventDispatcher PlayOneShot(GameObject go, AnimationClip clip) {
        var promise = new Promise();
        clip.wrapMode = WrapMode.Once;
        var animation = go.AddComponent<Animation>();
        var animationEventHandlerComponent = go.AddComponent<AnimationEventHandler>();
        var animationInformation = new AnimationEventDispatcher(GameAction.Create(promise, () => animation.Stop()));
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

    static IEnumerator WaitForSpineAnimation(SkeletonAnimation animation, string name, Action onDone) {
        while (animation.name == name) {
            yield return null;
        }
        if (onDone != null) onDone();
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

public class AnimationEventDispatcher {
    public GameAction gameAction;

    Dictionary<string, List<Action>> listeners = new Dictionary<string, List<Action>>();

    public AnimationEventDispatcher(GameAction gameAction) {
        this.gameAction = gameAction;
        gameAction
            .Done(() => {
                FireAnimationEvent("end");
                listeners.Clear();
            });
    }

    internal void FireAnimationEvent(string eventName) {
        if (listeners.ContainsKey(eventName)) {
            foreach(var listener in listeners[eventName].ToArray()) {  //toarray to copy the list (so listeners can unsubscribe during loop)
                listener();
            }
        }
    }

    public Action On(string eventName, Action cb) {
        if (!listeners.ContainsKey(eventName)) {
            listeners[eventName] = new List<Action>();
        }

        listeners[eventName].Add(cb);

        return () => Off(eventName, cb);
    }

    public void Off(string eventName, Action cb) {
        if (listeners.ContainsKey(eventName)) {
            listeners[eventName].Remove(cb);
        }
    }

    public IPromise Promise(string eventName) {
        if (eventName == null || eventName == "end") return gameAction;
        var promise = new Promise();

        Action off = null;
        Action offEnd = null;

        off = On(eventName, () => {
            promise.Resolve();
            off();
            offEnd();
        });

        offEnd = On("end", () => {
            promise.Reject(new Exception("Animation Event " + eventName + " was promised but never fired"));
            off();
            offEnd();
        });

        return promise;
    }
}
