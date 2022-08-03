using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondEnemy : MonoBehaviour
{
    /*
     * 체력 : 200
     * 페이즈1 : 체력이 120이 될때까지, 패턴1 사용-> 플레이어가 공격 ->  패턴 2 사용 -> 패턴 1 사용 -> 플레이어가 공격.
     * 페이즈2 : 체력이 60(미사일 2대 맞추기)이 될때까지, 패턴3 공격 (미사일은 일정시간 간격으로 맵에 배치 되는 개수는 3개 이하), 미사일의 공격력은 30
     * 페이즈3 : 강화 패턴2 사용, (일정시간 간격으로)우주선 찍기 공격.(찍으면 그 자리에 지속피해를 주는 장판 생성.) 3번 -> 미사일공격 -> 3번 찍기 -> 패턴1(사용 하면 패턴2 정지) -> 3번 찍기 -> 미사일 -> 3번 찍기 -> 패턴1 (그 와중에 강화패턴2는 지속적으로 실행)
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
    // 스테이터스
    float hp;
    float maxHp;
    float targetHp;

    // 패턴1 관리 변수
    public bool doPattern1;
    float pattern1EffectTime;
    float pattern1DmgTime;
    [SerializeField, Tooltip("0은 버튼, 1은 맞아야 하는 물체")]
    GameObject[] pattern1PuzzleObj;
    [SerializeField]
    GameObject pattern1PuzzleEffectPrefab;    // 폭파 이펙트
    GameObject pattern1PuzzleEffect;            // 미리 생성해둔 폭파이펙트
    int savePos;

    // 패턴2 관리 변수
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

    // 패턴3
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
        // 패턴1
        //StartCoroutine(Pattern1Frame());
        //StartCoroutine(Pattern2());
        //StartCoroutine(Pattern3Frame(1));

        // 패턴3
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

        // 패턴1
        // 시작할때 laser의 setactive를 true로 변경해줘야 함.
        if (doPattern1 && !PlayerCtrl.dontCtrl) {
            pattern1EffectTime += Time.deltaTime;
            pattern1DmgTime += Time.deltaTime;
            // 플레이어의 몸통방향을 목표로. 그리고 점프를 해도 점프값은 신경쓰지 않도록.
            Vector3 dir = new Vector3(target.position.x, 66.7f, target.position.z);
            laserDoublePos.LookAt(dir);

            // 레이저의 위치가 플레이어를 향해 이동하듯이 움직임.
            laserPos.localRotation = Quaternion.Lerp(laserPos.localRotation, laserDoublePos.localRotation, Time.deltaTime * 15f);

            RaycastHit hit;
            // Player와 Ground 확인
            int layerMask = (1 << 3) + (1 << 6) + (1 << 9);
            // 천천히 이동중인 그 위치로 레이를 쏨.
            Physics.Raycast(laserPos.position, laserPos.forward, out hit, 300, layerMask);
            //레이저의 길이를 레이를 맞춘 거리만큼 늘림. 
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

                    //타격 (스턴)
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

        // 패턴 2
        if (doPattern2) {
            pattern2DmgTime += Time.deltaTime;
            pattern2ProjectileTime += Time.deltaTime;
            // 레이저에 적이 있다면, 길이 조절.
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
                // 패턴2 종료
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
            // y 77까지 올라가기.
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
        
        // 랜덤한 위치
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
        // 미사일 공격
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
        // 레이저를 맞춘 위치에 파티클 생성 2초 후 제거
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
        // 아래쪽에서 기둥이 올라옴.
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
            // progress가 1일때, 0.1이 되도록
            // progress가 0.5면 0.01이다.
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
