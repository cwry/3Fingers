using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionStarIndicator : MonoBehaviour {
    public Image[] starIndicators;

    int _numStars = 0;
    public int NumStars {
        get {
            return _numStars;
        }

        set {
            if (value > starIndicators.Length) {
                _numStars = starIndicators.Length;
            }else {
                _numStars = value;
            }
            UpdateIndicators();
        }
    }

    void UpdateIndicators() {
        for(int i = 0; i < starIndicators.Length; i++) {
            starIndicators[i].color = i < NumStars ? Color.white : Color.black;
        }
    }
}
