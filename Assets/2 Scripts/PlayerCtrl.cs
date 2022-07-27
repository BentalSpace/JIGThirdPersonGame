using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Transform cam;
    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    GameObject normalAtkCol;
    [SerializeField]
    GameObject stabAtkCol;
    [SerializeField]
    GameObject rotAtkCol;

    float h, v;

    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float jumpPower;

    float applySpeed;

    bool isRun;
    bool isAtk;
    bool isRotAtk;
    bool isDJump;
    bool isGround;

    public static bool dontCtrl;

    [SerializeField, Tooltip("추가적인 중력")]
    float plusGravity;

    Rigidbody rigid;
    Animator anim;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        cam = Camera.main.transform;
        applySpeed = walkSpeed;
    }
    void Update() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if(isAtk || dontCtrl) {
            isRun = false;
            anim.SetBool("isRun", false);
            applySpeed = walkSpeed;
        }

        if (!dontCtrl) {
            if (!isRotAtk) {
                Run();
            }
            if (!isAtk && !isRotAtk) {
                Jump();
            }
            DoubleJump();
            if (Input.GetMouseButtonDown(0) && isGround && !isAtk && !isRotAtk) {
                StartCoroutine(Attack());
            }
        }
    }
    void FixedUpdate() {
        if (!dontCtrl && !isAtk) {
            Move();
        }
        GroundCheck();

        // 중력 추가
        if (!isGround)
            rigid.AddForce(Vector3.down * plusGravity);
    }
    void GroundCheck() {
        int layerMask = 1 << 3;
        layerMask = ~layerMask;
        RaycastHit hit;
        isGround = Physics.SphereCast(groundCheck.position, 0.2f, Vector3.down, out hit, 0.1f, layerMask);
        anim.SetBool("isGround", isGround);
        if (isDJump) {
            isDJump = !isGround;
        }
    }
    void Move() {
        Vector2 moveInput = new Vector2(h, v);
        bool isMove = moveInput.magnitude != 0;

        anim.SetBool("isMove", isMove);
        if (isMove) {
            Vector3 lookForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
            Vector3 lookRight = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            transform.forward = moveDir;
            Vector3 moveVec = new Vector3(moveDir.x, 0, moveDir.z) * applySpeed;
            moveVec.y = rigid.velocity.y;
            rigid.velocity = moveVec;
            //transform.position += moveDir * Time.deltaTime * applySpeed;
        }
    }
    void Run() {
        if (Input.GetButton("Run") && !isRun) {
            isRun = true;
            anim.SetBool("isRun", true);
            applySpeed = runSpeed;
        }
        else if (Input.GetButtonUp("Run")) {
            isRun = false;
            anim.SetBool("isRun", false);
            applySpeed = walkSpeed;
        }
    }
    void Jump() {
        if (Input.GetButtonDown("Jump") && isGround) {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetTrigger("Jump");
        }
    }
    void DoubleJump() {
        if (Input.GetButtonDown("Jump") && !isDJump && !isGround) {
            isDJump = true;
            rigid.velocity = Vector3.zero;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetTrigger("DJump");
        }
    }

    // 첫번째 베기 공격
    IEnumerator Attack() {
        anim.SetTrigger("FirstAtk");
        normalAtkCol.SetActive(true);
        normalAtkCol.GetComponent<Attack>().dmg = 1.5f;
        isAtk = true;
        float time = 0;
        float maxTime = 0.4f;
        yield return new WaitForSeconds(0.15f);
        normalAtkCol.SetActive(false);
        while(maxTime > time) {
            time += Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(SecondAttack());
                yield break;
            }
            if (Input.GetMouseButtonDown(1)) {
                StartCoroutine(StabAttack());
                yield break;
            }
            yield return null;
        }
        isAtk = false;
        anim.SetTrigger("AtkEnd");
    }

    // 두번째 베기 공격
    IEnumerator SecondAttack() {
        anim.SetTrigger("SecondAtk");
        normalAtkCol.SetActive(true);
        normalAtkCol.GetComponent<Attack>().dmg = 2f;
        isAtk = true;
        float time = 0;
        float maxTime = 0.4f;
        yield return new WaitForSeconds(0.15f);
        normalAtkCol.SetActive(false);
        while(maxTime > time) {
            time += Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) {
                StartCoroutine(Attack());
                yield break;
            }
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) && Input.GetMouseButton(1)) {
                StartCoroutine(RotAttack());
                yield break;
            }
            yield return null;
        }
        isAtk = false;
        anim.SetTrigger("AtkEnd");
    }

    // 찌르기 공격
    IEnumerator StabAttack() {
        anim.SetTrigger("StabAtk");
        stabAtkCol.SetActive(true);
        stabAtkCol.GetComponent<Attack>().dmg = 4f;
        isAtk = true;
        yield return new WaitForSeconds(0.2f);
        stabAtkCol.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        isAtk = false;
        anim.SetTrigger("AtkEnd");
    }

    // 회전 공격
    IEnumerator RotAttack() {
        anim.SetTrigger("RotAtk");
        isAtk = false;
        isRotAtk = true;
        rotAtkCol.SetActive(true);
        rotAtkCol.GetComponent<Attack>().dmg = 1.2f;
        applySpeed = 3;
        float time = 0;
        int cnt = 0;
        int maxCnt = 15;

        while (maxCnt > cnt) {
            time += Time.deltaTime;
            if (!Input.GetMouseButton(1)) {
                // 공격 종료
                yield return new WaitForSeconds(0.15f);
                isRotAtk = false;
                if (isRun)
                    applySpeed = runSpeed;
                else
                    applySpeed = walkSpeed;
                rotAtkCol.SetActive(false);
                anim.SetTrigger("AtkEnd");
                yield break;
            }
            if(time > 0.2f) {
                rotAtkCol.SetActive(false);
                rotAtkCol.SetActive(true);
                time = 0;
                cnt++;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        isRotAtk = false;
        if (isRun)
            applySpeed = runSpeed;
        else
            applySpeed = walkSpeed;
        rotAtkCol.SetActive(false);
        anim.SetTrigger("AtkEnd");
    }
    void OnDrawGizmos() {
        //int layerMask = 1 << 3;
        //layerMask = ~layerMask;
        //RaycastHit hit;
        //bool test = Physics.SphereCast(groundCheck.position, 0.2f, Vector3.down, out hit, 0.1f, layerMask);
        //if (!isGround) {
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(groundCheck.position, Vector3.down * 0.1f);
        //}
        //if (isGround) {
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawRay(groundCheck.position, Vector3.down * hit.distance);
        //    Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * hit.distance, 0.2f);
        //}
    }
}
