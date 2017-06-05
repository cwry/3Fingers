using RSG;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BananaFromBarrel : Interaction<IPromise> {
        public OAAnimation characterSearchBarrelAnimation;
        public Transform searchPosition;
        public GameObject charBanana;
        public OAAnimation characterStoreInventoryAnimation;

        public override IPromise Execute() {
            return PlayerController.Instance.MoveTo(searchPosition.position.x)
                .Then(() => {
                    PlayerController.Instance.LookDirection = LookDirection.RIGHT;
                    PlayerController.Instance.SortingLayerName = "Background_High";
                    return characterSearchBarrelAnimation.Play(PlayerController.Instance.gameObject).Promise("end");
                })
                .Then(() => {
                    PlayerController.Instance.SortingLayerName = "Default";
                    var removeHoldingObject = PlayerController.Instance.SetHoldingObject(charBanana);
                    var storeInventory = characterStoreInventoryAnimation.Play(PlayerController.Instance.gameObject);
                    return Promise.All(
                        storeInventory.Promise("inventory")
                            .Then(() => {
                                removeHoldingObject();
                                Achievments.NumStars++;
                            }),

                        storeInventory.Promise("end")
                    );
                })
                .Then(() => {
                    enabled = false;
                });
        }
    }
}
