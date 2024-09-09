using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Part2PlayerController : MonoBehaviour {
    Rigidbody rb;
    Collider col;
    [SerializeField] float gravity, moveSpeed, acceleration, rotateSpeed, jumpSpeed;
    [SerializeField] LayerMask groundLayer, climbableLayer;
    [SerializeField] Transform model;
    [SerializeField] Vector3 offset;
    [SerializeField] Animator anim;
    Transform cam;
    bool grounded, climbing, trackingPlayer = true;
    Vector3 input, direction, camVel;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        cam = Camera.main.transform;
    }

    private void Start() {
        cam.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    void Update() {
        CalculateInputAndDirection();
        CheckJump();
        ApplyAnims();
    }

    private void FixedUpdate() {
        CheckGrounded();
        if (climbing) {
            CheckClimbing();
            return;
        }
        CheckClimbables();
        Move();
        AddGravity();
        HandleRotation();
    }

    private void LateUpdate() {
        if (trackingPlayer) cam.position = Vector3.SmoothDamp(cam.position, transform.position + offset, ref camVel, .3f);
    }
    void CheckClimbing() {
        rb.velocity = Vector3.up * .7f + transform.forward * .2f;
        Ray bottomRay = new Ray(col.bounds.center + Vector3.down * col.bounds.extents.y, transform.forward);
        if (!Physics.Raycast(bottomRay, .4f, climbableLayer)) {
            climbing = false;
            rb.velocity = Vector3.zero;
        }
    }
    void CheckClimbables() {
        if (grounded) return;
        Ray bottomRay = new Ray(col.bounds.center, transform.forward);
        if (Physics.Raycast(bottomRay, .2f, climbableLayer)) {
            anim.SetTrigger("Climb");
            climbing = true;
            rb.velocity = Vector3.zero;
        }
    }

    void CalculateInputAndDirection() {
        input = (Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical")).normalized;

        Vector3 camForward = input.z * cam.forward;
        Vector3 camRight = input.x * cam.right;
        direction = camForward + camRight;
        direction = new Vector3(direction.x, 0, direction.z);
    }
    void Move() {
        Vector3 maxVelocity = direction * moveSpeed;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        Vector3 newPosition = Vector3.Lerp(rb.velocity, maxVelocity, maxSpeedChange);

        rb.velocity += newPosition - rb.velocity;
    }

    void CheckJump() {
        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            Ray bottomRay = new Ray(col.bounds.center + Vector3.down * (col.bounds.extents.y - .2f), transform.forward);
            Ray topRay = new Ray(col.bounds.center + (col.bounds.extents.y * Vector3.up), transform.forward);
            if (Physics.Raycast(bottomRay, .4f, climbableLayer) && !Physics.Raycast(topRay, .4f, climbableLayer)) {
                anim.SetTrigger("Climb");
                climbing = true;
            } else {
                rb.velocity += Vector3.up * jumpSpeed;
            }
        }
    }

    void AddGravity() {
        if (!grounded) rb.velocity += Vector3.down * gravity;
    }

    void CheckGrounded() {
        Ray ray = new Ray(col.bounds.center, Vector3.down);
        if (Physics.Raycast(ray, col.bounds.extents.y + .1f, groundLayer)) {
            grounded = true;
        } else {
            grounded = false;
        }
    }

    private void HandleRotation() {
        Quaternion playerRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;

        if (input != Vector3.zero) {
            targetRotation = Quaternion.LookRotation(direction);
        }

        if (input.sqrMagnitude != 0) {
            transform.rotation = Quaternion.Slerp(playerRotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

    }

    void ApplyAnims() {
        anim.SetFloat("Input", input.magnitude);
        anim.SetFloat("VVel", rb.velocity.y);
        anim.SetBool("Grounded", grounded);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Intersection")) {
            trackingPlayer = false;
            switch (other.name) {
                case "1":
                    cam.DOMove(new Vector3(-2, 7.1f, -10.25f), 2);
                    cam.DORotate(new Vector3(38.5f, 209.5f, 0), 2);
                    break;
                case "2":
                    cam.DOMove(new Vector3(-14.5f, 5.5f, -19), 2);
                    cam.DORotate(new Vector3(32.12f, 0, 0), 2);
                    break;
                default: break;
            }
        }
    }
}
