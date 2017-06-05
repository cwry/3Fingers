using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenOrientationUIUpdateHack : UIBehaviour {
    //hack to reflow a horizontal layout group on device orientation change - every time and not just sometimes
    //this is absolutely terrible
    //thanks unity
    public HorizontalLayoutGroup grp;

    protected override void OnRectTransformDimensionsChange() {
        if(enabled)StartCoroutine(UpdateGroupNextFrame());
    }

    IEnumerator UpdateGroupNextFrame() {
        yield return 0;

        //apparently this causes the group to reflow
        // :(
        grp.spacing = grp.spacing + 1;
        grp.spacing = grp.spacing - 1;
    }
}
