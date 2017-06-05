using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class KeyPickup : Interaction<IPromise> {
        public GameObject key;
        public OAAnimation characterStoreInventoryAnimation;

        public override IPromise Execute() {
            return PlayerController.Instance.MoveTo(key.transform.position.x)
                .Then(() => {
                    var removingHoldingKey = PlayerController.Instance.SetHoldingObject(key);
                    var storeInventory = characterStoreInventoryAnimation.Play(PlayerController.Instance.gameObject);
                    return Promise.All(
                        storeInventory.Promise("inventory")
                            .Then(() => {
                                removingHoldingKey();
                                key.SetActive(false);
                                Inventory.AddItem("key");
                                Achievments.NumStars++;
                            }),

                        storeInventory.Promise("end")
                    );
                });
        }
    }

}