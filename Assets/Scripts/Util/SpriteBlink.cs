using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteBlink{
    public static Action Enable(GameObject go, Color startColor, Color endColor, float timePerTransition) {
        if (go == null) return () => { };
        var renderers = go.transform.GetComponentsInChildren<SpriteRenderer>();
        if (renderers == null || renderers.Length == 0) return () => { };
        Color[] startColors = renderers.Select((r) => r.color).ToArray();
        var globalCoroutine = GlobalCoroutine.OnGameObject(go);
        var coroutine = globalCoroutine.StartCoroutine(Blinker(renderers, startColor, endColor, timePerTransition));
        return () => {
            if (globalCoroutine == null) return;
            globalCoroutine.StopCoroutine(coroutine);
            for(int i = 0; i < renderers.Length; i++) {
                if (renderers[i] == null) continue;
                renderers[i].color = startColors[i];
            }
        };
    }

    public static Action Enable(GameObject[] gos, Color startColor, Color endColor, float timePerTransition) {
        Action[] disablers = new Action[gos.Length];
        for (int i = 0; i < gos.Length; i++) {
            disablers[i] = Enable(gos[i], startColor, endColor, timePerTransition);
        }

        return () => {
            foreach (var disabler in disablers) {
                disabler();
            }
        };
    }

    static IEnumerator Blinker(SpriteRenderer[] renderers, Color startColor, Color endColor, float timePerTransition) {
        float progress = 0;
        while (true) {
            float cycle = progress / timePerTransition;
            float cycleProgress = cycle - (int)cycle; 
            Color state = Color.Lerp(startColor, endColor, ((int)cycle % 2 == 0 ? cycleProgress : 1 - cycleProgress));
            foreach(var renderer in renderers) {
                if (renderer == null) continue;
                renderer.color = state;
            }
            yield return 0;
            progress += Time.deltaTime;
        }
    }

}
