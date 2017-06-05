using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public static class OAUtil{
    public static void CompleteLevel(string name) {
        int previousStars = SaveState.Instance.levelStars[name];
        Analytics.CustomEvent("levelCompleted", new Dictionary<string, object>() {
            {"levelName", name},
            {"wasCompleted", previousStars >= 0},
            {"previousStars", previousStars},
            {"stars", Achievments.NumStars }
        });

        SaveState.Instance.levelStars[name] = Achievments.NumStars;
        SaveState.Instance.Save();
    }
}
