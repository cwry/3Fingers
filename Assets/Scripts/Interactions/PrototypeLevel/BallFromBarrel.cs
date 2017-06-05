using RSG;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallFromBarrel : Interaction<IPromise> {
        public OAAnimation characterSearchBarrelAnimation;
        public Transform searchPosition;
        public GameObject charBall;
        public OAAnimation characterStoreInventoryAnimation;
        public Interaction bananaFromBarrelInteraction;

        public override IPromise Execute() {
            return PlayerController.Instance.MoveTo(searchPosition.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.RIGHT;
                    PlayerController.Instance.SortingLayerName = "Background_High";
                    return characterSearchBarrelAnimation.Play(PlayerController.Instance.gameObject).Promise("end");
                })
                .Then(() => {
                    PlayerController.Instance.SortingLayerName = "Default";
                    var removeHoldingObject = PlayerController.Instance.SetHoldingObject(charBall);
                    var storeInventory = characterStoreInventoryAnimation.Play(PlayerController.Instance.gameObject);
                    return Promise.All(
                        storeInventory.Promise("inventory")
                            .Then(() => {
                                removeHoldingObject();
                                Inventory.AddItem("ball");
                            }),

                        storeInventory.Promise("end")
                    );
                })
                .Then(() => {
                    enabled = false;
                    bananaFromBarrelInteraction.enabled = true;
                });
        }
    }
}
