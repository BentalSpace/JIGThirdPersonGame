using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Missile : MonoBehaviour
{
    [SerializeField]
    GameObject expEffect;
    [SerializeField]
    GameObject fireParticle;
    public Transform target;
    bool isChase;

    float chaseTime;

    Rigidbody rigid;
    float turningForce;
    float speed;
    float dis;

    bool isControl;
    Transform cam;

    CapsuleCollider capsule;
    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        capsule = transform.GetChild(0).GetComponent<CapsuleCollider>();

        cam = Camera.main.transform;
        turningForce = 5;
    }
    void Update() {
        if (isChase) {
            chaseTime += Time.deltaTime;
            // 10√ 
            if (chaseTime > 8) {
                if (speed > 0) {
                    speed -= 10 * Time.deltaTime;
                    capsule.isTrigger = false;
                    rigid.useGravity = true;
                }
            }
            else {
                if (speed < 20) {
                    speed += 70 * Time.deltaTime;
                }
            }
            if(chaseTime > 10) {
                isChase = false;
                fireParticle.SetActive(false);
            }
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
            LookAtTarget();
        }

        if (isControl) {
            target.position = transform.position;
            if (speed < 30) {
                speed += 70 * Time.deltaTime;
            }
            Vector3 dir = cam.localRotation * Vector3.forward;
            transform.localRotation = cam.transform.localRotation;

            rigid.velocity = transform.forward * speed;
            if (rigid.velocity.z < 20) {
                
                //rigid.AddForce(transform.forward * 5f, ForceMode.VelocityChange);
            }
            //transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));

            //Quaternion lookRotation = Quaternion.LookRotation(transform.position - target.position);
            //transform.LookAt(target.position - cam.position);
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 8 * Time.deltaTime);
        }
    }
    void LookAtTarget() {
        Quaternion lookRotation = Quaternion.LookRotation((target.position + Vector3.up * 2f) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningForce * Time.deltaTime);
    }
    public void ChaseCon() {
        fireParticle.SetActive(true);
        Invoke("ChaseTrue", 0.7f);
    }
    void ChaseTrue() {
        isChase = true;
        dis = Vector3.Distance(transform.position, target.position);
        rigid.velocity = Vector3.zero;
        chaseTime = 0;
        isControl = false;
    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && isChase) {
            StartCoroutine(Camera.main.GetComponent<Shake>().ShakeCamera(0.4f, 0.8f));
            Instantiate(expEffect, target.position, Quaternion.identity);
            //gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision collision) {
        if(collision.collider.CompareTag("Ground") && isChase) {
            isChase = false;
        }
        if (collision.collider.CompareTag("Player")) {
            if(collision.transform.position.y > transform.position.y) {
                // ∑Œƒœ¿ª ≈Ω.
                PlayerCtrl.dontCtrl = true;
                CamCtrl.dontLimit = true;
                isControl = true;
            }
        }
    }
}
