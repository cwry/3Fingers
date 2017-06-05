using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorController : MonoBehaviour {
    public OALevel level;
    public OALevel dependencyLevel;

    public LevelSelectionStarIndicator starIndicator;
    public Text levelText;
    public Button button;

    void Awake() {
        if (dependencyLevel != null && dependencyLevel.NumStars <= 0) {
            levelText.text = "LOCKED";
            button.interactable = false;
        }else {
            levelText.text = level.levelName;
            button.onClick.AddListener(() => {
                LevelLoader.LoadLevel(level);
            });
        }

        starIndicator.NumStars = level.NumStars;
    }
}
