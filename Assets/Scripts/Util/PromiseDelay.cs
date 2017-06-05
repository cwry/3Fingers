using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;

public static class PromiseDelay{

	public static IPromise Get(float delaySec) {
        var promise = new Promise();
        Action onDestroy = () => promise.Reject(new ApplicationException("Scene changed before Delay could finish"));
        GlobalCoroutine.Instance.StartCoroutine(WaitForSeconds(delaySec, () => {
            ObjectDestroyEvent.Get(GlobalCoroutine.Instance.gameObject).Destroy -= onDestroy;
            promise.Resolve();
        }));
        ObjectDestroyEvent.Get(GlobalCoroutine.Instance.gameObject).Destroy += onDestroy;
        return promise;
    }

    static IEnumerator WaitForSeconds(float secs, Action onDone) {
        yield return new WaitForSeconds(secs);
        onDone();
    }
}
