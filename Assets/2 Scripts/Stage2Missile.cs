using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Missile : MonoBehaviour
{
    [SerializeField]
    GameObject expEffect;
    [SerializeField]
    GameObject fireParticle;
    [SerializeField]
    GameObject MissilePrefab;
    public Transform target;
    public bool isChase;

    float chaseTime;

    Rigidbody rigid;
    float turningForce;
    float speed;

    bool isControl;
    bool playerUpMissile;
    Transform cam;

    float selfBoomCurTime;

    CapsuleCollider capsule;
    private void Awake() {
        target = GameObject.Find("Player").transform;
        rigid = GetComponent<Rigidbody>();
        capsule = transform.GetChild(0).GetComponent<CapsuleCollider>();

        cam = Camera.main.transform;
        turningForce = 5;
    }
    void Update() {
        // 플레이어를 쫓음.
        if (isChase) {
            chaseTime += Time.deltaTime;
            // 10초
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

        // 자폭 관리
        if (!isChase) {
            if (!isControl) {
                selfBoomCurTime += Time.deltaTime;
            }
            else {
                selfBoomCurTime += Time.deltaTime / 5;
            }
        }
        if (selfBoomCurTime > 10) {
            float dis = Vector3.Distance(target.position, transform.position);
            if (dis < 0.7f)
                dis = 0.7f;
            if (dis < 5) {
                target.GetComponent<PlayerCtrl>().HpDown(20/dis);
            }
            target.localEulerAngles = new Vector3(0, target.localEulerAngles.y, 0);
            playerUpMissile = false;
            isControl = false;
            PlayerCtrl.dontCtrl = false;
            CamCtrl.dontLimit = false;
            Instantiate(expEffect, transform.position, Quaternion.identity);
            ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");

            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            target.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
        }

        // 컨트롤 관리
        if (playerUpMissile) {
            target.position = transform.position + Vector3.up * 0.7f;
        }
        if (isControl) {
            if (speed < 30) {
                speed += 70 * Time.deltaTime;
            }

            if (playerUpMissile) {
                target.localRotation = cam.transform.localRotation;
                Quaternion qua = cam.transform.localRotation;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, qua, Time.deltaTime * 5);
            }

            rigid.velocity = transform.forward * speed;

            if (Input.GetKeyDown(KeyCode.Space) && playerUpMissile) {
                playerUpMissile = false;
                target.localEulerAngles = new Vector3(0, target.localEulerAngles.y, 0);
                target.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
                target.GetComponent<Rigidbody>().AddForce(Vector3.up * 15, ForceMode.Impulse);
                PlayerCtrl.dontCtrl = false;
                CamCtrl.dontLimit = false;
            }
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
        rigid.velocity = Vector3.zero;
        chaseTime = 0;
        isControl = false;
    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && isChase) {
            // 플레이어 공격
            Shake.instance.ShakeCoroutine(0.8f, 1f);
            Instantiate(expEffect, target.position, Quaternion.identity);
            target.GetComponent<PlayerCtrl>().HpDown(30);
            ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");
        }
        if (isControl) {
            if (other.CompareTag("Ground")) {
                Shake.instance.ShakeCoroutine(0.4f, 0.8f);
                Instantiate(expEffect, transform.position, Quaternion.identity);
                Hit();
                if (playerUpMissile) {
                    target.GetComponent<PlayerCtrl>().HpDown(15);
                }
            }
            if (other.CompareTag("Enemy")) {
                Shake.instance.ShakeCoroutine(0.4f, 0.8f);
                Instantiate(expEffect, transform.position, Quaternion.identity);
                if (other.GetComponentInParent<secondEnemy>()) {
                    other.GetComponentInParent<secondEnemy>().HpDown(30);
                }
                Hit();
                if (playerUpMissile) {
                    target.GetComponent<PlayerCtrl>().HpDown(15);
                }

            }
        }
    }
    void OnCollisionEnter(Collision collision) {
        if(collision.collider.CompareTag("Ground") && isChase) {
            // 추격 종료
            isChase = false;
            fireParticle.SetActive(false);
            rigid.constraints = RigidbodyConstraints.FreezePositionY;
        }
        if (collision.collider.CompareTag("Player")) {
            if(collision.transform.position.y > transform.position.y) {
                // 로켓을 탐.
                PlayerCtrl.dontCtrl = true;
                fireParticle.SetActive(true);
                playerUpMissile = true;
                rigid.constraints = RigidbodyConstraints.None;
                StartCoroutine(Control());
            }
        }
    }
    IEnumerator Control() {
        target.GetComponent<Rigidbody>().constraints = ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);
        Vector3 vec = transform.localEulerAngles;
        vec.x = -60f;
        transform.localEulerAngles = vec;
        speed = 0;
        rigid.velocity = Vector3.zero;
        rigid.useGravity = false;
        rigid.AddForce(transform.forward * 10f, ForceMode.VelocityChange);

        capsule.isTrigger = true;
        yield return new WaitForSeconds(0.7f);
        CamCtrl.dontLimit = true;
        rigid.velocity = Vector3.zero;
        isControl = true;
    }
    void Hit() {
        ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");
        CamCtrl.dontLimit = false;
        PlayerCtrl.dontCtrl = false;
        target.localEulerAngles = new Vector3(0, target.localEulerAngles.y, 0);
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
    }
}
