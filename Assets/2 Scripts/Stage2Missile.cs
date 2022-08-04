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

    secondEnemy enemy;

    float chaseTime;

    Rigidbody rigid;
    float turningForce;
    float speed;

    bool isControl;
    bool playerUpMissile;
    Transform cam;

    public float selfBoomCurTime;
    bool isHit;

    CapsuleCollider capsule;
    void OnEnable() {
        selfBoomCurTime = 0;
        isHit = false;
    }
    private void Awake() {
        enemy = GameObject.Find("UFO").GetComponent<secondEnemy>();
        target = GameObject.Find("Player").transform;
        rigid = GetComponent<Rigidbody>();
        capsule = transform.GetChild(0).GetComponent<CapsuleCollider>();

        cam = Camera.main.transform;
        turningForce = 5;
    }
    void Update() {
        if (Time.timeScale == 0) {
            return;
        }
        // �÷��̾ ����.
        if (isChase) {
            chaseTime += Time.deltaTime;
            // 10��
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

        // ���� ����
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
            if (playerUpMissile) {
                target.localEulerAngles = new Vector3(0, target.localEulerAngles.y, 0);
                playerUpMissile = false;
                isControl = false;
                PlayerCtrl.dontCtrl = false;
                CamCtrl.dontLimit = false;
                target.GetComponent<Rigidbody>().velocity = Vector3.zero;
                target.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
            }
            Instantiate(expEffect, transform.position, Quaternion.identity);
            ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");
            enemy.curMissileCnt--;

        }

        // ��Ʈ�� ����
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
    public void Boom() {
        Instantiate(expEffect, transform.position, Quaternion.identity);
        ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");
        enemy.curMissileCnt--;
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
        if (other.CompareTag("Player") && (isChase || !playerUpMissile)) {
            // �÷��̾� ����
            Shake.instance.ShakeCoroutine(0.8f, 1f);
            Instantiate(expEffect, target.position, Quaternion.identity);
            target.GetComponent<PlayerCtrl>().HpDown(30);
            ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");
            enemy.curMissileCnt--;
        }
        if (!isControl) {
            if (other.CompareTag("Wall")) {
                Shake.instance.ShakeCoroutine(0.4f, 0.8f);
                Instantiate(expEffect, transform.position, Quaternion.identity);
                if (playerUpMissile) {
                    Hit();
                }
                else if (!isHit) {
                    StartCoroutine(ReturnObj());
                    enemy.curMissileCnt--;
                }
            }
        }
        if (isControl) {
            if (isHit)
                return;
            if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
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
            if (other.CompareTag("Missile")) {
                Shake.instance.ShakeCoroutine(0.4f, 0.8f);
                Instantiate(expEffect, transform.position, Quaternion.identity);
                Hit();
                if (playerUpMissile) {
                    target.GetComponent<PlayerCtrl>().HpDown(15);
                }
                other.GetComponent<Stage2Missile>().Boom();
            }
        }
    }
    void OnCollisionEnter(Collision collision) {
        if(collision.collider.CompareTag("Ground") && isChase) {
            // �߰� ����
            isChase = false;
            fireParticle.SetActive(false);
            rigid.constraints = RigidbodyConstraints.FreezePositionY;
        }
        if (collision.collider.CompareTag("Player")) {
            if(collision.transform.position.y > transform.position.y) {
                // ������ Ž.
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
        rigid.AddForce(transform.forward * 30f, ForceMode.VelocityChange);

        capsule.isTrigger = true;
        yield return new WaitForSeconds(0.7f);
        CamCtrl.dontLimit = true;
        rigid.velocity = Vector3.zero;
        isControl = true;
    }
    void Hit() {
        if (!isHit) {
            StartCoroutine(ReturnObj());
            enemy.curMissileCnt--;
        }
        CamCtrl.dontLimit = false;
        PlayerCtrl.dontCtrl = false;
        target.localEulerAngles = new Vector3(0, target.localEulerAngles.y, 0);
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePosition;
    }
    IEnumerator ReturnObj() {
        isHit = true;
        yield return new WaitForSeconds(0.1f);
        ObjectManager.instance.ReturnObject(gameObject, "st2Pattern3");
    }
}
