using System;
using UnityEngine;
using UnityEngine.UI;
using RSG;

public abstract class Interaction : MonoBehaviour {
    public Transform buttonPosition;
    public Transform triggerPosition;
    Button currentButton;

    void Awake() {
        if (buttonPosition == null) buttonPosition = transform;
        if (triggerPosition == null) triggerPosition = transform;
    }

    bool shouldShowButton() {
        if (
            !Condition() ||
            GameActionHandler.Instance.IsBlocked ||
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
        if (shouldShowButton()) {
            if (currentButton == null) {
                GameActionHandler.Instance.ExecutionStateChanged += OnExecutionStateChanged;
                currentButton = InteractCanvas.Instance.SpawnButton(new Vector2(buttonPosition.position.x, buttonPosition.position.y));
                currentButton.onClick.AddListener(() => {
                    GameActionHandler.Instance.ExecutionStateChanged -= OnExecutionStateChanged;
                    Destroy(currentButton.gameObject);
                    OnTrigger();
                });
            }
        } else if (currentButton != null) {
            GameActionHandler.Instance.ExecutionStateChanged -= OnExecutionStateChanged;
            Destroy(currentButton.gameObject);
        }
    }

    void OnExecutionStateChanged() {
        if (currentButton != null && GameActionHandler.Instance.IsBlocked) {
            GameActionHandler.Instance.ExecutionStateChanged -= OnExecutionStateChanged;
            Destroy(currentButton.gameObject);
        }
    }

    void OnDestroy() {
        GameActionHandler.Instance.ExecutionStateChanged -= OnExecutionStateChanged;
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

    public virtual void OnTrigger() {}
}
