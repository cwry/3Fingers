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
    public OAAnimation idleAnimation;
    public OAAnimation walkAnimation;
    public TerrainLayer currentTerrainLayer;
    public float speed = 1f;

    [SerializeField]
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
        walkAnimation.Play(gameObject);
        var gameAction = TweenUtil.TweenPathBySpeed(this, currentTerrainLayer.GetSubPath(transform.position.x, x), speed, EasingFunctions.Linear);
        gameAction
            .Then(() => {
                idleAnimation.Play(gameObject);
            });
        return gameAction;
    }

    void Awake() {
        if (currentTerrainLayer != null) transform.position = (Vector3)currentTerrainLayer.Sample(transform.position.x);
    }

    void Start() {
        LookDirection = LookDirection;
    }

    void Update() {
        if (
            !GameActionHandler.IsBlocked &&
            (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()) &&
            Input.GetMouseButtonDown(0)
        ) {
            GameActionHandler.Execute(() => MoveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition).x));
        }
    }
}
