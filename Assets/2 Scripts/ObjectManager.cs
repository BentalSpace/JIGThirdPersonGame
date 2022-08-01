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

    Queue<GameObject> dangerZone;
    Queue<GameObject> pillar;
    Queue<GameObject> pillarPlus;
    Queue<GameObject> pattern3;
    Queue<GameObject> pattern3Plus;
    Queue<GameObject> explosionEffect;

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
    }
    void QueueReset() {
        dangerZone = new Queue<GameObject>();
        pillar = new Queue<GameObject>();
        pillarPlus = new Queue<GameObject>();
        pattern3 = new Queue<GameObject>();
        pattern3Plus = new Queue<GameObject>();
        explosionEffect = new Queue<GameObject>();
    }
    //������Ʈ�� ����� �Լ�(CreateNewObject())�� ȣ���ؼ� ť�� ����ִ� �Լ�.
    void Initialize(Queue<GameObject> obj, int cnt, string name) {
        GameObject prefab = SearchPrefab(name);
        for (int i = 0; i < cnt; i++) {
            obj.Enqueue(CreateNewObject(prefab));
        }
    }

    // ������Ʈ�� ����� �Լ�.
    GameObject CreateNewObject(GameObject prefab) {
        GameObject newObj = Instantiate(prefab);

        newObj.SetActive(false);

        return newObj;
    }

    // ������ ���� ������Ʈ�� queue���� ã�Ƽ� ������ �Լ�
    public GameObject GetObject(string name) {
        Queue<GameObject> targetPool = null;
        // ť�� ã�´�.
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
            default:
                Debug.Log("GetObject not find name Error");
                return null;
        }
        // ã�� ť�� ������Ʈ�� �����ִٸ� ������.
        if (targetPool.Count > 0) {
            GameObject obj = targetPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        // ã�� ť�� ������Ʈ�� ���ٸ� ���� ���� ������.
        else {
            GameObject prefab = SearchPrefab(name);
            GameObject newObj = CreateNewObject(prefab);
            newObj.SetActive(true);
            return newObj;
        }
    }

    // ������ ���� ������Ʈ�� �����޴� �Լ�
    public void ReturnObject(GameObject obj, string name) {
        if (!obj.activeSelf) {
            //�̹� ������Ʈ�� ���ƿ� �ִٸ�
            return;
        }
        Queue<GameObject> targetPool = SearchQueue(name);

        targetPool.Enqueue(obj);
        obj.transform.position = Vector2.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(false);
    }

    // ���ϴ� �������� ã�Ƽ� �����ִ� �Լ�
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
            default:
                Debug.Log("SearchPrefab not find name Error");
                break;
        }
        return returnObj;
    }

    // ť�� ã�� �Լ�
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
            default:
                Debug.Log("SearchQueue not find name Error");
                return null;
        }
        return returnQueue;
    }
}
