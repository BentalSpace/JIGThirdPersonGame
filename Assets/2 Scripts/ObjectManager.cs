using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    [SerializeField]
    GameObject dangerZonePrefab;
    [SerializeField]
    GameObject pillarPrefab;
    [SerializeField]
    GameObject pillarPlusPrefab;
    [SerializeField]
    GameObject pattern3Prefab;
    [SerializeField]
    GameObject pattern3PlusPrefab;
    [SerializeField]
    GameObject explosionEffectPrefab;
    [SerializeField]
    GameObject st2Pattern2Prefab;
    [SerializeField]
    GameObject st2Pattern3MissilePrefab;

    Queue<GameObject> dangerZone;
    Queue<GameObject> pillar;
    Queue<GameObject> pillarPlus;
    Queue<GameObject> pattern3;
    Queue<GameObject> pattern3Plus;
    Queue<GameObject> explosionEffect;
    Queue<GameObject> st2Pattern2;
    Queue<GameObject> st2Pattern3Missile;

    void Awake() {
        instance = this;

        QueueReset();

        if (dangerZonePrefab) {
            Initialize(dangerZone, 30, "dangerZone");
        }
        if (pillarPrefab) {
            Initialize(pillar, 30, "pillar");
        }
        if (pillarPlusPrefab) {
            Initialize(pillarPlus, 200, "pillarPlus");
        }
        if (pattern3Prefab) {
            Initialize(pattern3, 5, "pattern3");
        }
        if (pattern3PlusPrefab) {
            Initialize(pattern3Plus, 5, "pattern3Plus");
        }
        if (explosionEffectPrefab) {
            Initialize(explosionEffect, 100, "explosion");
        }
        if (st2Pattern2Prefab) {
            Initialize(st2Pattern2, 10, "st2Pattern2");
        }
        if (st2Pattern3MissilePrefab) {
            Initialize(st2Pattern3Missile, 10, "st2Pattern3");
        }
    }
    void QueueReset() {
        dangerZone = new Queue<GameObject>();
        pillar = new Queue<GameObject>();
        pillarPlus = new Queue<GameObject>();
        pattern3 = new Queue<GameObject>();
        pattern3Plus = new Queue<GameObject>();
        explosionEffect = new Queue<GameObject>();
        st2Pattern2 = new Queue<GameObject>();
        st2Pattern3Missile = new Queue<GameObject>();
    }
    //오브젝트를 만드는 함수(CreateNewObject())를 호출해서 큐에 집어넣는 함수.
    void Initialize(Queue<GameObject> obj, int cnt, string name) {
        GameObject prefab = SearchPrefab(name);
        for (int i = 0; i < cnt; i++) {
            obj.Enqueue(CreateNewObject(prefab));
        }
    }

    // 오브젝트를 만드는 함수.
    GameObject CreateNewObject(GameObject prefab) {
        GameObject newObj = Instantiate(prefab);

        newObj.SetActive(false);

        return newObj;
    }

    // 밖으로 보낼 오브젝트를 queue에서 찾아서 보내는 함수
    public GameObject GetObject(string name) {
        Queue<GameObject> targetPool = null;
        // 큐를 찾는다.
        switch (name) {
            case "dangerZone":
                targetPool = dangerZone;
                break;
            case "pillar":
                targetPool = pillar;
                break;
            case "pillarPlus":
                targetPool = pillarPlus;
                break;
            case "pattern3":
                targetPool = pattern3;
                break;
            case "pattern3Plus":
                targetPool = pattern3Plus;
                break;
            case "explosion":
                targetPool = explosionEffect;
                break;
            case "st2Pattern2":
                targetPool = st2Pattern2;
                break;
            case "st2Pattern3":
                targetPool = st2Pattern3Missile;
                break;
            default:
                Debug.Log("GetObject not find name Error");
                return null;
        }
        // 찾은 큐에 오브젝트가 남아있다면 보내줌.
        if (targetPool.Count > 0) {
            GameObject obj = targetPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        // 찾은 큐에 오브젝트가 없다면 새로 만들어서 보내줌.
        else {
            GameObject prefab = SearchPrefab(name);
            GameObject newObj = CreateNewObject(prefab);
            newObj.SetActive(true);
            return newObj;
        }
    }

    // 밖으로 보낸 오브젝트를 돌려받는 함수
    public void ReturnObject(GameObject obj, string name) {
        if (!obj.activeSelf) {
            //이미 오브젝트가 돌아와 있다면
            return;
        }
        Queue<GameObject> targetPool = SearchQueue(name);

        targetPool.Enqueue(obj);
        obj.transform.position = Vector2.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(false);
    }

    // 원하는 프리팹을 찾아서 보내주는 함수
    GameObject SearchPrefab(string name) {
        GameObject returnObj = null;
        switch (name) {
            case "dangerZone":
                returnObj = dangerZonePrefab;
                break;
            case "pillar":
                returnObj = pillarPrefab;
                break;
            case "pillarPlus":
                returnObj = pillarPlusPrefab;
                break;
            case "pattern3":
                returnObj = pattern3Prefab;
                break;
            case "pattern3Plus":
                returnObj = pattern3PlusPrefab;
                break;
            case "explosion":
                returnObj = explosionEffectPrefab;
                break;
            case "st2Pattern2":
                returnObj = st2Pattern2Prefab;
                break;
            case "st2Pattern3":
                returnObj = st2Pattern3MissilePrefab;
                break;
            default:
                Debug.Log("SearchPrefab not find name Error");
                break;
        }
        return returnObj;
    }

    // 큐를 찾는 함수
    Queue<GameObject> SearchQueue(string name) {
        Queue<GameObject> returnQueue = null;
        switch (name) {
            case "dangerZone":
                returnQueue = dangerZone;
                break;
            case "pillar":
                returnQueue = pillar;
                break;
            case "pillarPlus":
                returnQueue = pillarPlus;
                break;
            case "pattern3":
                returnQueue = pattern3;
                break;
            case "pattern3Plus":
                returnQueue = pattern3Plus;
                break;
            case "explosion":
                returnQueue = explosionEffect;
                break;
            case "st2Pattern2":
                returnQueue = st2Pattern2;
                break;
            case "st2Pattern3":
                returnQueue = st2Pattern3Missile;
                break;
            default:
                Debug.Log("SearchQueue not find name Error");
                return null;
        }
        return returnQueue;
    }
}
