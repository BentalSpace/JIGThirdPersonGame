using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstEnemy : MonoBehaviour
{
    Transform target;
    [SerializeField]
    GameObject dangerZonePrefab;
    [SerializeField]
    GameObject pillarPrefab;
    [SerializeField]
    GameObject pattern2Particle;
    [SerializeField]
    GameObject pattern3Prefab;

    public bool isGround;
    
    float hp;
    float maxHp;
    List<Vector3> atkPos = new List<Vector3>();

    Rigidbody rigid;
    void Awake() {
        rigid = GetComponent<Rigidbody>();

        target = GameObject.Find("Player").transform;
        maxHp = 100;
        hp = maxHp;
    }
    void Start() {
        // 패턴1 사용 틀
        //RaycastHit hit;
        //Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        //for (int i = 0; i < 10; i++) {
        //    Vector3 randPos = hit.point + (Vector3.right * Random.Range(-10f, 10f)) + (Vector3.forward * Random.Range(-10f,10f));
        //    GameObject dangerZone = Instantiate(dangerZonePrefab, randPos + Vector3.up * 0.01f, Quaternion.identity);
        //    dangerZone.transform.localScale = Vector3.one * 0.5f;
        //    //atkPos.Add(randPos);
        //    StartCoroutine(Pattern1(randPos, dangerZone));
        //}

        // 패턴 2
        //RaycastHit hit;
        //Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        //GameObject dangerZone = Instantiate(dangerZonePrefab, hit.point + Vector3.up * 0.01f, Quaternion.identity);
        //dangerZone.transform.localScale = Vector3.one * 1.4f;

        //StartCoroutine(Pattern2(dangerZone));

        // 패턴 3
        RaycastHit hit;
        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        GameObject dangerZone = Instantiate(dangerZonePrefab, hit.point + Vector3.up * 0.01f, Quaternion.identity);
        dangerZone.transform.localScale = Vector3.one * 0.7f;
        StartCoroutine(Pattern3(dangerZone));
    }
    void Update() {
        int layerMask = 1 << 6;
        isGround = Physics.SphereCast(transform.position + (Vector3.up * 0.6f), 0.4f, Vector3.down, out RaycastHit hit, 0.2f, layerMask);

        //Debug.DrawRay(target.position + Vector3.up * 100, Vector3.down * 500, Color.red);
    }
    IEnumerator Pattern1(Vector3 pos, GameObject zone) {
        // 하늘에서 쏘는 레이저
        float randTime = Random.Range(1.5f, 2f);
        yield return new WaitForSeconds(randTime);
        GameObject pillar = Instantiate(pillarPrefab, pos + Vector3.up * 10f, Quaternion.identity);
        pillar.transform.localScale = new Vector3(0.5f, 10, 0.5f);
        float progress = 0;
        float maxProgress = 1;
        while(progress < maxProgress) {
            progress += 0.1f;
            pillar.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 10, 0.5f), new Vector3(0.1f, 10, 0.1f), progress / maxProgress);
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.3f);
        progress = 0;
        while(progress < maxProgress) {
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
        Destroy(pillar);
        Destroy(zone);
    }
    IEnumerator Pattern2(GameObject zone) {
        for (int i = 0; i < 3; i++) {
            float progress = 0;
            while (progress < 1) {
                progress += 0.01f;
                zone.transform.position = new Vector3(target.position.x, zone.transform.position.y, target.position.z);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            transform.position = new Vector3(zone.transform.position.x, transform.position.y, zone.transform.position.z);
            rigid.AddForce(Vector3.down * 100, ForceMode.Impulse);
            int layerMask = 1 << 6;
            while(true) {
                RaycastHit hit;
                isGround = Physics.SphereCast(transform.position + (Vector3.up * 0.6f), 0.4f, Vector3.down, out hit, 0.2f, layerMask);
                Quaternion rot = new Quaternion();
                rot.eulerAngles = new Vector3(-90, 0, 0);
                if (isGround) {
                    Instantiate(pattern2Particle, hit.point, rot);
                    break;
                }
                else {

                }
                yield return null;
            }
            
            // 낙하한 몬스터와 플레이어의 거리가 짧다면, 플레이어는 약간의 스턴과 점프
            if(Vector3.Distance(target.position, transform.position) < 10) {
                PlayerCtrl.dontCtrl = true;
                target.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
                yield return new WaitForSeconds(0.5f);
                PlayerCtrl.dontCtrl = false;
                yield return new WaitForSeconds(0.5f);
            }
            else {
                yield return new WaitForSeconds(1f);
            }
            rigid.AddForce(Vector3.up * 50, ForceMode.Impulse);
        }
        // dangerZone scale => 1.4f
    }
    IEnumerator Pattern3(GameObject zone) {
        yield return new WaitForSeconds(0.5f);
        // 플레이어를 공격할 오브젝트 생성 및 아래로 내리는 연출
        GameObject item = Instantiate(pattern3Prefab, zone.transform.position + Vector3.up * 20, Quaternion.identity);
        Destroy(zone);
        float progress = 0;
        Vector3 itemVec = item.transform.position;
        while(progress < 1) {
            progress += 0.03f;
            item.transform.position = Vector3.Slerp(itemVec, itemVec + Vector3.down * 10, progress);
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(0.5f);
        // 날을 늘린다.
        progress = 0;
        Transform knife = item.transform.GetChild(1);
        while(progress < 1) {
            progress += 0.01f;
            knife.localScale = Vector3.Lerp(new Vector3(5, 0.2f, 3), new Vector3(100, 0.2f, 3), progress);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        // 날을 돌림
        float power = 0.05f;
        progress = 0;
        float time = 0;
        while(progress < 10) {
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
            if(progress > 8f) {
                if(time > 0.2f) {
                    if(power > 0.05f)
                        power -= 0.05f;
                    time = 0;
                }
            }
            yield return null;
        }

        // 날을 접음
        yield return new WaitForSeconds(0.5f);
        progress = 0;
        while (progress < 1) {
            progress += 0.01f;
            knife.localScale = Vector3.Lerp(new Vector3(100, 0.2f, 3), new Vector3(5, 0.2f, 3), progress);
            yield return null;
        }

        // 오브젝트를 올려보냄
        yield return new WaitForSeconds(0.3f);
        progress = 0;
        itemVec = item.transform.position;
        while (progress < 1) {
            progress += 0.03f;
            item.transform.position = Vector3.Slerp(itemVec, itemVec + Vector3.up * 20, progress);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(item);
    }

    public void HpDown(float dmg) {
        hp -= dmg;
        Debug.Log(hp);
    }
    void OnDrawGizmos() {
        // 땅 확인
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

        //// 반지름 확인
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
