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

    List<Vector3> atkPos = new List<Vector3>();

    Rigidbody rigid;
    void Awake() {
        rigid = GetComponent<Rigidbody>();

        target = GameObject.Find("Player").transform;
    }
    void Start() {
        // 패턴1 사용 틀
        //RaycastHit hit;
        //Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        //for (int i = 0; i < 10; i++) {
        //    Vector3 randPos = hit.point + (Vector3.right * Random.Range(-10f, 10f)) + (Vector3.forward * Random.Range(-10f,10f));
        //    GameObject dangerZone = Instantiate(dangerZonePrefab, randPos + Vector3.up * 0.01f, Quaternion.identity);
        //    //atkPos.Add(randPos);
        //    StartCoroutine(Pattern1(randPos, dangerZone));
        //}

        // 패턴 2
        RaycastHit hit;
        Physics.Raycast(target.position + Vector3.up * 30, Vector3.down, out hit, 200, 1 << 6);
        GameObject dangerZone = Instantiate(dangerZonePrefab, hit.point + Vector3.up * 0.01f, Quaternion.identity);
        dangerZone.transform.localScale = Vector3.one * 1.4f;

        StartCoroutine(Pattern2(dangerZone));
    }
    void Update() {

        Debug.DrawRay(target.position + Vector3.up * 100, Vector3.down * 500, Color.red);
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
            while(Physics.SphereCast(transform.position, 0.3f, Vector3.down, out RaycastHit hit, 0.1f)) {

            }

            yield return new WaitForSeconds(1f);
            rigid.AddForce(Vector3.up * 50, ForceMode.Impulse);
        }
        // dangerZone scale => 1.4f
    }
}
