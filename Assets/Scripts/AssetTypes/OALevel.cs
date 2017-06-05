using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "OALevel")]
public class OALevel : ScriptableObject {
    public string levelName;
    public string sceneName;
    public Sprite loadingScreenImage;

    public int NumStars{
        get {
            return SaveState.Instance.levelStars[sceneName];
        }
    }
}
