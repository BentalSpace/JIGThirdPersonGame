using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject m_missilePrefab; // 미사일 프리팹.
    public GameObject m_target; // 도착 지점.

    [Header("미사일 기능 관련")]
    public float m_speed = 2; // 미사일 속도.
    [Space(10f)]
    public float m_distanceFromStart = 6.0f; // 시작 지점을 기준으로 얼마나 꺾일지.
    public float m_distanceFromEnd = 3.0f; // 도착 지점을 기준으로 얼마나 꺾일지.
    [Space(10f)]
    public int m_shotCount = 12; // 총 몇 개 발사할건지.
    [Range(0, 1)] public float m_interval = 0.15f;
    public int m_shotCountEveryInterval = 2; // 한번에 몇 개씩 발사할건지.

    public Transform target;
    public Transform laserPos;
    public Transform laserDoublePos;
    public GameObject laser;

    private void Update() {
        //if (Input.GetKeyDown(KeyCode.A)) {
        //    // Shot.
        //    StartCoroutine(CreateMissile());
        //}

        /* 
         * 오브젝트 2개 배치
         * 1번은 계속 타겟을 lookat으로 쳐다보는 오브젝트
         * 2번은 1번의 rotation값을 따라가는 오브젝트
         * laserDoublePos가 1번 | laserPos가 2번
         * GameObject laser는 레이저로 사용할 오브젝트
         */
        Vector3 dir = new Vector3(target.position.x, 66.7f, target.position.z);
        // 
        laserDoublePos.LookAt(target);

        // 레이저의 위치가 플레이어를 향해 이동하듯이 움직임.
        laserPos.localRotation = Quaternion.Lerp(laserPos.localRotation, laserDoublePos.localRotation, Time.deltaTime * 15f);

        Physics.Raycast(laserDoublePos.position, laserDoublePos.forward, out RaycastHit hit, 300);
        Debug.DrawRay(laserDoublePos.position, laserDoublePos.forward * 300, Color.red);
        //레이저의 길이를 레이를 맞춘 거리만큼 늘림. 
        laser.transform.localScale = new Vector3(1f, 1f, hit.distance / 2);
    }

    IEnumerator CreateMissile() {
        int _shotCount = m_shotCount;
        while (_shotCount > 0) {
            for (int i = 0; i < m_shotCountEveryInterval; i++) {
                if (_shotCount > 0) {
                    GameObject missile = Instantiate(m_missilePrefab);
                    missile.GetComponent<BezierMissile>().Init(this.gameObject.transform, m_target.transform, m_speed, m_distanceFromStart, m_distanceFromEnd);

                    _shotCount--;
                }
            }
            yield return new WaitForSeconds(m_interval);
        }
        yield return null;
    }
}
