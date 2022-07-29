using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOnOff : MonoBehaviour
{
    enum ColType { Capsule, Sphere}

    [SerializeField]
    ColType type;

    [SerializeField]
    float onOffTime;
    float curTime;

    CapsuleCollider capsule;
    SphereCollider sphere;

    void Awake() {
        switch (type) {
            case ColType.Capsule:
                capsule = GetComponent<CapsuleCollider>();
                break;
            case ColType.Sphere:
                sphere = GetComponent<SphereCollider>();
                break;
            default:
                break;
        }
    }

    
    void Update()
    {
        curTime += Time.deltaTime;

        switch (type) {
            case ColType.Capsule:
                if (!capsule.enabled)
                    capsule.enabled = true;
                break;
            case ColType.Sphere:
                if (!sphere.enabled)
                    sphere.enabled = true;
                break;
        }

        if(onOffTime < curTime) {
            switch (type) {
                case ColType.Capsule:
                    capsule.enabled = false;
                    break;
                case ColType.Sphere:
                    sphere.enabled = false;
                    break;
            }
            curTime = 0;
        }
    }
}
