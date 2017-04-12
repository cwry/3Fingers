using UnityEngine;

public abstract class Interaction : MonoBehaviour {
    public Interactable interactable;

    void Awake() {
        if(interactable != null) {
            interactable.conditional = Condition;
            interactable.Triggered += OnTrigger;
        }else {
            Debug.LogError("No Interactbale specified at " + gameObject.name);
        }
    }

    public virtual bool Condition() {
        return true;
    }

    public virtual void OnTrigger() {}
}
