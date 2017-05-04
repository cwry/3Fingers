using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class KeyPickup : Interaction {
        public GameObject key;

        public override void OnTrigger() {
            key.SetActive(false);
            Inventory.AddItem("key");
        }
    }

}