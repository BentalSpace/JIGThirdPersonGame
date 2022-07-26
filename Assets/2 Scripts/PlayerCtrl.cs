using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    Transform cam;
    [SerializeField]
    Transform groundCheck;

    float h, v;

    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float jumpPower;

    float applySpeed;

    bool isRun;
    bool isDJump;
    bool isGround;

    [SerializeField, Tooltip("추가적인 중력")]
    float plusGravity;

    Rigidbody rigid;
    Animator anim;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        cam = Camera.main.transform;
        applySpeed = walkSpeed;
    }
    void Update() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Run();
        Jump();
        DoubleJump();
        Debug.Log(rigid.velocity);
    }
    void FixedUpdate() {
        Move();
        int layerMask = 1 << 3;
        layerMask = ~layerMask;
        RaycastHit hit;
        isGround = Physics.SphereCast(groundCheck.position, 0.2f, Vector3.down, out hit, 0.1f, layerMask);
        anim.SetBool("isGround", isGround);
        if (isDJump) {
            isDJump = !isGround;
        }
        if(!isGround)
            rigid.AddForce(Vector3.down * plusGravity);
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
        if (Input.GetButtonDown("Run")) {
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
    void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        int layerMask = 1 << 3;
        layerMask = ~layerMask;
        RaycastHit hit;
        bool test = Physics.SphereCast(groundCheck.position, 0.2f, Vector3.down, out hit, 0.1f, layerMask);
        if (!isGround) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(groundCheck.position, Vector3.down * 0.1f);
        }
        if (isGround) {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(groundCheck.position, Vector3.down * hit.distance);
            Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * hit.distance, 0.2f);
        }
    }
}
