using RSG;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class OpenWindow : Interaction<IPromise> {
        public GameObject window;
        public OAAnimation openWindowAnimation;
        public Transform walkPositon;
        public Interaction takeFlowerPotInteraction;

        public override IPromise Execute() {
            return openWindowAnimation.Play(window).Promise("end")
                .Then(() => PromiseDelay.Get(0.25f))
                .Then(() => PlayerController.Instance.MoveTo(walkPositon.position.x))
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.LEFT;
                    enabled = false;
                    takeFlowerPotInteraction.enabled = true;
                });
        }
    }
}
