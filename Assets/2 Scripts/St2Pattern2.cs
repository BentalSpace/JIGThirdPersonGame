using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class St2Pattern2 : MonoBehaviour
{
    bool isEnter;
    int enterCnt;
    float enterTime;

    void Update() {
        if (isEnter) {
            enterTime += Time.deltaTime;
        }

        if(enterCnt > 3 || enterTime > 3) {

        }
    }

    void OnCollisionEnter(Collision collision) {
        enterCnt++;
        isEnter = true;
    }
    void OnCollisionExit(Collision collision) {
        isEnter = false;
    }
}
