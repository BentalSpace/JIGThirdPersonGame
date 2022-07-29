using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    float dmg;

    public bool isAtkTime;

    void Awake() {
        
    }
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && isAtkTime) {
            other.GetComponent<PlayerCtrl>().HpDown(dmg);
        }
    }
    void OnDisable() {
        
    }
}
