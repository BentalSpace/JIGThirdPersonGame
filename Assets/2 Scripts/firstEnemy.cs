using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstEnemy : MonoBehaviour {
    /*
    ������1
    - 3~4���� ������ �� ��� ���� 100�ٱ���
    ������2
    - �������� �ֱ������� �ϸ�, ����3 1ȸ ����� ������ 50�ٱ���
    ������3
    - �������� �ֱ�������(��Ÿ�� ����), ��ȭ ����3���� �� [???]
    */
    Transform target;
    [SerializeField]
    GameObject dangerZonePrefab;
    [SerializeField]
    GameObject pillarPrefab;
    [SerializeField]
    GameObject pattern2Particle;
    [SerializeField]
    GameObject pattern3Prefab;

    int phase;
    int phase1Cnt;
    int phase1AtkCnt;
    int phase1Pattern;
    int phase2Cnt;
    int phase2Pattern;
    int phase3Cnt;
    int phase3Pattern;

    public bool isGround;
    public bool isGetHitTime;

    bool pattern3End;
    bool pattern3PlusFollow;

    float hp;
    float maxHp;
    float targetHp;
    float downTime;

    float pattern1CurTime;
    float pattern1MaxTime;

    public List<GameObject> pattern3PlusObjects = new List<GameObject>();

    Rigidbody rigid;
    Animator anim;
    void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        target = GameObject.Find("Player").transform;
        maxHp = 150;
        hp = maxHp;
        targetHp = 0;
        pattern1MaxTime = 3.3f;
        phase = 1;
        phase1AtkCnt = Random.Range(2, 4);
        phase1Cnt = 0;
        phase1Pattern = 0;
        phase2Cnt = 0;
        phase2Pattern = 0;
        phase3Cnt = 0;
        phase3Pattern = 0;
    }
    void Start() {
        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out RaycastHit hit, 200, 1 << 6);
        Pattern1PlusFrame(40, 20, hit);
        // ����1 ��� Ʋ
        //RaycastHit hit;
        //Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        //for (int i = 0; i < 10; i++) {
        //    Vector3 randPos = hit.point + (Vector3.right * Random.Range(-10f, 10f)) + (Vector3.forward * Random.Range(-10f, 10f));
        //    GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
        //    dangerZone.transform.position = randPos + Vector3.up * 0.01f;
        //    dangerZone.transform.localScale = Vector3.one * 0.5f;
        //    StartCoroutine(Pattern1(randPos, dangerZone));
        //}

        // ���� 2
        //RaycastHit hit;
        //Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        //GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
        //dangerZone.transform.position = hit.point + Vector3.up * 0.01f;
        //dangerZone.transform.localScale = Vector3.one * 1.4f;

        //StartCoroutine(Pattern2(dangerZone));

        // ���� 3
        //RaycastHit hit;
        //Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        //GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
        //dangerZone.transform.position = hit.point + Vector3.up * 0.01f;
        //dangerZone.transform.localScale = Vector3.one * 0.7f;
        //StartCoroutine(Pattern3(dangerZone));
    }
    void Update() {
        pattern1CurTime += Time.deltaTime;
        // �� Ȯ��
        int layerMask = 1 << 6;
        isGround = Physics.SphereCast(transform.position + (Vector3.up * 0.6f), 0.4f, Vector3.down, out RaycastHit hit, 0.2f, layerMask);

        if (phase == 1) {
            targetHp = 100;
            if (pattern1CurTime > pattern1MaxTime) {
                pattern1CurTime = 0;
                phase1Cnt++;
                if (Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6)) {
                    Pattern1Frame(10, 10, hit);
                }
                if(phase1Cnt > phase1AtkCnt && phase1Pattern == 0) {
                    Invoke("Pattern2Frame", 2f);
                    pattern1MaxTime = 99999999;
                }
            }
        }

        if (phase == 2) {
            targetHp = 50;
            if (pattern1CurTime > pattern1MaxTime) {
                pattern1CurTime = 0;
                phase2Cnt++;
                if (Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6)) {
                    Pattern1Frame(20, 15, hit);
                }
                if(phase2Cnt > 1 && phase2Pattern == 0) {
                    Invoke("Pattern3Frame", 1.5f);
                    phase2Pattern++;
                }
                if (phase2Pattern == 1 && pattern3End && phase2Cnt > 4) {
                    Invoke("Pattern2Frame", 3.0f);
                    phase2Pattern++;
                }
            }
        }
        if(phase == 3) {
            targetHp = 0;
            if(pattern1CurTime > pattern1MaxTime) {
                pattern1CurTime = 0;
                phase3Cnt++;
                if (Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6)) {
                    Pattern1Frame(30, 20, hit);
                }
                if (phase3Cnt > 2 && phase3Pattern == 0) {
                    Invoke("Pattern3PlusFrame", 1.5f);
                    phase3Pattern++;
                }
                if(phase3Pattern == 1 && phase3Cnt > 6) {
                    Invoke("Pattern2Frame", 2.5f);
                    phase3Pattern++;
                }
            }
        }

        //Debug.DrawRay(target.position + Vector3.up * 100, Vector3.down * 500, Color.red);
        if (isGetHitTime) {
            downTime += Time.deltaTime;
            if (hp < targetHp) {
                // hp�� ���������� ����
                isGetHitTime = false;
                anim.SetBool("isDizzy", false);
                if (hp <= 0) {
                    phase++;
                    anim.SetTrigger("Die");
                }
                else {
                    Invoke("GoUp", 1.5f);
                }
            }
            else if(downTime > 10) {
                isGetHitTime = false;
                anim.SetBool("isDizzy", false);
                Invoke("GoUpFail", 1.5f);
            }
        }

        if (pattern3PlusFollow) {
            if (pattern3PlusObjects.Count > 0) {
                if (pattern3PlusObjects[0].activeSelf) {
                    for (int i = 0; i < 2; i++) {
                        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
                        float y = pattern3PlusObjects[i].transform.position.y;
                        if (i == 0) {
                            pattern3PlusObjects[i].transform.position = Vector3.Lerp(pattern3PlusObjects[i].transform.position, new Vector3(target.position.x, pattern3PlusObjects[i].transform.position.y, 30), Time.deltaTime * 5);
                        }
                        else {
                            pattern3PlusObjects[i].transform.position = Vector3.Lerp(pattern3PlusObjects[i].transform.position, new Vector3(target.position.x, pattern3PlusObjects[i].transform.position.y, -30), Time.deltaTime * 5);
                        }
                    }
                }
            }
        }
    }

    void GoUp() {
        downTime = 0;
        phase++;
        if(phase == 2) {
            rigid.AddForce(Vector3.up * 150, ForceMode.VelocityChange);
            pattern1MaxTime = 3;
        }
        if(phase == 3) {
            rigid.AddForce(Vector3.up * 150, ForceMode.VelocityChange);
            pattern1MaxTime = 2.5f;
        }
    }
    void GoUpFail() {
        rigid.AddForce(Vector3.up * 150, ForceMode.VelocityChange);
        downTime = 0;
        switch (phase) {
            case 1:
                pattern1MaxTime = 3.3f;
                phase1Cnt = 0;
                phase1Pattern = 0;
                break;
            case 2:
                pattern1MaxTime += 0.1f;
                phase2Pattern = 0;
                phase2Cnt = 1;
                break;
            case 3:
                pattern1MaxTime += 0.1f;
                phase3Pattern = 0;
                phase3Cnt = 2;
                break;
        }
    }
    void Pattern1Frame(int cnt, float range, RaycastHit raycastHit) {
        RaycastHit hit = raycastHit;
        for (int i = 0; i < cnt; i++) {
            Vector3 randPos = hit.point + (Vector3.right * Random.Range(-range, range)) + (Vector3.forward * Random.Range(-range, range));
            if (i == 0) {
                randPos = hit.point;
            }
            while (!Physics.Raycast(randPos + Vector3.up * 30, Vector3.down, out RaycastHit hit2, 200, 1 << 6)) {
                randPos = target.position + (Vector3.right * Random.Range(-range, range)) + (Vector3.forward * Random.Range(-range, range));
            }
            GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
            dangerZone.transform.position = randPos + Vector3.up * 0.01f;
            dangerZone.transform.localScale = Vector3.one * 0.5f;
            StartCoroutine(Pattern1(randPos, dangerZone));
        }
    }
    void Pattern1PlusFrame(int cnt, float range, RaycastHit raycastHit) {
        RaycastHit hit = raycastHit;
        for (int i = 0; i < cnt; i++) {
            Vector3 randPos = hit.point + (Vector3.right * Random.Range(-range, range)) + (Vector3.forward * Random.Range(-range, range));
            if (i == 0) {
                randPos = hit.point;
            }
            while (!Physics.Raycast(randPos + Vector3.up * 30, Vector3.down, out RaycastHit hit2, 200, 1 << 6)) {
                randPos = target.position + (Vector3.right * Random.Range(-range, range)) + (Vector3.forward * Random.Range(-range, range));
            }
            GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
            dangerZone.transform.position = randPos + Vector3.up * 0.01f;
            dangerZone.transform.localScale = Vector3.one * 0.1f;
            StartCoroutine(Pattern1Plus(randPos, dangerZone));
        }
    }
    void Pattern2Frame() {
        RaycastHit hit;
        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
        dangerZone.transform.position = hit.point + Vector3.up * 0.01f;
        dangerZone.transform.localScale = Vector3.one * 1.4f;

        StartCoroutine(Pattern2(dangerZone, 3));
    }
    void Pattern3Frame() {
        RaycastHit hit;
        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        GameObject dangerZone = ObjectManager.instance.GetObject("dangerZone");
        dangerZone.transform.position = hit.point + Vector3.up * 0.01f;
        dangerZone.transform.localScale = Vector3.one * 0.7f;
        StartCoroutine(Pattern3(dangerZone));
        pattern3End = false;
    }
    void Pattern3PlusFrame() {
        RaycastHit hit;
        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        GameObject dangerZone1 = ObjectManager.instance.GetObject("dangerZone");
        GameObject dangerZone2 = ObjectManager.instance.GetObject("dangerZone");
        dangerZone1.transform.position = hit.point + Vector3.up * 0.01f + Vector3.forward * 30;
        dangerZone2.transform.position = hit.point + Vector3.up * 0.01f + Vector3.forward * -30;
        dangerZone1.transform.localScale = Vector3.one * 0.7f;
        dangerZone2.transform.localScale = Vector3.one * 0.7f;
        StartCoroutine(Pattern3Plus(dangerZone1));
        StartCoroutine(Pattern3Plus(dangerZone2));
        pattern3PlusObjects = new List<GameObject>();
    }

    IEnumerator Pattern1(Vector3 pos, GameObject zone) {
        // �ϴÿ��� ��� ������
        float randTime = Random.Range(0.7f, 1.3f);
        yield return new WaitForSeconds(randTime);
        GameObject pillar = ObjectManager.instance.GetObject("pillar");
        pillar.transform.position = pos + Vector3.up * 10f;
        pillar.transform.localScale = new Vector3(0.5f, 10, 0.5f);
        float progress = 0;
        float maxProgress = 1;
        while (progress < maxProgress) {
            progress += 0.1f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 10, 0.5f), new Vector3(0.1f, 10, 0.1f), progress / maxProgress);
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.3f);
        progress = 0;
        while (progress < maxProgress) {
            progress += 0.05f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 10, 0.5f), new Vector3(3, 10, 3), progress / maxProgress);
            pillar.GetComponentInChildren<Light>().intensity += 1;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        progress = 0;
        while (progress < maxProgress) {
            progress += 0.1f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(3, 10, 3), new Vector3(0, 10, 0), progress / maxProgress);
            pillar.GetComponentInChildren<Light>().intensity -= 1;
            yield return new WaitForSeconds(0.07f);
        }
        ObjectManager.instance.ReturnObject(pillar, "pillar");
        ObjectManager.instance.ReturnObject(zone, "dangerZone");
    }
    IEnumerator Pattern1Plus(Vector3 pos, GameObject zone) {
        // �ϴÿ��� ��� ���� ������
        float randTime = Random.Range(0.7f, 1.3f);
        yield return new WaitForSeconds(randTime);
        GameObject pillar = ObjectManager.instance.GetObject("pillar");
        pillar.transform.position = pos + Vector3.up * 10f;
        pillar.transform.localScale = new Vector3(0.3f, 10, 0.3f);
        float progress = 0;
        float maxProgress = 1;
        while (progress < maxProgress) {
            progress += 0.1f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(0.3f, 10, 0.3f), new Vector3(0.05f, 10, 0.05f), progress / maxProgress);
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.3f);
        progress = 0;
        while (progress < maxProgress) {
            progress += 0.05f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(0.05f, 10, 0.05f), new Vector3(0.5f, 10, 0.5f), progress / maxProgress);
            pillar.GetComponentInChildren<Light>().intensity += 1;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        progress = 0;
        while (progress < maxProgress) {
            progress += 0.1f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 10, 0.5f), new Vector3(0, 10, 0), progress / maxProgress);
            pillar.GetComponentInChildren<Light>().intensity -= 1;
            yield return new WaitForSeconds(0.07f);
        }
        ObjectManager.instance.ReturnObject(pillar, "pillar");
        ObjectManager.instance.ReturnObject(zone, "dangerZone");
    }
    IEnumerator Pattern2(GameObject zone, float downTime) {
        //downTime �⺻ 1�̿���
        float progress = 0;
        // �׶��常 Ȯ��
        int layerMask = 1 << 6;
        for (int i = 0; i < 3; i++) {
            progress = 0;
            zone.SetActive(true);
            while (progress < downTime) {
                progress += 0.01f;
                zone.transform.position = new Vector3(target.position.x, zone.transform.position.y, target.position.z);
                yield return null;
            }
            zone.SetActive(false);
            transform.position = new Vector3(zone.transform.position.x, target.position.y + 20f, zone.transform.position.z);
            rigid.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.1f);
            // �Ʒ��� ���
            rigid.AddForce(Vector3.down * 100, ForceMode.VelocityChange);

            while (true) {
                RaycastHit hit;
                isGround = Physics.SphereCast(transform.position + (Vector3.up * 0.6f), 0.4f, Vector3.down, out hit, 0.2f, layerMask);
                Quaternion rot = new Quaternion();
                rot.eulerAngles = new Vector3(-90, 0, 0);
                if (isGround) {
                    Instantiate(pattern2Particle, hit.point, rot);
                    break;
                }
                yield return null;
            }

            // ������ ���Ϳ� �÷��̾��� �Ÿ��� ª�ٸ�, �÷��̾�� �ణ�� ���ϰ� ����
            if (Vector3.Distance(target.position, transform.position) < 10) {
                PlayerCtrl.dontCtrl = true;
                PlayerCtrl.isOneTime = true;
                target.GetComponent<Rigidbody>().velocity = Vector3.zero;
                target.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
                yield return new WaitForSeconds(0.7f);
                PlayerCtrl.dontCtrl = false;
                yield return new WaitForSeconds(0.5f);
            }
            else {
                yield return new WaitForSeconds(1f);
            }
            rigid.AddForce(Vector3.up * 50, ForceMode.VelocityChange);
        }
        // dangerZone scale => 1.4f

        // ������ ���� �� �����ӿ��� ���� �� �� �ִ� �ð��� �ش�.
        progress = 0;
        zone.SetActive(true);
        while (progress < 1) {
            progress += 0.01f;
            zone.transform.position = new Vector3(target.position.x, zone.transform.position.y, target.position.z);
            yield return null;
        }
        transform.position = new Vector3(zone.transform.position.x, target.position.y + 20f, zone.transform.position.z);
        zone.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        rigid.AddForce(Vector3.down * 100, ForceMode.VelocityChange);

        while (true) {
            RaycastHit hit;
            isGround = Physics.SphereCast(transform.position + (Vector3.up * 0.6f), 0.4f, Vector3.down, out hit, 0.2f, layerMask);
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(-90, 0, 0);
            if (isGround) {
                Instantiate(pattern2Particle, hit.point, rot);
                break;
            }
            yield return null;
        }

        // ������ ����.
        if (Vector3.Distance(target.position, transform.position) < 10) {
            PlayerCtrl.dontCtrl = true;
            PlayerCtrl.isOneTime = true;
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            target.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
            yield return new WaitForSeconds(0.5f);
            PlayerCtrl.dontCtrl = false;
            yield return new WaitForSeconds(0.5f);
        }
        //���� �ð�
        zone.SetActive(true);
        ObjectManager.instance.ReturnObject(zone, "dangerZone");
        anim.SetBool("isDizzy", true);
        isGetHitTime = true;
    }
    IEnumerator Pattern3(GameObject zone) {
        yield return new WaitForSeconds(0.5f);
        // �÷��̾ ������ ������Ʈ ���� �� �Ʒ��� ������ ����
        GameObject item = ObjectManager.instance.GetObject("pattern3");
        item.transform.position = zone.transform.position + Vector3.up * 20;
        ObjectManager.instance.ReturnObject(zone, "dangerZone");
        float progress = 0;
        Vector3 itemVec = item.transform.position;
        while (progress < 1) {
            progress += 0.03f;
            item.transform.position = Vector3.Slerp(itemVec, itemVec + Vector3.down * 10, progress);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.5f);
        // ���� �ø���.
        progress = 0;
        Transform knife = item.transform.GetChild(1);
        while (progress < 1) {
            progress += 0.01f;
            knife.localScale = Vector3.Lerp(new Vector3(5, 0.2f, 3), new Vector3(100, 0.2f, 3), progress);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        // ���� ����
        float power = 0.05f;
        progress = 0;
        float time = 0;
        float randTime = Random.Range(9.5f, 11);
        while (progress < randTime) {
            Debug.Log(power);
            time += Time.deltaTime;
            progress += Time.deltaTime;
            knife.Rotate(Vector3.up * power);
            if (progress < 3f) {
                if (time > 0.3f) {
                    if (power < 0.45f)
                        power += 0.05f;
                    time = 0;
                }
            }
            if (progress > randTime-2) {
                if (time > 0.2f) {
                    if (power > 0.05f)
                        power -= 0.05f;
                    time = 0;
                }
            }
            yield return null;
        }

        // ���� ����
        yield return new WaitForSeconds(0.5f);
        progress = 0;
        while (progress < 1) {
            progress += 0.01f;
            knife.localScale = Vector3.Lerp(new Vector3(100, 0.2f, 3), new Vector3(5, 0.2f, 3), progress);
            yield return null;
        }

        // ������Ʈ�� �÷�����
        yield return new WaitForSeconds(0.3f);
        progress = 0;
        itemVec = item.transform.position;
        while (progress < 1) {
            progress += 0.03f;
            item.transform.position = Vector3.Slerp(itemVec, itemVec + Vector3.up * 20, progress);
            yield return new WaitForSeconds(0.02f);
        }
        ObjectManager.instance.ReturnObject(item, "pattern3");
        pattern3End = true;
    }

    IEnumerator Pattern3Plus(GameObject zone) {
        yield return new WaitForSeconds(1f);
        // �÷��̾ ������ ������Ʈ ���� �� �Ʒ��� ������ ����
        GameObject item = ObjectManager.instance.GetObject("pattern3Plus");
        pattern3PlusObjects.Add(item);
        item.transform.position = zone.transform.position + Vector3.up * 20;
        ObjectManager.instance.ReturnObject(zone, "dangerZone");
        float progress = 0;
        Vector3 itemVec = item.transform.position;
        while (progress < 1) {
            progress += 0.03f;
            item.transform.position = Vector3.Slerp(itemVec, itemVec + Vector3.down * 10, progress);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.5f);
        // ���� �ø���.
        progress = 0;
        Transform knife = item.transform.GetChild(1);
        while (progress < 1) {
            progress += 0.01f;
            knife.localScale = Vector3.Lerp(new Vector3(5, 0.2f, 3), new Vector3(100, 0.2f, 3), progress);
            yield return null;
        }

        pattern3PlusFollow = true;
        yield return new WaitForSeconds(0.5f);
        // ���� ����
        float power = 0.05f;
        progress = 0;
        float time = 0;
        float randTime = Random.Range(9.5f, 11);
        while (progress < randTime) {
            Debug.Log(power);
            time += Time.deltaTime;
            progress += Time.deltaTime;
            knife.Rotate(Vector3.up * power);
            if (progress < 3f) {
                if (time > 0.3f) {
                    if (power < 0.45f)
                        power += 0.05f;
                    time = 0;
                }
            }
            if (progress > randTime - 2) {
                if (time > 0.2f) {
                    if (power > 0.05f)
                        power -= 0.05f;
                    time = 0;
                }
            }
            yield return null;
        }

        pattern3PlusFollow = false;
        // ���� ����
        yield return new WaitForSeconds(0.5f);
        progress = 0;
        while (progress < 1) {
            progress += 0.01f;
            knife.localScale = Vector3.Lerp(new Vector3(100, 0.2f, 3), new Vector3(5, 0.2f, 3), progress);
            yield return null;
        }

        // ������Ʈ�� �÷�����
        yield return new WaitForSeconds(0.3f);
        progress = 0;
        itemVec = item.transform.position;
        while (progress < 1) {
            progress += 0.03f;
            item.transform.position = Vector3.Slerp(itemVec, itemVec + Vector3.up * 20, progress);
            yield return new WaitForSeconds(0.02f);
        }
        ObjectManager.instance.ReturnObject(item, "pattern3");
    }

    public void HpDown(float dmg) {
        anim.SetTrigger("Hit");
        hp -= dmg;
        Debug.Log(hp);
    }
    void OnDrawGizmos() {
        // �� Ȯ��
        //int layerMask = 1 << 6;
        ////layerMask = ~layerMask;
        //isGround = Physics.SphereCast(transform.position + (Vector3.up * 0.6f), 0.4f, Vector3.down, out RaycastHit hit, 0.2f, layerMask);

        //if (isGround) {
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawRay(transform.position + (Vector3.up * 0.6f), Vector3.down * hit.distance);
        //    Gizmos.DrawWireSphere(transform.position + (Vector3.up * 0.6f) + Vector3.down * hit.distance, 0.4f);
        //}
        //else {
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(transform.position + (Vector3.up * 0.6f), Vector3.down * 0.2f);
        //}

        //// ������ Ȯ��
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
