using UnityEngine;
using RSG;
using System;
using System.Collections.Generic;

namespace Interactions.PrototypeLevel {
    public class BallThrowRight : Interaction<IPromise> {
        public GameObject ball;
        public OAAnimation ballThrowAnimation;
        public GameObject bird;
        public OAAnimation birdFlyAnimation;
        public GameObject key;
        public OAAnimation keyFallAnimation;

        public override bool Condition() {
            return Inventory.HasItem("ball");
        }

        public override IPromise Execute() {
            return PlayerController.Instance.MoveTo(ball.transform.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.LEFT;
                    Inventory.RemoveItem("ball");
                    ball.SetActive(true);

                    var ballThrow = ballThrowAnimation.Play(ball);
                    return Promise.All(
                        ballThrow.Promise("hit_roof")
                            .Then(() => birdFlyAnimation.Play(bird).Promise("end"))
                            .Then(() => keyFallAnimation.Play(key).Promise("end")),

                        ballThrow.Promise("end")
                    );
                });
        }
    }
}
