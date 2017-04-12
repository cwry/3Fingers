using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenDoor : Interaction {

    public override bool Condition() {
        return Flags.Scene["HasKey"];
    }

    public override void OnTrigger() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
