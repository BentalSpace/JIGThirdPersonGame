using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject m_missilePrefab; // �̻��� ������.
    public GameObject m_target; // ���� ����.

    [Header("�̻��� ��� ����")]
    public float m_speed = 2; // �̻��� �ӵ�.
    [Space(10f)]
    public float m_distanceFromStart = 6.0f; // ���� ������ �������� �󸶳� ������.
    public float m_distanceFromEnd = 3.0f; // ���� ������ �������� �󸶳� ������.
    [Space(10f)]
    public int m_shotCount = 12; // �� �� �� �߻��Ұ���.
    [Range(0, 1)] public float m_interval = 0.15f;
    public int m_shotCountEveryInterval = 2; // �ѹ��� �� ���� �߻��Ұ���.

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
         * ������Ʈ 2�� ��ġ
         * 1���� ��� Ÿ���� lookat���� �Ĵٺ��� ������Ʈ
         * 2���� 1���� rotation���� ���󰡴� ������Ʈ
         * laserDoublePos�� 1�� | laserPos�� 2��
         * GameObject laser�� �������� ����� ������Ʈ
         */
        Vector3 dir = new Vector3(target.position.x, 66.7f, target.position.z);
        // 
        laserDoublePos.LookAt(target);

        // �������� ��ġ�� �÷��̾ ���� �̵��ϵ��� ������.
        laserPos.localRotation = Quaternion.Lerp(laserPos.localRotation, laserDoublePos.localRotation, Time.deltaTime * 15f);

        Physics.Raycast(laserDoublePos.position, laserDoublePos.forward, out RaycastHit hit, 300);
        Debug.DrawRay(laserDoublePos.position, laserDoublePos.forward * 300, Color.red);
        //�������� ���̸� ���̸� ���� �Ÿ���ŭ �ø�. 
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
