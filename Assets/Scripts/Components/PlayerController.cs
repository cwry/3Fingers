using UnityEngine;
using UnityEngine.EventSystems;

public enum LookDirection {
    LEFT,
    RIGHT
}

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

    LookDirection _lookDirection;
    public LookDirection LookDirection {
        get {
            return _lookDirection;
        }
        set {
            if (spriteRenderer != null) {
                spriteRenderer.flipX = value == LookDirection.RIGHT;
            }
            _lookDirection = value;
        }
    }

    public GameAction MoveTo(float x) {
        LookDirection = Mathf.Sign(x - transform.position.x) > 0 ? LookDirection.RIGHT : LookDirection.LEFT;
        return TweenUtil.TweenPathBySpeed(this, currentTerrainLayer.GetSubPath(transform.position.x, x), speed, EasingFunctions.InOutSin);
    }

    void Awake() {
        if (currentTerrainLayer != null) transform.position = (Vector3)currentTerrainLayer.Sample(transform.position.x);
    }

    void Update() {
        if (
            !GameActionHandler.Instance.IsBlocked &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()) &&
            Input.GetMouseButtonDown(0)
        ) {
            GameActionHandler.Instance.SetCurrent(MoveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition).x));
        }
    }
}
