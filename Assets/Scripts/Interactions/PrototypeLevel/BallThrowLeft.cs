using RSG;
using System;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallThrowLeft : Interaction<IPromise> {
        public GameObject ball;
        public OAAnimation ballThrowAnimation;
        public OAAnimation characterThrowAnimation;

        Vector3? initBallPosition = null;

        public override bool Condition() {
            return Inventory.HasItem("ball");
        }

        public override IPromise Execute() {
            if (initBallPosition == null) initBallPosition = ball.transform.position;
            Action removeHoldingBall = null;
            ball.transform.position = (Vector3)initBallPosition;
            return PlayerController.Instance.MoveTo(ball.transform.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.RIGHT;
                    Inventory.RemoveItem("ball");
                    ball.SetActive(true);
                    removeHoldingBall = PlayerController.Instance.SetHoldingObject(ball);
                    return characterThrowAnimation.Play(PlayerController.Instance.gameObject).Promise("throw_start");
                })
                .Then(() => {
                    removeHoldingBall();
                    CameraController.Instance.target = ball;
                    return ballThrowAnimation.Play(ball).Promise("end");
                })
                .Then(() => {
                    CameraController.Instance.target = PlayerController.Instance.gameObject;
                });
        }
    }
}
