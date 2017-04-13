using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallThrowLeft : Interaction {
        public GameObject ball;
        public AnimationClip ballThrowAnimation;

        Vector3? initBallPosition = null;

        public override bool Condition() {
            return Flags.Scene["HasBall"];
        }

        public override void OnTrigger() {
            if(initBallPosition == null) initBallPosition = ball.transform.position;
            ball.transform.position = (Vector3)initBallPosition;
            GameActionHandler.Instance.Execute(
                PlayerController.Instance.MoveTo(ball.transform.position.x)
                    .Then(() => {
                        PlayerController.Instance.LookDirection = LookDirection.RIGHT;
                        Flags.Scene["HasBall"] = false;
                        ball.SetActive(true);
                        return AnimationUtil.PlayOneShot(ball, ballThrowAnimation)[null];
                    })
            );
        }
    }
}
