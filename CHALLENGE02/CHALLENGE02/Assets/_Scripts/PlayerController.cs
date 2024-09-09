using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {
    Rigidbody rb;
    CapsuleCollider col;
    Transform cam;
    [SerializeField] Transform playerModel;
    [SerializeField] Animator anim;
    Vector2 input;
    Vector3 desiredVelocity, velocity, contactNormal, inputAccordingToCam;
    [SerializeField] float maxSpeed, acceleration, maxGroundAngle, turnSpeed;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] CinemachineFreeLook flCam;
    [SerializeField] GameObject punchHitbox;
    bool canMove;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        cam = Camera.main.transform;

        Time.timeScale = 1.0f;
    }
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        for (int i = 0; i < 2; i++) {
            CinemachineOrbitalTransposer tempTransposer = flCam.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>();
            tempTransposer.m_XDamping = 0;
            tempTransposer.m_YDamping = 0;
            tempTransposer.m_ZDamping = 0;
        }
        canMove = true;
    }

    void Update() {
        CalculateInput();
        HandleRotation();
        CheckPunch();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Main");
        }
    }
    private void FixedUpdate() {
        if (!canMove) return;
        ApplyVelocity();
        CheckNormal();
    }

    void CalculateInput() {
        input = Input.GetAxisRaw("Horizontal") * Vector2.right + Input.GetAxisRaw("Vertical") * Vector2.up;
        input = Vector2.ClampMagnitude(input, 1f);

        inputAccordingToCam = (input.y * cam.forward) + (input.x * cam.right);
        inputAccordingToCam.y = 0;
        inputAccordingToCam = inputAccordingToCam.normalized;
        desiredVelocity = inputAccordingToCam * maxSpeed;

        anim.SetBool("Walking", input.magnitude != 0);
    }

    void ApplyVelocity() {
        velocity = rb.velocity;

        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float maxSpeedChange = acceleration * Time.fixedDeltaTime;
        Vector3 currentVelocity = new Vector3(currentX, 0, currentZ);
        Vector3 newPos = Vector3.Lerp(currentVelocity, desiredVelocity, maxSpeedChange);

        velocity += xAxis * (newPos.x - currentX) + zAxis * (newPos.z - currentZ);
        rb.velocity = velocity;
    }

    Vector3 ProjectOnContactPlane(Vector3 vector) {
        return vector - (contactNormal * Vector3.Dot(vector, contactNormal));
    }

    void CheckNormal() {
        if (Physics.Raycast(col.bounds.center /*+ Vector3.forward * col.bounds.extents.z*/, Vector3.down, out RaycastHit hit, col.bounds.extents.y + .2f, groundLayer)) {
            contactNormal = hit.normal;

        } else { contactNormal = Vector3.up; }
    }

    void HandleRotation() {
        if (!canMove) return;
        Quaternion playerRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.identity;
        Vector3 lookAtVector = Vector3.zero;
        if (rb.velocity.magnitude > 0) {
            lookAtVector.x = rb.velocity.normalized.x;
            lookAtVector.z = rb.velocity.normalized.z;
        }

        //The rotation the player wants to end at (because they'll rotate over time)
        if (rb.velocity.sqrMagnitude != 0 && lookAtVector != Vector3.zero) {
            targetRotation = Quaternion.LookRotation(lookAtVector);
        }

        if (input.sqrMagnitude != 0) {
            transform.rotation = Quaternion.Slerp(playerRotation, targetRotation, turnSpeed * Time.deltaTime);
        }

    }

    void CheckPunch() {
        if (!canMove) return;
        if (Input.GetMouseButtonDown(0)) {
            anim.SetTrigger("Punch");
            StartCoroutine(Punch());
        }
    }

    IEnumerator Punch() {
        canMove = false;
        rb.velocity *= .6f;
        punchHitbox.SetActive(true);

        yield return new WaitForSeconds(.5f);

        punchHitbox.SetActive(false);
        canMove = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            Die();
        }
    }

    void Die() {
        Time.timeScale = .55f;
        rb.AddForce(Vector3.up * 6, ForceMode.Impulse);
        canMove = false;
        anim.SetTrigger("Die");
    }
}
