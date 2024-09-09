using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Part2_PlayerController : MonoBehaviour {
    [SerializeField] FastIKFabric[] iks;
    [SerializeField] Transform lFoot, rFoot, lTarget, rTarget, model;
    bool moving, canRotate = true;
    Vector2 moveDirection;
    Vector3 intendedDirection;
    Rigidbody rb;
    Transform cam;
    Animator anim;
    int currentWallContacts = 0;
    [SerializeField] int lookIndex;
    [SerializeField] float moveSpeed, rotateSpeed;

    private readonly int IDLE = Animator.StringToHash("Idle");
    private readonly int MOVE = Animator.StringToHash("Run");
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
    }
    void Start() {
        SetIKs(true);
    }

    void Update() {
        if (!moving) {
            if (Physics.Raycast(lFoot.position, Vector3.down, out RaycastHit hit)) {
                lTarget.position = hit.point;
            }

            if (Physics.Raycast(rFoot.position, Vector3.down, out RaycastHit hit2)) {
                rTarget.position = hit2.point;
            }
        }

        if (canRotate) Rotate(intendedDirection);
        
    }

    private void FixedUpdate() {
        intendedDirection =
            moveDirection.y * cam.forward +
            moveDirection.x * cam.right;
        intendedDirection.y = 0;
        rb.velocity = new Vector3(intendedDirection.x * moveSpeed, 0, intendedDirection.z * moveSpeed);
    }

    void SetIKs(bool active) {
        foreach (FastIKFabric ik in iks) {
            ik.enabled = active;
        }
    }

    public void Walk(InputAction.CallbackContext ctx) {
        print(ctx.ReadValue<Vector2>());
        if (ctx.phase == InputActionPhase.Performed) {
            moveDirection = ctx.ReadValue<Vector2>();
            moving = true;
            anim.CrossFade(MOVE, .2f);
            SetIKs(false);
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            moveDirection = Vector2.zero;
            moving = false;
            anim.CrossFade(IDLE, .2f);
            SetIKs(true);
        }
    }
    public void Run(InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Performed) {
            moveSpeed = 6;
            anim.speed = 1.3f;
        } else if (ctx.phase == InputActionPhase.Canceled) {
            moveSpeed = 3;
            anim.speed = 1;
        }
    }
    public void Crouch(InputAction.CallbackContext ctx) {
        if (ctx.phase == InputActionPhase.Performed) {
            model.DOLocalMoveY(-.23f, .2f);
        } else if (ctx.phase == InputActionPhase.Canceled) {
            model.DOLocalMoveY(0f, .2f);

        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Wall")) return;
        currentWallContacts++;
        canRotate = false;
    }

    private void OnCollisionExit(Collision collision) {
        if (!collision.gameObject.CompareTag("Wall")) return;
        currentWallContacts--;
        if (currentWallContacts == lookIndex) {
            canRotate = true;
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (!collision.gameObject.CompareTag("Wall")) return;
        Rotate(collision.GetContact(0).normal);
    }

    void Rotate(Vector3 direction) {
        Quaternion playerRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        if (direction != Vector3.zero) {
            targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(playerRotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
