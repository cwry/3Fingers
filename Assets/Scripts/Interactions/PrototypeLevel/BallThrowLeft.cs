using RSG;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallThrowLeft : Interaction<IPromise> {
        public GameObject ball;
        public OAAnimation ballThrowAnimation;

        Vector3? initBallPosition = null;

        public override bool Condition() {
            return Inventory.HasItem("ball");
        }

        public override IPromise Execute() {
            if (initBallPosition == null) initBallPosition = ball.transform.position;
            ball.transform.position = (Vector3)initBallPosition;
            return PlayerController.Instance.MoveTo(ball.transform.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.RIGHT;
                    Inventory.RemoveItem("ball");
                    ball.SetActive(true);
                    return ballThrowAnimation.Play(ball).Promise("end");
                });
        }
    }
}
