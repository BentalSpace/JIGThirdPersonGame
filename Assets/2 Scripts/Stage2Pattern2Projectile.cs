using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Pattern2Projectile : MonoBehaviour
{
    float dmg;

    float colliderDisableTime;

    bool scaleCoroutine;

    Rigidbody rigid;
    GameObject particle;
    Material mat;
    SphereCollider col;

    [SerializeField]
    AudioClip shotClip;
    [SerializeField]
    AudioClip groundClip;

    AudioSource audio;

    private void OnEnable() {
        mat.color = new Color32(224, 54, 54,255);
        particle.SetActive(true);
        rigid.useGravity = true;
        scaleCoroutine = false;
        dmg = 20;
    }
    void Awake() {
        rigid = GetComponent<Rigidbody>();
        particle = transform.GetChild(1).gameObject;
        mat = GetComponent<MeshRenderer>().material;
        col = GetComponent<SphereCollider>();
        audio = GetComponent<AudioSource>();
    }
    void Update() {
        colliderDisableTime += Time.deltaTime;
        if(dmg == 10) {
            if (!col.enabled) {
                col.enabled = true;
            }
            if(colliderDisableTime > 1f) {
                colliderDisableTime = 0;
                col.enabled = false;
            }
        }
    }
    void OnTriggerEnter(Collider other) {
        if (!scaleCoroutine) {
            if (other.CompareTag("Ground")) {
                audio.clip = groundClip;
                audio.pitch = 3;
                audio.Play();
                particle.SetActive(false);
                StartCoroutine(ScaleUp());
            }
        }
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerCtrl>().HpDown(dmg);
        }
    }
    IEnumerator ScaleUp() {
        scaleCoroutine = true;
        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
        dmg = 10;
        float progress = 0;
        while(progress < 1) {
            progress += 0.05f;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 10, progress / 1);
            if(progress < 0.5f) {
                mat.color = Color.Lerp(new Color32(224, 54, 54, 255), new Color32(224, 54, 54, 100), progress / 1);
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(5f);
        progress = 0;
        while(progress < 1) {
            progress += 0.1f;
            mat.color = Color.Lerp(new Color32(224, 54, 54, 100), new Color32(224, 54, 54, 0), progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
        mat.color = new Color32(224, 54, 54, 255);
        transform.localScale = Vector3.one;
        particle.SetActive(true);
        ObjectManager.instance.ReturnObject(gameObject, "st2Pattern2");
        audio.clip = shotClip;
        audio.pitch = 1;
    }
}
