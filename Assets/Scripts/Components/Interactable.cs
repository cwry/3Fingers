using System;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour {
    public Transform buttonPosition;
    public Transform triggerPosition;
    public Vector2 buttonRange;
    Button currentButton;

    public Func<bool> conditional;
    public event Action Triggered;

    void Awake() {
        if (buttonPosition == null) buttonPosition = transform;
        if (triggerPosition == null) triggerPosition = transform;
    }

    void Update() {
        bool condition = conditional == null ? true : conditional(); 
        if (
            condition &&
            !GameActionHandler.Instance.IsBlocked &&
            PlayerController.Instance != null &&
            PlayerController.Instance.transform.position.x < triggerPosition.position.x + buttonRange.x / 2 &&
            PlayerController.Instance.transform.position.x > triggerPosition.position.x - buttonRange.x / 2 &&
            PlayerController.Instance.transform.position.y < triggerPosition.position.y + buttonRange.y / 2 &&
            PlayerController.Instance.transform.position.y > triggerPosition.position.y - buttonRange.y / 2
        ) {
            if(currentButton == null) {
                GameActionHandler.Instance.ExecutionStateChanged += OnExecutionStateChanged;
                currentButton = InteractCanvas.Instance.SpawnButton(new Vector2(buttonPosition.position.x, buttonPosition.position.y));
                currentButton.onClick.AddListener(() => {
                    GameActionHandler.Instance.ExecutionStateChanged -= OnExecutionStateChanged;
                    Destroy(currentButton.gameObject);
                    if (Triggered != null) Triggered();
                });
            }
        }else if(currentButton != null) {
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
        Gizmos.DrawCube(triggerPosition == null ? transform.position : triggerPosition.position, buttonRange);
    }
}
