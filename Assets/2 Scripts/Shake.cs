using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    Transform camTr;
    bool shakeRotate;

    Vector3 originPos;
    Quaternion originRot;

    void Awake() {
        camTr = Camera.main.transform;
    }

    public IEnumerator ShakeCamera(float duration = 0.1f, float magnitudePos = 0.3f, float magnitudeRot = 0.1f) {
        Debug.Log("SHAKE");
        originPos = camTr.localPosition;
        originRot = camTr.localRotation;
        float passTime = 0.0f;

        while(passTime < duration) {
            Debug.Log("SHAKE");
            Vector3 shakePos = Random.insideUnitSphere;
            camTr.localPosition = originPos + shakePos * magnitudePos;

            if (shakeRotate) {
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));

                camTr.localRotation = Quaternion.Euler(shakeRot);
            }
            passTime += Time.deltaTime;
            yield return null;
        }
        camTr.localPosition = originPos;
        camTr.localRotation = originRot;
    }
}
