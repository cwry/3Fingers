using System.Collections.Generic;
using RSG;
using System;

public class AnimationEventDispatcher {
    public GameAction gameAction;

    Dictionary<string, List<Action>> listeners = new Dictionary<string, List<Action>>();

    public AnimationEventDispatcher(GameAction gameAction) {
        this.gameAction = gameAction;
        gameAction
            .Catch((e) => {
                FireAnimationEvent("end");
                listeners.Clear();
            })
            .Done(() => {
                FireAnimationEvent("end");
                listeners.Clear();
            });
    }

    public void FireAnimationEvent(string eventName) {
        if (listeners.ContainsKey(eventName)) {
            foreach(var listener in listeners[eventName].ToArray()) {  //toarray to copy the list (so listeners can unsubscribe during loop)
                listener();
            }
        }
    }

    public Action On(string eventName, Action cb) {
        if (!listeners.ContainsKey(eventName)) {
            listeners[eventName] = new List<Action>();
        }

        listeners[eventName].Add(cb);

        return () => Off(eventName, cb);
    }

    public void Off(string eventName, Action cb) {
        if (listeners.ContainsKey(eventName)) {
            listeners[eventName].Remove(cb);
        }
    }

    public IPromise Promise(string eventName) {
        if (eventName == null || eventName == "end") return gameAction;
        var promise = new Promise();

        Action off = null;
        Action offEnd = null;

        off = On(eventName, () => {
            promise.Resolve();
            off();
            offEnd();
        });

        offEnd = On("end", () => {
            promise.Reject(new Exception("Animation Event " + eventName + " was promised but never fired"));
            off();
            offEnd();
        });

        return promise;
    }
}
