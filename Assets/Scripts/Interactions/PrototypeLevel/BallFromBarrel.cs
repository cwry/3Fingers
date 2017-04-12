using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallFromBarrel : Interaction {
        bool exhausted = false;

        public override bool Condition() {
            return !exhausted;
        }

        public override void OnTrigger() {
            exhausted = true;
            Flags.Scene["HasBall"] = true;
        }
    }
}
