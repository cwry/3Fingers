using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievments : MonoBehaviour {
    static Achievments _instance;
    static Achievments Instance {
        get {
            if (_instance == null) _instance = (Instantiate(Resources.Load("GameUI")) as GameObject).GetComponent<Achievments>();
            return _instance;
        }
    }

    int _numStars;
    public static int NumStars{
        get {
            return Instance._numStars;
        }
        set {
            if(value > Instance.stars.Length) {
                Instance._numStars = Instance.stars.Length;
            }else {
                Instance._numStars = value;
            }
            Instance.UpdateStars();
        }
    }

    [SerializeField]
    Image[] stars;

    void Awake() {
        if (_instance == null) _instance = this;
    }

    void UpdateStars() {
        for(int i = 0; i < stars.Length; i++) {
            StartCoroutine(ColorStar(stars[i], i < NumStars ? Color.white : Color.black, 0.5f));
        }
    }

    IEnumerator ColorStar(Image star, Color clr, float dur) {
        Color startClr = star.color;
        float time = 0;
        while (time < dur) {
            yield return 0;
            time += Time.deltaTime;
            star.color = Color.Lerp(startClr, clr, time / dur);
        }
    }
}
