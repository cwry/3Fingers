using Spine.Unity;
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

    public SkeletonAnimation skeletonAnimation;
    public SpineAnimationDescription idleAnimation;
    public SpineAnimationDescription walkAnimation;
    public TerrainLayer currentTerrainLayer;
    public float speed = 1f;

    LookDirection _lookDirection;
    public LookDirection LookDirection {
        get {
            return _lookDirection;
        }
        set {
            if (skeletonAnimation != null) {
                skeletonAnimation.skeleton.flipX = value == LookDirection.LEFT;
            }
            _lookDirection = value;
        }
    }

    public GameAction MoveTo(float x) {
        LookDirection = Mathf.Sign(x - transform.position.x) > 0 ? LookDirection.RIGHT : LookDirection.LEFT;
        AnimationUtil.PlaySpine(gameObject, walkAnimation);
        var gameAction = TweenUtil.TweenPathBySpeed(this, currentTerrainLayer.GetSubPath(transform.position.x, x), speed, EasingFunctions.Linear);
        gameAction
            .Then(() => {
                AnimationUtil.PlaySpine(gameObject, idleAnimation);
            });
        return gameAction;
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
