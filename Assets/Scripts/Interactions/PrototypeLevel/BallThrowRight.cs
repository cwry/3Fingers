using UnityEngine;
using RSG;
using System;
using System.Collections.Generic;

namespace Interactions.PrototypeLevel {
    public class BallThrowRight : Interaction {
        public GameObject ball;
        public AnimationClip ballThrowAnimation;
        public GameObject bird;
        public AnimationClip birdFlyAnimation;
        public GameObject key;
        public AnimationClip keyFallAnimation;

        public override bool Condition() {
            return Flags.Scene["HasBall"];
        }

        public override void OnTrigger() {
            Flags.Scene["HasBall"] = false;
            ball.SetActive(true);

            var promise = new Promise();
            AnimationUtil.PlayOneShot(ball, ballThrowAnimation, 
                new Dictionary<string, Action>() {
                    { "hit_roof", () => {
                        AnimationUtil.PlayOneShot(bird, birdFlyAnimation)
                            .Then(() => {
                                return AnimationUtil.PlayOneShot(key, keyFallAnimation);
                            })
                            .Done(() => {
                                promise.Resolve();
                            });
                    } }
                });

            GameActionHandler.Instance.Execute(promise);
        }
    }
}
