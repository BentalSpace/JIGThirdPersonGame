using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondEnemy : MonoBehaviour
{
    Transform target;
    [SerializeField]
    Transform laserPos;
    [SerializeField]
    Transform laserDoublePos;
    [SerializeField]
    GameObject laser;

    // 패턴1 관리 변수
    public bool doPattern1;
    float pattern1EffectTime;
    float pattern1DmgTime;
    [SerializeField]
    GameObject pattern1PuzzleEffectPrefab;
    GameObject pattern1PuzzleEffect;

    // 패턴2 관리 변수
    public bool doPattern2;
    bool pattern2Ending;
    Transform pattern2Tr;
    public Transform[] pattern2Laser = new Transform[8];
    float pattern2Dis;
    float pattern2RotTime;
    public float pattern2RotPower;

    void Awake() {
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
    }
    void Start() {
        // 패턴1
        //StartCoroutine(Pattern1());

        StartCoroutine(Pattern2());
    }
    void Update() {
        // 패턴1
        // 시작할때 laser의 setactive를 true로 변경해줘야 함.
        //if (doPattern1 && !PlayerCtrl.dontCtrl) {
        //    pattern1EffectTime += Time.deltaTime;
        //    pattern1DmgTime += Time.deltaTime;
        //    // 플레이어의 몸통방향을 목표로. 그리고 점프를 해도 점프값은 신경쓰지 않도록.
        //    Vector3 dir = new Vector3(target.position.x, 66.7f, target.position.z);
        //    laserDoublePos.LookAt(dir);

        //    // 레이저의 위치가 플레이어를 향해 이동하듯이 움직임.
        //    laserPos.localRotation = Quaternion.Lerp(laserPos.localRotation, laserDoublePos.localRotation, Time.deltaTime * 15f);

        //    RaycastHit hit;
        //    // Player와 Ground 확인
        //    int layerMask = (1 << 3) + (1 << 6) + (1 << 9);
        //    // 천천히 이동중인 그 위치로 레이를 쏨.
        //    Physics.Raycast(laserPos.position, laserPos.forward, out hit, 300, layerMask);
        //    //레이저의 길이를 레이를 맞춘 거리만큼 늘림. 
        //    laser.transform.localScale = new Vector3(1f, hit.distance / 2, 1f);
        //    if (pattern1EffectTime > 0.1f) {
        //        pattern1EffectTime = 0;
        //        StartCoroutine(Pattern1(hit));
        //    }
        //    if(pattern1DmgTime > 0.1f) {
        //        if (hit.collider.CompareTag("Player")) {
        //            pattern1DmgTime = 0;
        //            hit.collider.GetComponent<PlayerCtrl>().HpDown(0.2f);
        //        }
        //        if (hit.collider.CompareTag("Puzzle")) {

        //            pattern1DmgTime = 0;
        //            doPattern1 = false;
        //            pattern1PuzzleEffect.transform.position = hit.collider.transform.position;
        //            pattern1PuzzleEffect.SetActive(true);
        //            laser.SetActive(false);
        //            StartCoroutine(Camera.main.GetComponent<Shake>().ShakeCamera(0.2f, 1f));
        //            hit.collider.GetComponent<Stage2Pattern1>().Hit();
        //        }
        //    }
        //}

        // 패턴 2
        if (doPattern2) {
            // 레이저에 적이 있다면, 길이 조절.
            pattern2RotTime += Time.deltaTime;
            foreach (Transform tr in pattern2Laser) {
                Physics.Raycast(tr.position, tr.forward, out RaycastHit hit, pattern2Dis, (1 << 3) + (1 << 6));
                if (hit.collider) {
                    tr.localScale = new Vector3(1, 1, hit.distance / 2);
                }
                else {
                    tr.localScale = new Vector3(1, 1, pattern2Dis / 2);
                }
            }
            // 0.05 -> 0.1로 증가
            pattern2Tr.Rotate(Vector3.up * pattern2RotPower);
            if(pattern2RotTime > 15 && !pattern2Ending) {
                // 패턴2 종료
                StartCoroutine(Pattern2End());
            }
        }
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
}
