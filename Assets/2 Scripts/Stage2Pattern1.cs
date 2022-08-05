using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Pattern1 : MonoBehaviour
{
    enum Type { Button, Pillar}
    [SerializeField]
    Type type;

    [SerializeField]
    GameObject friendObject;

    Vector3 upObjOriginPos;
    public bool isActive;
    void Awake() {
        gameObject.SetActive(false);
    }
    void OnCollisionEnter(Collision collision) {
        if (type == Type.Pillar)
            return;
        if (collision.collider.CompareTag("Player") && !isActive) {
            StartCoroutine(ObjectUp());
        }
    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Veneer")) {
            ObjectManager.instance.ReturnObject(other.gameObject, "downAtk");
        }
    }
    IEnumerator ObjectUp() {
        isActive = true;
        float progress = 0;
        upObjOriginPos = new Vector3(friendObject.transform.position.x, 64, friendObject.transform.position.z);
        friendObject.GetComponent<AudioSource>().Play();
        while(progress < 1) {
            progress+=0.1f;
            friendObject.transform.position = Vector3.Lerp(upObjOriginPos, upObjOriginPos + Vector3.up * 4f, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
        friendObject.GetComponent<AudioSource>().Stop();
        // �ٽ� ������
        yield return new WaitForSeconds(8f);
        friendObject.GetComponent<AudioSource>().Play();
        progress = 0;
        while (progress < 1) {
            progress += 0.1f;
            friendObject.transform.position = Vector3.Lerp(upObjOriginPos + Vector3.up * 4f, upObjOriginPos, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
        friendObject.GetComponent<AudioSource>().Stop();
        isActive = false;
    }

    //���� Ŭ����
    public void Hit() {
        if (type == Type.Pillar) {
            friendObject.SetActive(false);
            gameObject.SetActive(false);
            friendObject.GetComponent<Stage2Pattern1>().isActive = false;
        }
    }
}
