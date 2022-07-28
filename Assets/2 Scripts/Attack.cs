using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffect;

    public float dmg;

    GameObject hitTarget;
    void Awake() {
    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy") && other.gameObject != hitTarget && other.GetComponentInParent<firstEnemy>().isGetHitTime) {
            Instantiate(hitEffect, transform.position, transform.rotation, null);
            //GetComponentInParent<Animator>().speed = 0;
            //Invoke("SpeedBack", 0.1f);
            other.GetComponentInParent<firstEnemy>().HpDown(dmg);
            hitTarget = other.gameObject;
        }
    }
    //void SpeedBack() {
    //    GetComponentInParent<Animator>().speed = 1;
    //}
    void OnDisable() {
        hitTarget = null;
    }

}
