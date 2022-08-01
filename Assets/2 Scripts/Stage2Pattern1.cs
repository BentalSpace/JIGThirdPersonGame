using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Pattern1 : MonoBehaviour
{
    enum Type { Button, Pillar}
    [SerializeField]
    Type type;

    [SerializeField]
    GameObject upObject;

    Vector3 upObjOriginPos;
    bool isActive;

    void OnCollisionEnter(Collision collision) {
        if (type == Type.Pillar)
            return;
        if (collision.collider.CompareTag("Player") && !isActive) {
            StartCoroutine(ObjectUp());
        }
    }
    IEnumerator ObjectUp() {
        isActive = true;
        float progress = 0;
        upObjOriginPos = new Vector3(upObject.transform.position.x, 64, upObject.transform.position.z);
        while(progress < 1) {
            progress+=0.1f;
            upObject.transform.position = Vector3.Lerp(upObjOriginPos, upObjOriginPos + Vector3.up * 4f, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void Hit() {
        if (type == Type.Pillar) {
            upObject.SetActive(false);
            gameObject.SetActive(false);
            isActive = false;
        }
    }
}
