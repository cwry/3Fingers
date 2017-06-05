using Spine.Unity;
using System;
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
    Transform objectHolder;

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

    public string SortingLayerName {
        get{
            return skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingLayerName;
        }
        set{
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach(var renderer in renderers) {
                renderer.sortingLayerName = value;
            }
        }
    }

    public GameAction MoveTo(float x) {
        LookDirection = Mathf.Sign(x - transform.position.x) > 0 ? LookDirection.RIGHT : LookDirection.LEFT;
        var walkAnimationAction = walkAnimation.Play(gameObject).gameAction;
        var moveAction = TweenUtil.TweenPathBySpeed(this, currentTerrainLayer.GetSubPath(transform.position.x, x), speed, EasingFunctions.Linear);
        moveAction
            .Then(() => {
                walkAnimationAction.Cancel();
            });
        return moveAction;
    }

    void Awake() {
        if (currentTerrainLayer != null) transform.position = (Vector3)currentTerrainLayer.Sample(transform.position.x);

        objectHolder.localScale = Vector3.one;
        Vector3 lossyScale = objectHolder.lossyScale;
        objectHolder.localScale = new Vector3(1 / lossyScale.x, 1 / lossyScale.y, 1 / lossyScale.z);

        SortingLayerName = "Default";
    }

    public Action SetHoldingObject(GameObject go) {
        var renderer = go.GetComponentInChildren<SpriteRenderer>();
        var oldActiveState = go.activeSelf;
        var oldParent = go.transform.parent;
        var oldSortingLayerName = renderer.sortingLayerName;
        var oldSortingOrder = renderer.sortingOrder;
        var oldPosition = go.transform.position;
        var oldRotation = go.transform.rotation;
        go.SetActive(true);
        go.transform.parent = objectHolder;
        go.transform.localPosition = Vector3.zero;
        renderer.sortingLayerName = SortingLayerName;
        renderer.sortingOrder = 6;
        return () => {
            go.SetActive(oldActiveState);
            go.transform.parent = oldParent;
            go.transform.position = oldPosition;
            go.transform.rotation = oldRotation;
            renderer.sortingLayerName = oldSortingLayerName;
            renderer.sortingOrder = oldSortingOrder;
        };
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
