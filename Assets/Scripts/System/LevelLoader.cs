using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {
    static OALevel levelToLoad;

    public Image loadingScreenImage;
    public Text loadingText;

    public static void LoadLevel(OALevel level) {
        levelToLoad = level;
        SceneManager.LoadScene("LoadingScreen");
    }

    void Start() {
        if (levelToLoad == null) {
            SceneManager.LoadScene("Menu");
            return;
        }

        loadingScreenImage.sprite = levelToLoad.loadingScreenImage;

        var asyncOp = SceneManager.LoadSceneAsync(levelToLoad.sceneName);
        asyncOp.allowSceneActivation = false;
        StartCoroutine(WaitForLoad(asyncOp));
    }

    IEnumerator WaitForLoad(AsyncOperation asyncOp) {
        while (asyncOp.progress < 0.9f) {
            loadingText.text = "LOADING - " + (int)(asyncOp.progress * 100) + "%";
            yield return 0;
        }
        loadingText.text = "READY - Tap to continue";
        while (!Input.GetMouseButtonDown(0)) {
            yield return 0;
        }
        asyncOp.allowSceneActivation = true;
    }
}
