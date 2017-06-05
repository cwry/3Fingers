using System;
using RSG;
using UnityEngine.SceneManagement;

public class GameActionHandler {
    GameActionHandler() { }
    static GameActionHandler _instance;
    static GameActionHandler Instance {
        get {
            if (_instance == null) {
                _instance = new GameActionHandler();
                SceneManager.activeSceneChanged += cleanupOnSceneChange;
            }
            return _instance;
        }
    }

    static void cleanupOnSceneChange(Scene oldScene, Scene newScene) {
        _instance = null;
        SceneManager.activeSceneChanged -= cleanupOnSceneChange;
    }

    GameAction currentAction;

    event Action _executionStateChanged;

    public static event Action ExecutionStateChanged {
        add {
            Instance._executionStateChanged += value;
        }
        remove {
            Instance._executionStateChanged -= value;
        }
    }

    public static bool IsBlocked{
        get {
            if (Instance.currentAction == null) return false;
            return Instance.currentAction.IsBlocking;
        }
    }

    public static bool Execute(Func<GameAction> getGameAction) {
        if (Instance.currentAction != null) {
            if (!Instance.currentAction.Cancel()) return false;
        }

        Instance.currentAction = getGameAction();
        Instance.currentAction
            .Catch(e => {
                Instance.currentAction = null;
                if (Instance._executionStateChanged != null) Instance._executionStateChanged();
            })
            .Done(() => {
                Instance.currentAction = null;
                if (Instance._executionStateChanged != null) Instance._executionStateChanged();
            });

        if (Instance._executionStateChanged != null) Instance._executionStateChanged();
        return true;
    }

    public static bool Execute(Func<IPromise> getPromise) {
        return Execute(() => GameAction.Create(getPromise()));
    }

    public static bool Execute<T>(Func<IPromise<T>> getPromise) {
        return Execute(() => GameAction.Create(getPromise()));
    }

    public static bool SetCurrent(GameAction gameAction) {
        return Execute(() => gameAction);
    }

    public static bool SetCurrent(IPromise promise) {
        return Execute(() => promise);
    }

    public static bool SetCurrent<T>(IPromise<T> promise) {
        return Execute(() => promise);
    }
}

public class GameAction : Promise{
    Action cancel;

    GameAction(Action cancel = null) {
        this.cancel = cancel;
    }

    GameAction(IPromise promise, Action cancel = null) {
        promise
            .Catch(e => Reject(e))
            .Done(() => Resolve());
        this.cancel = cancel;
    }

    public static GameAction Create(Action cancel = null) {
        return new GameAction(cancel);
    }

    public static GameAction Create(IPromise promise, Action cancel = null) {
        return new GameAction(promise, cancel);
    }

    public static GameAction Create<T>(IPromise<T> promise, Action cancel = null) {
        return Create(
            promise
                .Then(v => Resolved())
                .Catch(e => Rejected(e)),

            cancel
        );
    }

    public bool IsBlocking {
        get { return cancel == null; }
    }

    public bool Cancel() {
        if (!IsBlocking) {
            cancel();
            if(CurState == PromiseState.Pending) Resolve();
            return true;
        }
        return false;
    }
}
