using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCam : MonoBehaviour
{
    float progress;

    Vector3 firstPos;
    Vector3 secondPos;
    Vector3 thirdPos;
    public static bool isReady;

    int cnt;
    bool pause;
    void Awake() {
        progress = 0;
        cnt = 0;
        isReady = false;
        pause = false;
        firstPos = new Vector3(0, 1, -10);
        secondPos = new Vector3(21, 1, -13);
        thirdPos = new Vector3(51, 1, -20);
    }
    void Start() {
        StartCoroutine(Pause(0.5f));
    }
    void Update()
    {
        if (pause)
            return;
        if (cnt == 0) {
            transform.position = Vector3.Lerp(firstPos, secondPos, progress);
            progress += Time.deltaTime;
            if (progress >= 1) {
                StartCoroutine(Pause(0.5f));
                progress = 0;
                cnt = 1;
            }
        }
        else if(cnt == 1) {
            transform.position = Vector3.Lerp(secondPos, thirdPos, progress);
            progress += Time.deltaTime;
            if (progress >= 1) {
                StartCoroutine(Ready());
            }
        }
    }
    IEnumerator Pause(float time) {
        pause = true;
        yield return new WaitForSeconds(time);
        pause = false;
    }
    IEnumerator Ready() {
        yield return new WaitForSeconds(0.5f);
        isReady = true;
    }
}
