using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Collections.Generic;
using RSG;
using UnityEngine;

public class OpenDoor : Interaction<IPromise> {
    public Transform characterUnlockPosition;
    public Transform characterWalkThroughDoorPosition;
    public GameObject door;
    public OAAnimation doorOpen;


    public override bool Condition() {
        return Inventory.HasItem("key");
    }

    public override IPromise Execute() {
        return PlayerController.Instance.MoveTo(characterUnlockPosition.position.x)
            .Then(() => doorOpen.Play(door).Promise("end"))
            .Then(() => PlayerController.Instance.MoveTo(characterWalkThroughDoorPosition.position.x))
            .Then(() => {
                OAUtil.CompleteLevel(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene("Menu");
            })
            .Catch((e) => Debug.LogError(e));
    }
}
