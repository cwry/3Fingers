using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OAAnimation : ScriptableObject{
    public abstract AnimationEventDispatcher Play(GameObject go);
}
