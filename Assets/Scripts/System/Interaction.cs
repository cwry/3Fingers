using System;
using UnityEngine;
using UnityEngine.UI;
using RSG;

public abstract class Interaction : MonoBehaviour {
    public GameObject[] blinkers;
    public Transform buttonPosition;
    public Transform triggerPosition;
    Button currentButton;
    Action cancelCurrentBlinker;

    void Awake() {
        if (buttonPosition == null) buttonPosition = transform;
        if (triggerPosition == null) triggerPosition = transform;
    }

    bool shouldBeTriggerable() {
        if (
            !Condition() ||
            GameActionHandler.IsBlocked ||
            PlayerController.Instance == null
        ) {
            return false;
        }

        var playerPosition = PlayerController.Instance.transform.position;

        return
            playerPosition.x < triggerPosition.position.x + triggerPosition.localScale.x / 2 &&
            playerPosition.x > triggerPosition.position.x - triggerPosition.localScale.x / 2 &&
            playerPosition.y < triggerPosition.position.y + triggerPosition.localScale.y / 2 &&
            playerPosition.y > triggerPosition.position.y - triggerPosition.localScale.y / 2;
    }

    void Update() {
        if (shouldBeTriggerable()) {
            if (currentButton == null) {
                GameActionHandler.ExecutionStateChanged += OnExecutionStateChanged;
                currentButton = InteractCanvas.Instance.SpawnButton(new Vector2(buttonPosition.position.x, buttonPosition.position.y));
                cancelCurrentBlinker = SpriteBlink.Enable(blinkers, Color.white, Color.gray, 1);
                currentButton.onClick.AddListener(() => {
                    GameActionHandler.ExecutionStateChanged -= OnExecutionStateChanged;
                    Destroy(currentButton.gameObject);
                    cancelCurrentBlinker();
                    OnTrigger();
                });
            }
        } else if (currentButton != null) {
            GameActionHandler.ExecutionStateChanged -= OnExecutionStateChanged;
            Destroy(currentButton.gameObject);
            cancelCurrentBlinker();
        }
    }

    void OnExecutionStateChanged() {
        if (currentButton != null && GameActionHandler.IsBlocked) {
            GameActionHandler.ExecutionStateChanged -= OnExecutionStateChanged;
            Destroy(currentButton.gameObject);
            currentButton = null;
            cancelCurrentBlinker();
        }
    }

    void OnDestroy() {
        GameActionHandler.ExecutionStateChanged -= OnExecutionStateChanged;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0, 0, 1f, 0.5f);
        Gizmos.DrawCube(triggerPosition == null ? transform.position : triggerPosition.position, triggerPosition == null ? transform.localScale : triggerPosition.localScale);
        Gizmos.color = Color.black;
        Gizmos.DrawCube(buttonPosition == null ? transform.position : buttonPosition.position, new Vector3(1, 1, 0));
        Gizmos.DrawLine(triggerPosition == null ? transform.position : triggerPosition.position, buttonPosition == null ? transform.position : buttonPosition.position);
    }

    public virtual bool Condition() {
        return true;
    }

    public abstract void OnTrigger();
}

public abstract class Interaction<T> : Interaction where T : IPromise{

    public sealed override void OnTrigger() {
        GameActionHandler.Execute(() => Execute());
    }

    public abstract T Execute();
}
