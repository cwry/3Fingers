using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization = MemberSerialization.Fields)]
public class SaveState {
    const string EXPECTED_INTEGRITY_STRING = "catsanddogs";
    const int TOTAL_SLOTS = 3;
    static SaveState _instance;
    public static SaveState Instance {
        get {
            if (_instance == null) _instance = FromDisk();
            return _instance;
        }
    }

    string integrityString = "";

    public StarDictionary levelStars = new StarDictionary();

    static SaveState FromDisk(int slot = 0) {
        var json = PlayerPrefs.GetString("save_state_" + slot, "");
        if (json != "") {
            var saveState = JsonConvert.DeserializeObject<SaveState>(json);
            if (saveState != null && saveState.integrityString == EXPECTED_INTEGRITY_STRING) {
                if (saveState.levelStars == null) saveState.levelStars = new StarDictionary();
                return saveState;
            }
        }
        if (slot < TOTAL_SLOTS - 1) return FromDisk(slot + 1);
        return new SaveState();
    }

    public void Save() {
        integrityString = EXPECTED_INTEGRITY_STRING;
        var json = JsonConvert.SerializeObject(this);
        for(int i = 0; i < TOTAL_SLOTS - 1; i++) {
            var oldSave = PlayerPrefs.GetString("save_state_" + i, "");
            if (oldSave != "") PlayerPrefs.SetString("save_state_" + (i + 1), oldSave);
        }
        PlayerPrefs.SetString("save_state_" + 0, json);
        integrityString = "";
    }
}

public class StarDictionary : Dictionary<string, int> {
    public new int this[string flag] {
        get {
            string key = flag.ToUpper();
            if (!ContainsKey(key)) return 0;
            return base[key];
        }
        set {
            base[flag.ToUpper()] = value;
        }
    }
}
