using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.PrototypeLevel {
    public class BallPickup : Interaction {
        public GameObject ball;

        public override void OnTrigger() {
            ball.SetActive(false);
            Inventory.AddItem("ball");
        }
    }
}
