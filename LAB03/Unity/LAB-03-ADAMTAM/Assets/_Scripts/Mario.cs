using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario : MonoBehaviour {
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 offset;
    Transform cam;
    Rigidbody rb;
    Vector3 vel;
    void Awake() {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
    }

    private void Start() {
        GameManager.Instance.mario = this;
    }

    void Update() {
        if (GameManager.Instance.CamFocus) cam.position = Vector3.SmoothDamp(cam.position, transform.position + offset, ref vel, .3f);

        if (!GameManager.Instance.CanMove) return;
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        rb.AddForce(input * moveSpeed, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hand")) {
            print("Damaged");
        }
    }
}
