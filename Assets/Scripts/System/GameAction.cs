using System;
using RSG;
using UnityEngine;

public class GameActionHandler {
    GameActionHandler() { }
    static GameActionHandler instance;
    public static GameActionHandler Instance {
        get {
            if (instance == null) instance = new GameActionHandler();
            return instance;
        }
    }

    GameAction currentAction;

    public event Action ExecutionStateChanged;

    public bool IsBlocked{
        get {
            if (currentAction == null) return false;
            return currentAction.IsBlocking;
        }
    }

    public bool SetCurrent(GameAction gameAction) {
        if (currentAction != null) {
            if(!currentAction.Cancel()) return false;
        }
        currentAction = gameAction;
        currentAction
            .Catch(e => {
                currentAction = null;
                if (ExecutionStateChanged != null) ExecutionStateChanged();
            })
            .Done(() => {
                currentAction = null;
                if (ExecutionStateChanged != null) ExecutionStateChanged();
            });

        if (ExecutionStateChanged != null) ExecutionStateChanged();
        return true;
    }

    public bool SetCurrent(IPromise promise) {
        return SetCurrent(GameAction.Create(promise));
    }

    public bool SetCurrent<T>(IPromise<T> promise) {
        return SetCurrent(GameAction.Create(promise));
    }
}

public class GameAction : Promise{
    Action cancel;

    GameAction(IPromise promise, Action cancel = null) {
        promise
            .Catch(e => Reject(e))
            .Done(() => Resolve());
        this.cancel = cancel;
    }

    public static GameAction Create(IPromise promise, Action cancel = null) {
        return new GameAction(promise, cancel);
    }

    public static GameAction Create<T>(IPromise<T> promise, Action cancel = null) {
        return Create(
            promise
                .Catch(e => Rejected(e))
                .Then(v => Resolved()),

            cancel
        );
    }

    public bool IsBlocking {
        get { return cancel == null; }
    }

    public bool Cancel() {
        if (!IsBlocking) {
            cancel();
            Resolve();
            return true;
        }
        return false;
    }
}
