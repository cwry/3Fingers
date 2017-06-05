using UnityEngine;
using RSG;
using System;
using System.Collections.Generic;

namespace Interactions.PrototypeLevel {
    public class BallThrowRight : Interaction<IPromise> {
        public GameObject ball;
        public OAAnimation ballThrowAnimation;
        public OAAnimation characterThrowAnimation;
        public GameObject bird;
        public OAAnimation birdFlyAnimation;
        public GameObject key;
        public OAAnimation keyFallAnimation;

        public override bool Condition() {
            return Inventory.HasItem("ball");
        }

        public override IPromise Execute() {
            Action removeHoldingBall = null;
            return PlayerController.Instance.MoveTo(ball.transform.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.LEFT;
                    Inventory.RemoveItem("ball");
                    ball.SetActive(true);

                    removeHoldingBall = PlayerController.Instance.SetHoldingObject(ball);
                    return characterThrowAnimation.Play(PlayerController.Instance.gameObject).Promise("throw_start");
                })
                .Then(() => {
                    removeHoldingBall(); 
                    CameraController.Instance.target = ball;
                    var ballThrow = ballThrowAnimation.Play(ball);
                    return Promise.All(
                        ballThrow.Promise("hit_roof")
                            .Then(() => {
                                CameraController.Instance.target = bird;
                                return birdFlyAnimation.Play(bird).Promise("end");
                            })
                            .Then(() => {
                                CameraController.Instance.target = key;
                                return keyFallAnimation.Play(key).Promise("end");
                            })
                            .Then(() => PromiseDelay.Get(0.5f))
                            .Then(() => CameraController.Instance.target = PlayerController.Instance.gameObject),

                            ballThrow.Promise("end")
                    );
                });
        }
    }
}
