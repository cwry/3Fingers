using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallPickup : Interaction<IPromise> {
        public GameObject ball;
        public OAAnimation characterStoreInventoryAnimation;

        public override IPromise Execute() {
            return PlayerController.Instance.MoveTo(ball.transform.position.x)
                .Then(() => {
                    var removingHoldingBall = PlayerController.Instance.SetHoldingObject(ball);
                    var storeInventory = characterStoreInventoryAnimation.Play(PlayerController.Instance.gameObject);
                    return Promise.All(
                        storeInventory.Promise("inventory")
                            .Then(() => {
                                removingHoldingBall();
                                ball.SetActive(false);
                                Inventory.AddItem("ball");
                            }),

                        storeInventory.Promise("end")
                    );
                });
        }
    }
}
