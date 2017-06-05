using Spine.Unity;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "OAAnimation/Spine Animation")]
public class SpineOAAnimation : OAAnimation {
    public string animation;
    public float timeScale = 1;
    public bool loop;
    public SpineOAAnimation exitAnimation;

    public override AnimationEventDispatcher Play(GameObject go) {
        var skeletonAnimation = go.GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation == null) {
            Debug.LogError("No SkeletonAnimation found in GameObject " + go.name);
            return null;
        }

        var gameAction = GameAction.Create(() => { }); //cancel invokes resolve() - exit logic is handled by then()
        var animationEventDispatcher = new AnimationEventDispatcher(gameAction);

        var animationEntry = skeletonAnimation.AnimationState.SetAnimation(0, animation, loop);
        animationEntry.TimeScale = timeScale;

        Action destroyCallback = () => gameAction.Resolve();

        ObjectDestroyEvent.Get(go).Destroy += destroyCallback;

        Spine.AnimationState.TrackEntryEventDelegate onEvent = (entry, e) => {
            if (entry.ToString() == animation) {
                animationEventDispatcher.FireAnimationEvent(e.data.name);
            }
        };
        animationEntry.Event += onEvent;

        Spine.AnimationState.TrackEntryDelegate onEnd = (entry) => {
            gameAction.Resolve();
        };
        animationEntry.End += onEnd;

        Spine.AnimationState.TrackEntryDelegate onComplete = (entry) => {
            if (!loop) {
                gameAction.Resolve();
            }
        };
        animationEntry.Complete += onComplete;

        gameAction
            .Then(() => {
                ObjectDestroyEvent.Get(go).Destroy -= destroyCallback;
                animationEntry.Event -= onEvent;
                animationEntry.End -= onEnd;
                animationEntry.Complete -= onComplete;
                PlayExitAnimation(skeletonAnimation);
            });

        return animationEventDispatcher;
    }

    void PlayExitAnimation(SkeletonAnimation skeletonAnimation) {
        if (exitAnimation != null) {
            skeletonAnimation.AnimationState.SetAnimation(0, exitAnimation.animation, exitAnimation.loop).TimeScale = exitAnimation.timeScale;
        }else{
            skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0.5f);
        }
    }
}
