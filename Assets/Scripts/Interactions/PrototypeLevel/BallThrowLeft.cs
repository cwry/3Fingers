using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallThrowLeft : Interaction {
        public GameObject ball;
        public AnimationClip ballThrowAnimation;

        public override bool Condition() {
            return Flags.Scene["HasBall"];
        }

        public override void OnTrigger() {
            Flags.Scene["HasBall"] = false;
            ball.SetActive(true);
            GameActionHandler.Instance.Execute(GameAction.Create(AnimationUtil.PlayOneShot(ball, ballThrowAnimation)));
        }
    }
}
