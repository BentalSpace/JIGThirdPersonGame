using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffect;

    public float dmg;

    GameObject hitTarget;

    [SerializeField]
    AudioClip atkClip;
    [SerializeField]
    AudioSource audio;
    void Awake() {
    }
    void OnTriggerEnter(Collider other) {

        if (other.GetComponentInParent<firstEnemy>()) {
            if (other.CompareTag("Enemy") && other.gameObject != hitTarget && other.GetComponentInParent<firstEnemy>().isGetHitTime) {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
                audio.clip = atkClip;
                audio.Play();
                StartCoroutine(Camera.main.GetComponent<Shake>().ShakeCamera());
                GetComponentInParent<Animator>().speed = 0;
                Invoke("SpeedBack", 0.1f);
                other.GetComponentInParent<firstEnemy>().HpDown(dmg);
                hitTarget = other.gameObject;
                Debug.Log(dmg);
            }
        }
        if (other.GetComponentInParent<secondEnemy>()) {
            if(other.CompareTag("Enemy") && other.GetComponent<SphereCollider>()) {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
                audio.clip = atkClip;
                audio.Play();
                StartCoroutine(Camera.main.GetComponent<Shake>().ShakeCamera());
                GetComponentInParent<Animator>().speed = 0;
                Invoke("SpeedBack", 0.1f);
                other.GetComponentInParent<secondEnemy>().HpDown(dmg);
                hitTarget = other.gameObject;
            }
        }
        else if (other.CompareTag("Enemy")) {
            Instantiate(hitEffect, transform.position, transform.rotation, null);
            StartCoroutine(Camera.main.GetComponent<Shake>().ShakeCamera());
        }
    }
    void SpeedBack() {
        GetComponentInParent<Animator>().speed = 1;
    }
    void OnDisable() {
        hitTarget = null;
    }

}
