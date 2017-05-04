using UnityEngine.SceneManagement;

public class OpenDoor : Interaction {

    public override bool Condition() {
        return Inventory.HasItem("key");
    }

    public override void OnTrigger() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
