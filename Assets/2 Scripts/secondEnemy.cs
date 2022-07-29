using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondEnemy : MonoBehaviour
{
    Transform target;
    [SerializeField]
    Transform laserPos;

    [SerializeField]
    GameObject laser;
    void Awake() {
        target = GameObject.Find("Player").transform;
    }
    void Start() {
        // ∆–≈œ1
        RaycastHit hit;
        Vector3 dir = target.position - laserPos.position;
        Physics.Raycast(laserPos.position, dir, out hit, 100, 1 << 6);
        Debug.DrawRay(laserPos.position, dir * hit.distance, Color.red);
        laser.transform.position = laserPos.position;
        laser.transform.localScale = new Vector3(1f, hit.distance, 1f);
        laser.transform.localRotation = Quaternion.Euler(dir);


    }
    void Update() {
        RaycastHit hit;
        Vector3 dir = target.position - laserPos.position;
        if (Physics.Raycast(laserPos.position, dir, out hit, 100, 1 << 3)) {
            Debug.DrawRay(laserPos.position, dir * hit.distance, Color.red);
        }
    }
    IEnumerator Pattern1() {
        yield return null;
    }
}
