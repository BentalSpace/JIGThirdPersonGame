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
    public static bool isSpecialProduction;

    [SerializeField, Tooltip("Ä«¸Þ¶ó¿Í Å¸°ÙÀÇ °Å¸®")]
    float distance;

    [SerializeField, Tooltip("ÁÂ¿ì·Î Èçµå´Â ¸¶¿ì½º ¹Î°¨µµ")]
    float ySensitivity;
    [SerializeField, Tooltip("À§¾Æ·¡·Î Èçµå´Â ¸¶¿ì½º ¹Î°¨µµ")]
    float xSensitivity;

    void Awake() {
        target = GameObject.Find("Player").transform;
        dontCtrl = false;

        isSpecialProduction = false;
        distance = 15;
    }
    void Update() {
        SpecialProduction();
        if (!dontCtrl) {
            LookAround();
            camDistanceCtrl();
        }
    }
    void camDistanceCtrl() {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0) {
            // ÈÙ ¾÷
            if (distance > 3) {
                distance -= Time.deltaTime * 100;
            }
            if (distance < 3) {
                distance = 3;
            }
        }
        else if (wheelInput < 0) {
            // ÈÙ ´Ù¿î
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
    public void ProductionReady() {
        dontCtrl = true;
        transform.localEulerAngles = new Vector3(20, transform.localEulerAngles.y, 0);
        distance = 5;
        isSpecialProduction = true;
    }
    void SpecialProduction() {
        if (!isSpecialProduction)
            return;
        transform.localEulerAngles = new Vector3(20, transform.localEulerAngles.y, 0);
        transform.RotateAround(target.position, Vector3.up, 10 * Time.deltaTime);
        distance = 5f;
        Vector3 reserveDistance = new Vector3(0, 0, distance);
        transform.position = target.transform.position - transform.rotation * reserveDistance + (Vector3.up * 1);
    }
}
