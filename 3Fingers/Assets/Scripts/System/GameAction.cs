﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RSG;

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

    public bool IsBlocked() {
        if (currentAction == null) return false;
        return true;
    }

    public bool Execute(GameAction gameAction) {
        if (currentAction != null) {
            if(!currentAction.Cancel()) return false;
        }
        currentAction = gameAction;
        currentAction
            .Catch(e => {
                currentAction = null;
            })
            .Done(() => {
                currentAction = null;
            });

        return true;
    }

    public bool Execute(IPromise promise) {
        return Execute(GameAction.Create(promise));
    }

    public bool Execute<T>(IPromise<T> promise) {
        return Execute(GameAction.Create(promise));
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

    public bool IsBlocking() {
        return cancel != null;
    }

    public bool Cancel() {
        if (!IsBlocking()) {
            cancel();
            Resolve();
            return true;
        }
        return false;
    }
}