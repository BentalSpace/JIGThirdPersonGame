using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondEnemy : MonoBehaviour
{
    /*
     * 체력 : 200
     * 페이즈1 : 체력이 120이 될때까지, 패턴1 사용-> 플레이어가 공격 ->  패턴 2 사용 -> 패턴 1 사용 -> 플레이어가 공격.
     * 페이즈2 : 체력이 60(미사일 2대 맞추기)이 될때까지, 패턴3 공격 (미사일은 일정시간 간격으로 맵에 배치 되는 개수는 3개 이하), 미사일의 공격력은 30
     * 페이즈3 : 강화 패턴2 사용, (일정시간 간격으로)우주선 찍기 공격.(찍으면 그 자리에 지속피해를 주는 장판 생성.) 3번 -> 미사일공격2개 -> 3번 찍기 -> 패턴1(사용 후 터뜨리면 회전 정지) -> 3번 찍기 -> 미사일 -> 3번 찍기 -> 패턴1 (그 와중에 강화패턴2는 지속적으로 실행)
    */
    [SerializeField]
    GameObject fire;
    Transform target;
    [SerializeField]
    Transform laserPos;
    [SerializeField]
    Transform laserDoublePos;
    [SerializeField]
    GameObject laser;

    int phase;
    int phase1Cnt;
    int phase2Cnt;
    float phase2FirstTime;
    int phase3Cnt;
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
    Transform[] pattern2Laser = new Transform[8];
    float pattern2Dis;
    float pattern2RotTime;
    public float pattern2RotPower;
    float pattern2DmgTime;
    float pattern2ProjectileTime;
    [SerializeField]
    GameObject pattern2Projectile;
    public int curMissileCnt;
    int maxMissileCnt;
    float missileSpawnCurTime;
    float missileSpawnMaxTime;

    // 패턴2플러스
    bool doPattern2Plus;
    float pattern2PlusTime;
    Transform[] pattern2PlusTr = new Transform[3];
    Transform[] pattern2PlusLaser = new Transform[12];
    int pattern2PlusObjCnt;
    bool[] dopattern2PlusSmall = new bool[3];
    float[] pattern2PlusSmallRotPower = new float[3];
    float[] pattern2PlusSmallDis = new float[3];
    float[] pattern2PlusDmgTime = new float[3];
    bool isRotate;

    // 패턴3
    [SerializeField]
    GameObject missilePrefab;

    Rigidbody rigid;

    bool phaseChange;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        target = GameObject.Find("Player").transform;
        pattern2Tr = GameObject.Find("UFOPattern2").transform;
        int i = 0;
        foreach(Transform tr in pattern2Tr.GetChild(0).transform) {
            pattern2Laser[i++] = tr;
        }
        pattern2PlusTr[0] = GameObject.Find("UFOPattern2Plus 1").transform;
        pattern2PlusTr[1] = GameObject.Find("UFOPattern2Plus 2").transform;
        pattern2PlusTr[2] = GameObject.Find("UFOPattern2Plus 3").transform;
        i = 0;
        for (int j = 0; j < pattern2PlusTr.Length; j++) {
            foreach (Transform tr in pattern2PlusTr[j].GetChild(0).transform) {
                pattern2PlusLaser[i++] = tr;
            }
        }
        pattern1PuzzleEffect = Instantiate(pattern1PuzzleEffectPrefab);
        pattern1PuzzleEffect.SetActive(false);

        pattern2Dis = 0;
        pattern2RotPower = 0.01f;
        savePos = 999;

        phase = 3;
        phase1Cnt = 0;
        phase2Cnt = 0;
        phase3Cnt = 0;
        maxHp = 200;
        hp = maxHp;
        targetHp = 0;

        curMissileCnt = 0;
        maxMissileCnt = 3;
        missileSpawnCurTime = 0;
        missileSpawnMaxTime = 0;
        pattern2PlusObjCnt = 0;

        phaseChange = false;
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

        // 패턴2 Plus
        //StartCoroutine(Pattern2Plus());
        //StartCoroutine(DownAtk());
    }
    void Update() {
        if(phase == 1) {
            targetHp = 120;
            if(phase1Cnt == 0) {
                StartCoroutine(Pattern1Frame());
                phase1Cnt++;
            }
            else if(phase1Cnt == 2) {
                StartCoroutine(Pattern2());
                phase1Cnt++;
            }
            else if(phase1Cnt == 4) {
                StartCoroutine(Pattern1Frame(2));
                phase1Cnt++;
            }
        }
        else if(phase == 2) {
            phase2FirstTime += Time.deltaTime;
            Debug.Log(phase2FirstTime);
            targetHp = 60;
            if (phase2Cnt == 0) {
                if (phase2FirstTime > 5) {
                    phase2Cnt++;
                }
            }
            else {
                if (maxMissileCnt > curMissileCnt) {
                    if ((missileSpawnCurTime > missileSpawnMaxTime) || curMissileCnt == 0) {
                        missileSpawnCurTime = 0;
                        StartCoroutine(Pattern3Frame(1));
                        curMissileCnt++;
                        missileSpawnMaxTime = Random.Range(6f, 8f);
                    }
                    else {
                        missileSpawnCurTime += Time.deltaTime;
                    }
                }
            }
        }
        else if(phase == 3) {
            targetHp = 0;
            maxMissileCnt = 2;
            if(phase3Cnt == 0) {
                StartCoroutine(Pattern2Plus());
                phase3Cnt++;
            }
            else if(phase3Cnt == 2) {
                // 3번 찍기
                StartCoroutine(DownAtk());
                phase3Cnt++;
            }
            else if(phase3Cnt == 4) {
                // 미사일 공격
                StartCoroutine(Pattern3Frame(2));
                curMissileCnt = 2;
                phase3Cnt++;
            }
            else if(phase3Cnt == 5) {
                if(curMissileCnt <= 0) {
                    phase3Cnt++;
                }
            }
            else if(phase3Cnt == 6) {
                //3번 찍기
                StartCoroutine(DownAtk());
                phase3Cnt++;
            }
            else if(phase3Cnt == 8) {
                // 패턴1 공격
                // 그후 2번으로 이동
                StartCoroutine(Pattern1Frame(1));
                phase3Cnt++;
            }
        }
        Pattern1Update();
        Pattern2Update();

        // 패턴2plus
        if (doPattern2Plus) {
            // 10초 후부터 찍기공격  | 10초 후 십자레이저 1개 추가 | 10초 후 십자레이저 추가 | 10초 후 십자레이저 추가.
            pattern2DmgTime += Time.deltaTime;
            pattern2PlusTime += Time.deltaTime;
            for(int i = 0; i < pattern2PlusObjCnt; i++) {
                if (!dopattern2PlusSmall[i])
                    break;
                pattern2PlusDmgTime[i] += Time.deltaTime;
            }


            //회전
            if (isRotate) {
                pattern2Tr.Rotate(Vector3.up * pattern2RotPower);
                // 작은애들 회전
                for (int i = 0; i < pattern2PlusObjCnt; i++) {
                    if (!dopattern2PlusSmall[i])
                        break;
                    pattern2PlusTr[i].Rotate(Vector3.up * pattern2PlusSmallRotPower[i]);
                }
            }
            
            // 레이저 중간에 플레이어가 있다면 길이 조절과 hp감소
            foreach (Transform tr in pattern2Laser) {
                Physics.Raycast(tr.position, tr.forward, out RaycastHit hit, pattern2Dis, (1 << 3) + (1 << 6));
                if (hit.collider) {
                    tr.localScale = new Vector3(1, 1, hit.distance / 2);
                    if (pattern2DmgTime > 0.1f) {
                        if (hit.collider.CompareTag("Player")) {
                            pattern2DmgTime = 0;
                            hit.collider.GetComponent<PlayerCtrl>().HpDown(5f);
                        }
                    }
                }
                else {
                    tr.localScale = new Vector3(1, 1, pattern2Dis / 2);
                }
            }
            // 작은 애들 길이 조절
            for(int i = 0; i < pattern2PlusObjCnt * 4; i++) {
                // 0123은 0, 4567은 1, 891011은 2
                Physics.Raycast(pattern2PlusLaser[i].position, pattern2PlusLaser[i].forward, out RaycastHit hit, pattern2PlusSmallDis[i / 4], (1 << 3) + (1 << 6));
                if (hit.collider) {
                    pattern2PlusLaser[i].localScale = new Vector3(1, 1, hit.distance / 2);
                    if(pattern2PlusDmgTime[i / 4] > 0.3f) {
                        if (hit.collider.CompareTag("Player")) {
                            pattern2PlusDmgTime[i / 4] = 0;
                            hit.collider.GetComponent<PlayerCtrl>().HpDown(5);
                        }
                    }
                }
                else {
                    pattern2PlusLaser[i].localScale = new Vector3(1, 1, pattern2PlusSmallDis[i / 4]);
                }
            }
            if(pattern2PlusTime > 10) {
                pattern2PlusTime = 0;
                if(pattern2PlusObjCnt < 3) {
                    StartCoroutine(Pattern2PlusSmall(pattern2PlusTr[pattern2PlusObjCnt], pattern2PlusObjCnt));
                    pattern2PlusObjCnt++;
                }
            }
        }

        // HP감소
        if (hp <= targetHp) {
            if (phase == 1) {
                StopCoroutine(WakeUp());
                StartCoroutine(WakeUp(0));
                phase++;
                //if (!phaseChange)
                //    StartCoroutine(PhaseChange());
            }
            else {
                phase++;
            }
        }
    }
    IEnumerator PhaseChange() {
        phaseChange = true;
        yield return new WaitForSeconds(5f);
        phase++;
        phaseChange = false;
    }
    public void Pattern1Update() {
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
                    isRotate = false;
                    StartCoroutine(WakeUp());
                }
            }
        }
    }
    public void Pattern2Update() {
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
                    if (pattern2DmgTime > 0.5f) {
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
            // 총 발사
            if (pattern2ProjectileTime > 4) {
                pattern2ProjectileTime = 0;
                GameObject projectile = ObjectManager.instance.GetObject("st2Pattern2");
                projectile.transform.position = laserPos.position;
                projectile.transform.LookAt(target);
                float dis = Vector3.Distance(target.position, projectile.transform.position);
                dis /= 2.5f;
                projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * dis + projectile.transform.up * 15, ForceMode.Impulse);
                StartCoroutine(Pattern2ProjectileCtrl(projectile.GetComponent<Rigidbody>(), dis));
            }
            //회전
            pattern2Tr.Rotate(Vector3.up * pattern2RotPower);
            if (pattern2RotTime > 15 && !pattern2Ending) {
                // 패턴2 종료
                StartCoroutine(Pattern2End());
            }
        }
    }
    IEnumerator WakeUp(float waitTime = 10) {
        yield return new WaitForSeconds(waitTime);
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
            if(phase1Cnt >= 4) {
                phase1Cnt = 2;
            }
            else {
                phase1Cnt++;
            }
        }
        else if(phase == 3) {
            phase3Cnt = 2;
            isRotate = true;
        }
    }
    IEnumerator Pattern1Frame(float waitTime = 0) {
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
                pattern1PuzzleObj[0].transform.position = new Vector3(4, 65.4f, 34);
                break;
            case 4:
                pattern1PuzzleObj[0].transform.position = new Vector3(31, 65.4f, 20);
                break;
            case 5:
                pattern1PuzzleObj[0].transform.position = new Vector3(33, 65.4f, -21);
                break;

        }

        Vector3 dir = new Vector3(transform.position.x, pattern1PuzzleObj[0].transform.position.y, transform.position.z);
        pattern1PuzzleObj[0].transform.LookAt(dir);
        pattern1PuzzleObj[1].transform.position = pattern1PuzzleObj[0].transform.position + Vector3.down * 1.4f;
        pattern1PuzzleObj[1].transform.position += pattern1PuzzleObj[0].transform.forward * Random.Range(-10f, -15f) + pattern1PuzzleObj[0].transform.right * Random.Range(-3f, 3f);
        savePos = rand;

        pattern1PuzzleObj[0].SetActive(true);
        pattern1PuzzleObj[1].SetActive(true);

        yield return new WaitForSeconds(waitTime);
        doPattern1 = true;
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

            yield return new WaitForSeconds(Random.Range(4, 6f));
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
            pattern2RotPower = progress * 0.15f;
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
                if (progress <= 0) {
                    pattern2Dis = 0;
                }
                else {
                    Physics.Raycast(tr.position, tr.forward, out RaycastHit hit, pattern2Dis, (1 << 3) + (1 << 6));
                    if (hit.collider) {
                        tr.localScale = new Vector3(1, 1, hit.distance / 2);
                    }
                    else {
                        tr.localScale = new Vector3(1, 1, pattern2Dis / 2);
                    }
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

        if(phase == 1) {
            phase1Cnt++;
        }
    }
    IEnumerator Pattern2Plus() {
        float progress = 0;
        isRotate = true;
        pattern2RotPower = 0.01f;
        Vector3 origin = pattern2Tr.position;
        origin.y = 64;
        // 아래쪽에서 기둥이 올라옴.
        while (progress < 1) {
            progress += 0.1f;
            pattern2Tr.position = Vector3.Lerp(origin, origin + Vector3.up * 2.2f, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
        float maxDistance = 120;
        progress = 0;
        while (progress < 1) {
            progress += 0.005f;
            if (progress > 0.5f && !doPattern2Plus) {
                doPattern2Plus = true;
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
        pattern2PlusTime = 0;
        if(phase == 3) {
            phase3Cnt++;
        }
    }
    IEnumerator Pattern2PlusSmall(Transform small, int num) {
        float progress = 0;
        Vector3 origin = small.position;
        // 오브젝트가 올라온다.
        while(progress < 1) {
            progress += 0.1f;
            small.position = Vector3.Lerp(origin, origin + Vector3.up * 2.2f, progress / 1);
            yield return new WaitForSeconds(0.05f);
        }
        float maxDistance = 200;
        progress = 0;
        while(progress < 1) {
            progress += 0.005f;
            // 돌리는 조건
            if(progress > 0.5f && !dopattern2PlusSmall[num]) {
                dopattern2PlusSmall[num] = true;
            }
            // 최종 속도는 0.05
            pattern2PlusSmallRotPower[num] = progress * 0.05f;
            pattern2PlusSmallDis[num] = maxDistance * (progress / 1);
            for(int i = 4 * num; i < (4 * num) + 4; i++) {
                Physics.Raycast(pattern2PlusLaser[i].position, pattern2PlusLaser[i].forward, out RaycastHit hit, pattern2PlusSmallDis[num], (1 << 3) + (1 << 6));
                if (hit.collider) {
                    pattern2PlusLaser[i].localScale = new Vector3(1, 1, hit.distance / 2);
                }
                else {
                    pattern2PlusLaser[i].localScale = new Vector3(1, 1, pattern2PlusSmallDis[num] / 2);
                }
            }
            yield return new WaitForSeconds(0.03f);
        }
    }
    IEnumerator DownAtk() {
        Vector3 origin;
        float progress;
        // 2초 추적
        for (int i = 0; i < 3; i++) {
            origin = transform.position;
            progress = 0;
            while (progress < 1) {
                progress += 0.016f;
                Debug.Log("TEST " + transform.position);
                transform.position = Vector3.Lerp(origin, new Vector3(target.position.x, origin.y, target.position.z), progress);
                // 66번 반복?
                yield return new WaitForSeconds(0.03f);
            }

            //찍기
            yield return new WaitForSeconds(0.3f);
            origin = transform.position;
            progress = 0;
            while (progress < 1) {
                progress += 0.1f;
                transform.position = Vector3.Lerp(origin, origin + Vector3.up * 1, progress / 1);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.2f);
            progress = 0;
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100, 1 << 6);
            while (progress < 1) {
                progress += 0.1f;
                if (hit.collider) {
                    transform.position = Vector3.Lerp(origin + Vector3.up * 1, origin + (Vector3.up * 1) + (Vector3.down * (hit.distance / 1)), progress / 1);
                }
                else {
                    transform.position = Vector3.Lerp(origin + Vector3.up * 1, origin + Vector3.down * 20f, progress / 1);
                }
                yield return new WaitForSeconds(0.01f);
            }
            Instantiate(fire, transform.position + Vector3.down * 0.05f, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            progress = 0;
            Vector3 vec = transform.position;
            while (progress < 1) {
                progress += 0.1f;
                transform.position = Vector3.Lerp(vec, origin, progress);
                yield return new WaitForSeconds(0.02f);
            }
        }
        yield return new WaitForSeconds(1f);
        if(phase == 3) {
            phase3Cnt++;
        }
    }
    public void HpDown(float dmg) {
        hp -= dmg;
        Debug.Log(hp);
    }
}
