using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    public Transform target;
    float xMove;
    float yMove;

    public static bool dontCtrl;
    public static bool dontLimit;

    [SerializeField, Tooltip("ī�޶�� Ÿ���� �Ÿ�")]
    float distance;

    [SerializeField, Tooltip("�¿�� ���� ���콺 �ΰ���")]
    float ySensitivity;
    [SerializeField, Tooltip("���Ʒ��� ���� ���콺 �ΰ���")]
    float xSensitivity;

    void Awake() {
        target = GameObject.Find("Player").transform;
        dontCtrl = false;
    }
    void Update() {
        if (!dontCtrl) {
            LookAround();
            camDistanceCtrl();
        }
    }
    void camDistanceCtrl() {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0) {
            // �� ��
            if (distance > 3) {
                distance -= Time.deltaTime * 100;
            }
            if (distance < 3) {
                distance = 3;
            }
        }
        else if (wheelInput < 0) {
            // �� �ٿ�
            if (distance < 15) {
                distance += Time.deltaTime * 100;
            }
            if (distance > 15) {
                distance = 15;
            }
        }
        Vector3 reserveDistance = new Vector3(0, 0, distance);

        //transform.position = target.transform.position - transform.rotation * reserveDistance + (Vector3.up * 1);
        Vector3 vec = target.transform.position - (transform.rotation * reserveDistance) + (Vector3.up * 1);
        transform.position = Vector3.Slerp(transform.position, vec, Time.deltaTime * 500);
    }
    void LookAround() {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = transform.rotation.eulerAngles;
        float x = camAngle.x - (mouseDelta.y * xSensitivity);

        if (!dontLimit) {
            if (x < 180f) {
                x = Mathf.Clamp(x, -1f, 70f);
            }
            else {
                x = Mathf.Clamp(x, 360f, 361f);
            }
        }

        transform.rotation = Quaternion.Euler(x, camAngle.y + (mouseDelta.x * ySensitivity), camAngle.z) ;
    }
}
