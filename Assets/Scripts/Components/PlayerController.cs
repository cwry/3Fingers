using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {
    static PlayerController instance;
    public static PlayerController Instance {
        get {
            if (instance == null) instance = FindObjectOfType<PlayerController>();
            return instance;
        }
    }

    public SpriteRenderer spriteRenderer;
    public TerrainLayer currentTerrainLayer;
    public float speed = 1f;

    void Awake() {
        if (currentTerrainLayer != null) transform.position = (Vector3)currentTerrainLayer.Sample(transform.position.x);
    }

    void Update() {
        if (
            !GameActionHandler.Instance.IsBlocked &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()) &&
            Input.GetMouseButtonDown(0)
        ) {
            float inputX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            if(spriteRenderer != null) {
                spriteRenderer.flipX = Mathf.Sign(transform.position.x - inputX) < 0;
            }
            GameActionHandler.Instance.Execute(TweenUtil.TweenPathBySpeed(this, currentTerrainLayer.GetSubPath(transform.position.x, inputX), speed, EasingFunctions.InOutSin));
        }
    }
}
