using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
    [SerializeField] Transform target;
    [SerializeField] bool controllable;
    NavMeshAgent agent;
    Animator animator;
    bool isWalking, isAttacking;
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isWalking = true;
    }

    void Update() {
        if (isAttacking) {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.position, 1 * Time.deltaTime, 0));
            animator.SetTrigger("ATTACK");
            return;
        }
        if (controllable) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    SetWalk(true);
                    agent.destination = hit.point;
                }
            }
        } else {
            SetWalk(true);
            agent.destination = target.position;
        }
        
        if (transform.position == agent.destination && isWalking) SetWalk(false);

    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            isWalking = false;

            other.GetComponent<Animator>().SetTrigger("ATTACK");
            animator.SetTrigger("ATTACK");
            isAttacking = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Enemy")) {
            isWalking = true;
            other.GetComponent<Animator>().SetTrigger("IDLE");
            animator.SetTrigger("WALK");
            isAttacking = false;
        }
    }

    void SetWalk(bool walking) {
        if (!walking) print(transform.position - agent.destination);
        isWalking = walking;
        animator.SetTrigger(isWalking ? "WALK" : "IDLE");
    }
}
