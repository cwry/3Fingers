﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public AnimationClip clip;

    void Awake() {
        AnimationUtil.PlayOneShot(gameObject, clip);
    }
}
