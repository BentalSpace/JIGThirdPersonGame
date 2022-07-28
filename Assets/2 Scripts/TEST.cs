using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    Rigidbody rigid;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
    }
    void Start() {
        rigid.AddForce(new Vector3(20, 10, 0), ForceMode.Impulse);
    }
}
