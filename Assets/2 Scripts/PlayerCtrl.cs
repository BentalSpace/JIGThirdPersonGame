using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    GameObject gameOverPanel;
    float h, v;

    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float jumpPower;
    float maxHp;
    float curHp;

    float applySpeed;

    bool isRun;
    bool isAtk;
    bool isRotAtk;
    bool isDJump;
    bool isGround;

    public static bool dontCtrl;
    public static bool isOneTime;

    Image hpBar;

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
        maxHp = 100;
        curHp = maxHp;
        dontCtrl = false;

        hpBar = GameObject.Find("HPBarFill").GetComponent<Image>();
        gameOverPanel = GameObject.Find("GameOverPanel");
    }
    void Start() {
        gameOverPanel.SetActive(false);
    }
    void Update() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if(isAtk || dontCtrl) {
            isRun = false;
            anim.SetBool("isRun", false);
            applySpeed = walkSpeed;
        }
        if (dontCtrl && isOneTime) {
            // 공격 모두 제거
            isOneTime = false;
            StopAllCoroutines();
            anim.SetTrigger("AtkEnd");
            isAtk = false;
            isRotAtk = false;
            isRun = false;
            anim.SetBool("isMove", false);
            anim.SetBool("isRun", false);
            applySpeed = walkSpeed;

            Invoke("ResetTrigger", 0.1f);
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
    void LateUpdate() {
        hpBar.fillAmount = curHp / maxHp;
    }
    void ResetTrigger() {
        anim.ResetTrigger("AtkEnd");
        anim.ResetTrigger("FirstAtk");
        anim.ResetTrigger("SecondAtk");
        anim.ResetTrigger("StabAtk");
        anim.ResetTrigger("RotAtk");
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
            Vector3 lookForward = new Vector3(cam.forward.x, 0f, cam.forward.z);
            Vector3 lookRight = new Vector3(cam.right.x, 0f, cam.right.z);
            Vector3 moveDir = (lookForward * moveInput.y + lookRight * moveInput.x).normalized;
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
        yield return new WaitForSeconds(0.13f);
        normalAtkCol.SetActive(true);
        normalAtkCol.GetComponent<Attack>().dmg = 151f;
        isAtk = true;
        float time = 0;
        float maxTime = 0.3f;
        yield return new WaitForSeconds(0.13f);
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
        yield return new WaitForSeconds(0.13f);
        normalAtkCol.SetActive(true);
        normalAtkCol.GetComponent<Attack>().dmg = 2f;
        isAtk = true;
        float time = 0;
        float maxTime = 0.25f;
        yield return new WaitForSeconds(0.2f);
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
        yield return new WaitForSeconds(0.13f);
        stabAtkCol.SetActive(true);
        stabAtkCol.GetComponent<Attack>().dmg = 4f;
        isAtk = true;
        yield return new WaitForSeconds(0.23f);
        stabAtkCol.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        isAtk = false;
        anim.SetTrigger("AtkEnd");
    }

    // 회전 공격
    IEnumerator RotAttack() {
        anim.SetTrigger("RotAtk");
        isAtk = false;
        isRotAtk = true;
        rotAtkCol.SetActive(true);
        rotAtkCol.GetComponent<Attack>().dmg = 2f;
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
    public void HpDown(float dmg) {
        curHp -= dmg;
        if(curHp <= 0) {
            anim.SetTrigger("Die");
            dontCtrl = true;
            CamCtrl.dontCtrl = true;
            Invoke("GameOver", 1f);
        }
    }
    void GameOver() {
        gameOverPanel.SetActive(true);
        CamCtrl.dontCtrl = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
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
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(transform.position, 15);
    }
}
