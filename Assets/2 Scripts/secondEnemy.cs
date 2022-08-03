using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondEnemy : MonoBehaviour
{
    /*
     * ü�� : 200
     * ������1 : ü���� 120�� �ɶ�����, ����1 ���-> �÷��̾ ���� ->  ���� 2 ��� -> ���� 1 ��� -> �÷��̾ ����.
     * ������2 : ü���� 60(�̻��� 2�� ���߱�)�� �ɶ�����, ����3 ���� (�̻����� �����ð� �������� �ʿ� ��ġ �Ǵ� ������ 3�� ����), �̻����� ���ݷ��� 30
     * ������3 : ��ȭ ����2 ���, (�����ð� ��������)���ּ� ��� ����.(������ �� �ڸ��� �������ظ� �ִ� ���� ����.) 3�� -> �̻��ϰ��� -> 3�� ��� -> ����1(��� �ϸ� ����2 ����) -> 3�� ��� -> �̻��� -> 3�� ��� -> ����1 (�� ���߿� ��ȭ����2�� ���������� ����)
    */
    Transform target;
    [SerializeField]
    Transform laserPos;
    [SerializeField]
    Transform laserDoublePos;
    [SerializeField]
    GameObject laser;

    int phase;
    int phase1Cnt;
    // �������ͽ�
    float hp;
    float maxHp;
    float targetHp;

    // ����1 ���� ����
    public bool doPattern1;
    float pattern1EffectTime;
    float pattern1DmgTime;
    [SerializeField, Tooltip("0�� ��ư, 1�� �¾ƾ� �ϴ� ��ü")]
    GameObject[] pattern1PuzzleObj;
    [SerializeField]
    GameObject pattern1PuzzleEffectPrefab;    // ���� ����Ʈ
    GameObject pattern1PuzzleEffect;            // �̸� �����ص� ��������Ʈ
    int savePos;

    // ����2 ���� ����
    public bool doPattern2;
    bool pattern2Ending;
    Transform pattern2Tr;
    public Transform[] pattern2Laser = new Transform[8];
    float pattern2Dis;
    float pattern2RotTime;
    public float pattern2RotPower;
    float pattern2DmgTime;
    float pattern2ProjectileTime;
    [SerializeField]
    GameObject pattern2Projectile;

    // ����3
    [SerializeField]
    GameObject missilePrefab;

    Rigidbody rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        target = GameObject.Find("Player").transform;
        pattern2Tr = GameObject.Find("UFOPattern2").transform;
        int i = 0;
        foreach(Transform tr in pattern2Tr.GetChild(0).transform) {
            pattern2Laser[i++] = tr;
        }
        pattern1PuzzleEffect = Instantiate(pattern1PuzzleEffectPrefab);
        pattern1PuzzleEffect.SetActive(false);

        pattern2Dis = 0;
        pattern2RotPower = 0.01f;
        savePos = 999;

        phase = 1;
        phase1Cnt = 2;
        maxHp = 200;
        hp = maxHp;
        targetHp = 0;
    }
    void Start() {
        laser.SetActive(false);
        // ����1
        //StartCoroutine(Pattern1Frame());
        //StartCoroutine(Pattern2());
        //StartCoroutine(Pattern3Frame(1));

        // ����3
        //GameObject missile = ObjectManager.instance.GetObject("st2Pattern3");
        //missile.transform.position = transform.position;
        //missile.transform.LookAt(target);
        //Vector3 vec = missile.transform.localEulerAngles;
        //vec.x = -60f;
        //missile.transform.localEulerAngles = vec;
        //missile.GetComponent<Rigidbody>().AddForce(missile.transform.forward * 10f, ForceMode.VelocityChange);
        //missile.GetComponent<Stage2Missile>().ChaseCon();
    }
    void Update() {
        if(phase == 1) {
            targetHp = 120;
            if(phase1Cnt == 0) {
                StartCoroutine(Pattern1Frame());
                phase1Cnt++;
            }
            if(phase1Cnt == 2) {
                StartCoroutine(Pattern2());
                phase1Cnt++;
            }
        }

        // ����1
        // �����Ҷ� laser�� setactive�� true�� ��������� ��.
        if (doPattern1 && !PlayerCtrl.dontCtrl) {
            pattern1EffectTime += Time.deltaTime;
            pattern1DmgTime += Time.deltaTime;
            // �÷��̾��� ��������� ��ǥ��. �׸��� ������ �ص� �������� �Ű澲�� �ʵ���.
            Vector3 dir = new Vector3(target.position.x, 66.7f, target.position.z);
            laserDoublePos.LookAt(dir);

            // �������� ��ġ�� �÷��̾ ���� �̵��ϵ��� ������.
            laserPos.localRotation = Quaternion.Lerp(laserPos.localRotation, laserDoublePos.localRotation, Time.deltaTime * 15f);

            RaycastHit hit;
            // Player�� Ground Ȯ��
            int layerMask = (1 << 3) + (1 << 6) + (1 << 9);
            // õõ�� �̵����� �� ��ġ�� ���̸� ��.
            Physics.Raycast(laserPos.position, laserPos.forward, out hit, 300, layerMask);
            //�������� ���̸� ���̸� ���� �Ÿ���ŭ �ø�. 
            laser.transform.localScale = new Vector3(1f, hit.distance / 2, 1f);
            if (pattern1EffectTime > 0.1f) {
                pattern1EffectTime = 0;
                StartCoroutine(Pattern1(hit));
            }
            if (pattern1DmgTime > 0.1f) {
                if (hit.collider.CompareTag("Player")) {
                    pattern1DmgTime = 0;
                    hit.collider.GetComponent<PlayerCtrl>().HpDown(0.2f);
                }
                if (hit.collider.CompareTag("Puzzle")) {

                    //Ÿ�� (����)
                    pattern1DmgTime = 0;
                    doPattern1 = false;
                    pattern1PuzzleEffect.transform.position = hit.collider.transform.position;
                    pattern1PuzzleEffect.SetActive(true);
                    laser.SetActive(false);
                    StartCoroutine(Camera.main.GetComponent<Shake>().ShakeCamera(0.2f, 1f));
                    hit.collider.GetComponent<Stage2Pattern1>().Hit();
                    rigid.AddForce(Vector3.down * 20f, ForceMode.VelocityChange);

                    StartCoroutine(WakeUp());
                }
            }
        }

        // ���� 2
        if (doPattern2) {
            pattern2DmgTime += Time.deltaTime;
            pattern2ProjectileTime += Time.deltaTime;
            // �������� ���� �ִٸ�, ���� ����.
            pattern2RotTime += Time.deltaTime;
            foreach (Transform tr in pattern2Laser) {
                Physics.Raycast(tr.position, tr.forward, out RaycastHit hit, pattern2Dis, (1 << 3) + (1 << 6));
                if (hit.collider) {
                    tr.localScale = new Vector3(1, 1, hit.distance / 2);
                    if(pattern2DmgTime > 1f) {
                        if (hit.collider.CompareTag("Player")) {
                            pattern2DmgTime = 0;
                            hit.collider.GetComponent<PlayerCtrl>().HpDown(10f);
                        }
                    }
                }
                else {
                    tr.localScale = new Vector3(1, 1, pattern2Dis / 2);
                }
            }
            if(pattern2ProjectileTime > 4) {
                pattern2ProjectileTime = 0;
                GameObject projectile = ObjectManager.instance.GetObject("st2Pattern2");
                projectile.transform.position = laserPos.position;
                projectile.transform.LookAt(target);
                float dis = Vector3.Distance(target.position, projectile.transform.position);
                dis /= 2.5f;
                projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * dis + projectile.transform.up *15, ForceMode.Impulse);
                StartCoroutine(Pattern2ProjectileCtrl(projectile.GetComponent<Rigidbody>(), dis));
            }
            pattern2Tr.Rotate(Vector3.up * pattern2RotPower);
            if(pattern2RotTime > 15 && !pattern2Ending) {
                // ����2 ����
                StartCoroutine(Pattern2End());
            }
        }
    }
    IEnumerator WakeUp() {
        yield return new WaitForSeconds(10f);
        float progress = 0;
        Vector3 downPos = transform.position;
        Vector3 targetPos = downPos;
        targetPos.y = 77;
        BoxCollider box = transform.GetChild(0).GetComponent<BoxCollider>();
        SphereCollider sphere = transform.GetChild(1).GetComponent<SphereCollider>();
        while(progress < 1) {
            progress += 0.05f;
            // y 77���� �ö󰡱�.
            transform.position = Vector3.Lerp(downPos, targetPos, progress / 1);
            if(progress > 0.5f && box.enabled) {
                box.enabled = false;
                sphere.enabled = false;
            }
            yield return new WaitForSeconds(0.05f);
        }
        box.enabled = true;
        sphere.enabled = true;

        if (phase == 1) {
            phase1Cnt++;
        }
    }
    IEnumerator Pattern1Frame() {
        laser.transform.localScale = Vector3.one;
        laser.SetActive(true);
        yield return new WaitForSeconds(1f);
        int rand = Random.Range(0, 6);
        while (savePos == rand) {
            rand = Random.Range(0, 6);
            yield return null;
        }
        
        // ������ ��ġ
        switch (rand) {
            case 0:
                pattern1PuzzleObj[0].transform.position = new Vector3(-16f, 65.4f, -21f);
                break;
            case 1:
                pattern1PuzzleObj[0].transform.position = new Vector3(-38f, 65.4f, -5f);
                break;
            case 2:
                pattern1PuzzleObj[0].transform.position = new Vector3(-24f, 65.4f, 18f);
                break;
            case 3:
                pattern1PuzzleObj[0].transform.position = new Vector3(4, 65.4f, 39);
                break;
            case 4:
                pattern1PuzzleObj[0].transform.position = new Vector3(31, 65.4f, 20);
                break;
            case 5:
                pattern1PuzzleObj[0].transform.position = new Vector3(33, 65.4f, -21);
                break;

        }
        doPattern1 = true;

        Vector3 dir = new Vector3(transform.position.x, pattern1PuzzleObj[0].transform.position.y, transform.position.z);
        pattern1PuzzleObj[0].transform.LookAt(dir);
        pattern1PuzzleObj[1].transform.position = pattern1PuzzleObj[0].transform.position + Vector3.down * 1.4f;
        pattern1PuzzleObj[1].transform.position += pattern1PuzzleObj[0].transform.forward * Random.Range(-10f, -15f) + pattern1PuzzleObj[0].transform.right * Random.Range(-3f, 3f);
        savePos = rand;
    }
    IEnumerator Pattern3Frame(float missileCnt) {
        // �̻��� ����
        for (int i = 0; i < missileCnt; i++) {
            GameObject missile = ObjectManager.instance.GetObject("st2Pattern3");
            missile.transform.position = transform.position;
            missile.transform.LookAt(target);
            Vector3 vec = missile.transform.localEulerAngles;
            vec.x = -60f;
            vec.y += Random.Range(-30f, 30f);
            missile.transform.localEulerAngles = vec;
            missile.GetComponent<Rigidbody>().AddForce(missile.transform.forward * 10f, ForceMode.VelocityChange);
            missile.GetComponent<Stage2Missile>().ChaseCon();

            yield return new WaitForSeconds(Random.Range(0.3f, 1f));
        }
        yield return null;
    }
    IEnumerator Pattern1(RaycastHit hit) {
        // �������� ���� ��ġ�� ��ƼŬ ���� 2�� �� ����
        Quaternion rot = Quaternion.LookRotation(hit.normal);
        GameObject eff = ObjectManager.instance.GetObject("explosion");
        eff.transform.position = hit.point;
        eff.transform.rotation = rot;
        yield return new WaitForSeconds(2f);
        ObjectManager.instance.ReturnObject(eff, "explosion");
        yield return null;
    }
    IEnumerator Pattern2() {
        float progress = 0;
        pattern2RotPower = 0.01f;
        Vector3 origin = pattern2Tr.position;
        origin.y = 64;
        // �Ʒ��ʿ��� ����� �ö��.
        while(progress < 1) {
            progress += 0.1f;
            pattern2Tr.position = Vector3.Lerp(origin, origin + Vector3.up * 2.2f, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
        float maxDistance = 120;
        progress = 0;
        while(progress < 1) {
            progress += 0.005f;
            if(progress > 0.5f && !doPattern2) {
                doPattern2 = true;
            }
            // progress�� 1�϶�, 0.1�� �ǵ���
            // progress�� 0.5�� 0.01�̴�.
            pattern2RotPower = progress * 0.1f;
            pattern2Dis = maxDistance * (progress / 1);
            foreach (Transform tr in pattern2Laser) {
                Physics.Raycast(tr.position, tr.forward, out RaycastHit hit, pattern2Dis, (1 << 3) + (1 << 6));
                if (hit.collider) {
                    tr.localScale = new Vector3(1, 1, hit.distance / 2);
                }
                else {
                    tr.localScale = new Vector3(1, 1, pattern2Dis / 2);
                }
            }
            yield return new WaitForSeconds(0.03f);
        }
        pattern2RotTime = 0;
        pattern2ProjectileTime = 2;
    }
    IEnumerator Pattern2ProjectileCtrl(Rigidbody projectileRigid, float dis) {
        yield return new WaitForSeconds(1f);
        projectileRigid.velocity = Vector3.zero;
        projectileRigid.AddForce(projectileRigid.gameObject.transform.forward * dis, ForceMode.Impulse);
    }
    IEnumerator Pattern2End() {
        pattern2Ending = true;
        float progress = 1;
        while (progress > 0) {
            progress -= 0.005f;
            //if (progress > 0.5f && !doPattern2) {
            //    doPattern2 = true;
            //}
            pattern2RotPower = progress * 0.1f; 
            pattern2Dis = 120 * (progress / 1);
            foreach (Transform tr in pattern2Laser) {
                Physics.Raycast(tr.position, tr.forward, out RaycastHit hit, pattern2Dis, (1 << 3) + (1 << 6));
                if (hit.collider) {
                    tr.localScale = new Vector3(1, 1, hit.distance / 2);
                }
                else {
                    tr.localScale = new Vector3(1, 1, pattern2Dis / 2);
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
        progress = 0;

        Vector3 origin = pattern2Tr.position;
        while (progress < 1) {
            progress += 0.1f;
            pattern2Tr.position = Vector3.Lerp(origin, origin - Vector3.up * 2.2f, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }

        doPattern2 = false;
        pattern2Ending = false;
        pattern2RotTime = 0;
    }
    IEnumerator Pattern3() {
        yield return null;
    }
    public void HpDown(float dmg) {
        hp -= dmg;
        Debug.Log(hp);
    }
}
