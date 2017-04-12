using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Flags{
    static FlagDictionary _global;
    public static FlagDictionary Global {
        get {
            if (_global == null) _global = new FlagDictionary();
            return _global;
        }
    }

    static SceneFlagDictionary _scene;
    public static SceneFlagDictionary Scene {
        get {
            if (_scene == null) _scene = new SceneFlagDictionary();
            return _scene;
        }
    }
}

public class FlagDictionary : Dictionary<string, bool> {
    public new bool this[string flag] {
        get {
            string key = flag.ToUpper();
            if (!ContainsKey(key)) return false;
            return base[key];
        }
        set {
            base[flag.ToUpper()] = value;
        }
    }
}

public class SceneFlagDictionary : FlagDictionary {
    public SceneFlagDictionary() {
        SceneManager.activeSceneChanged += (oldScene, newScene) => {
            Clear();
        };
    }
}
