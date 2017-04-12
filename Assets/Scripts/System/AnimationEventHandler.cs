using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {

    public event Action<string> AnimationEvent;

	public void HandleAnimationEvent(string name) {
        if (AnimationEvent != null) AnimationEvent(name);
    }
}
