using Spine.Unity;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "OAAnimation/Spine Animation")]
public class SpineOAAnimation : OAAnimation {
    public string animation;
    public float timeScale = 1;
    public bool loop;

    public override AnimationEventDispatcher Play(GameObject go) {
        var skeletonAnimation = go.GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation == null) {
            Debug.LogError("No SkeletonAnimation found in GameObject " + go.name);
            return null;
        }

        var gameAction = GameAction.Create(() => skeletonAnimation.AnimationName = null);
        var animationEventDispatcher = new AnimationEventDispatcher(gameAction);

        skeletonAnimation.loop = loop;
        skeletonAnimation.timeScale = timeScale;

        Action destroyCallback = () => gameAction.Resolve();

        ObjectDestroyEvent.Get(go).Destroy += destroyCallback;

        Spine.AnimationState.TrackEntryEventDelegate onEvent = (entry, e) => {
            if (entry.ToString() == animation) {
                animationEventDispatcher.FireAnimationEvent(e.data.name);
            }
        };
        skeletonAnimation.state.Event += onEvent;

        Spine.AnimationState.TrackEntryDelegate onEnd = (entry) => {
            if (entry.ToString() == animation) {
                gameAction.Resolve();
            }
        };
        skeletonAnimation.state.End += onEnd;

        Spine.AnimationState.TrackEntryDelegate onComplete = (entry) => {
            if (!loop) {
                skeletonAnimation.AnimationName = null;
            }
        };
        skeletonAnimation.state.Complete += onComplete;

        gameAction
            .Then(() => {
                ObjectDestroyEvent.Get(go).Destroy -= destroyCallback;
                skeletonAnimation.state.Event -= onEvent;
                skeletonAnimation.state.End -= onEnd;
                skeletonAnimation.state.Complete -= onComplete;
            });

        if (skeletonAnimation.AnimationName != animation) skeletonAnimation.AnimationName = animation;
        return animationEventDispatcher;
    }
}
