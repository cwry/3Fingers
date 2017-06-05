using RSG;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class TakeFlowerPot : Interaction<IPromise> {
        public GameObject flowerPot;
        public OAAnimation playerTakeObjectAnimation;
        public Transform walkPosition;

        public override IPromise Execute() {
            return PlayerController.Instance.MoveTo(walkPosition.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.LEFT;
                    var removeHoldingObject = PlayerController.Instance.SetHoldingObject(flowerPot);
                    var takeObjectProc = playerTakeObjectAnimation.Play(PlayerController.Instance.gameObject);
                    return Promise.All(
                        takeObjectProc.Promise("inventory")
                            .Then(() => {
                                removeHoldingObject();
                                flowerPot.SetActive(false);
                                Achievments.NumStars++;
                            }),

                        takeObjectProc.Promise("end")
                    );
                })
                .Then(() => {
                    gameObject.SetActive(false);
                });
        }
    }
}
