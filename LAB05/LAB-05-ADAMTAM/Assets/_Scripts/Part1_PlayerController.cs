using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;
public class Part1_PlayerController : MonoBehaviour {
    Animator anim;
    bool running, inRangeToIK = false;
    [SerializeField] int moveSpeed;
    [SerializeField] FastIKFabric[] iks;
    Transform cam;

    readonly int IDLE = Animator.StringToHash("Idle");
    readonly int RUN = Animator.StringToHash("Run");
    private void Awake() {
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
    }
    void Start() {
        SetIKs(false);
    }

    void Update() {
        cam.LookAt(transform);
        float input = Input.GetAxisRaw("Horizontal");
        if (input == 0) {
            if (running) anim.CrossFade(IDLE, .2f);
            running = false;
        } else {
            if (!running) anim.CrossFade(RUN, .2f);
            running = true;
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * input, Space.World);
            transform.rotation = Quaternion.Euler(Vector3.up * (input > 0 ? 0 : 180));
            if (inRangeToIK) SetIKs(input > 0 ? true : false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Door")) inRangeToIK = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Door")) inRangeToIK = false;
    }

    void SetIKs(bool active) {
        foreach (FastIKFabric ik in iks) {
            ik.enabled = active;
        }
    }
}
