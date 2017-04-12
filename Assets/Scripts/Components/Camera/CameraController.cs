using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public CameraFollowSettings cfs;
    public GameObject target;

    private bool init = true;

    void Update() {
        if (cfs == null) return;

        if (init) {
            init = false;
            setCameraValues();
        } else {
            if (cfs.dampen) {
                interpolateCameraValues();
            } else {
                setCameraValues();
            }
        }
    }

    void setCameraValues() {
        float targetSize = cfs.sanitizeCameraSize(Camera.main.orthographicSize);
        Camera.main.orthographicSize = targetSize;

        Vector2 targetPos = new Vector2(cfs.sanitizeCameraXPos(target.transform.position.x), cfs.sanitizeCameraYPos(target.transform.position.y));
        Camera.main.transform.position = new Vector3(targetPos.x, targetPos.y, Camera.main.transform.position.z);
    }

    void interpolateCameraValues() {
        float targetSize = cfs.sanitizeCameraSize(Camera.main.orthographicSize);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, Time.deltaTime * cfs.dampModifier);

        Vector2 targetPos = new Vector2(cfs.sanitizeCameraXPos(target.transform.position.x), cfs.sanitizeCameraYPos(target.transform.position.y));
        Vector2 interpolatedPos = new Vector2(
            Mathf.Lerp(Camera.main.transform.position.x, targetPos.x, Time.deltaTime * cfs.dampModifier),
            Mathf.Lerp(Camera.main.transform.position.y, targetPos.y, Time.deltaTime * cfs.dampModifier)
        );
        Camera.main.transform.position = new Vector3(interpolatedPos.x, interpolatedPos.y, Camera.main.transform.position.z);
    }
}
