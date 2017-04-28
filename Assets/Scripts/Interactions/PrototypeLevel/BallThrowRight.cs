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
            return Inventory.Instance.HasItem("ball");
        }

        public override void OnTrigger() {
            GameActionHandler.Instance.SetCurrent(
                PlayerController.Instance.MoveTo(ball.transform.position.x)
                    .Then(() => {
                        PlayerController.Instance.LookDirection = LookDirection.LEFT;
                        Inventory.Instance.RemoveItem("ball");
                        ball.SetActive(true);

                        var ballThrow = AnimationUtil.PlayOneShot(ball, ballThrowAnimation);
                        return Promise.All(
                            ballThrow["hit_roof"]
                                .Then(() => AnimationUtil.PlayOneShot(bird, birdFlyAnimation)[null])
                                .Then(() => AnimationUtil.PlayOneShot(key, keyFallAnimation)[null]),

                            ballThrow[null]
                        );
                    })
            );
        }
    }
}
