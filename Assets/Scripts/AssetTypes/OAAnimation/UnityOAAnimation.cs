using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "OAAnimation/Unity Animation")]
public class UnityOAAnimation : OAAnimation {
    public AnimationClip clip;

    public override AnimationEventDispatcher Play(GameObject go) {
        clip.wrapMode = WrapMode.Once;
        var animation = go.AddComponent<Animation>();
        var animationEventHandlerComponent = go.AddComponent<AnimationEventHandler>();
        var gameAction = GameAction.Create(() => animation.Stop());
        var animationInformation = new AnimationEventDispatcher(gameAction);
        animationEventHandlerComponent.AnimationEvent += (e) => {
            animationInformation.FireAnimationEvent(e);
        };

        animation.AddClip(clip, clip.name);
        animation.clip = animation.GetClip(clip.name);
        animation.Play();

        Action destroyCallback = () => gameAction.Resolve();

        ObjectDestroyEvent.Get(go).Destroy += destroyCallback;

        GlobalCoroutine.OnGameObject(go).StartCoroutine(WaitForAnimation(animation, () => {
            Destroy(animation);
            Destroy(animationEventHandlerComponent);
            ObjectDestroyEvent.Get(go).Destroy -= destroyCallback;
            gameAction.Resolve();
        }));

        return animationInformation;
    }

    static IEnumerator WaitForAnimation(Animation animation, Action onDone) {
        while (animation.isPlaying) {
            yield return null;
        }
        if (onDone != null) onDone();
    }
}
