using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour {
    [SerializeField] PathManager pathManager;
    [SerializeField] Animator animator;

    readonly string WALKING = "isWalking";
    readonly string HIT = "Hit";

    List<Waypoint> path;
    Waypoint target;

    public float moveSpeed;
    public float rotateSpeed;
    bool isWalking;
    bool isObstructed;
    GameObject currentObstruction;
    bool legacy;
    int index;
    bool dead;
    bool aggro;

    void Start() {
        MouseCast.Instance.allControllers.Add(this);
        isWalking = false;
        animator.SetBool(WALKING, false);
        path = pathManager.GetPath();
        if (path != null && path.Count > 0) target = path[0];
    }

    void RotateTowardsTarget() {
        float stepSize = rotateSpeed * Time.deltaTime;
        Vector3 targetDirection = (aggro ? TempPlayer.Instance.Position : target.Position) - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, stepSize, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void MoveForward() {
        float stepSize = moveSpeed * Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, (aggro ? TempPlayer.Instance.Position : target.Position));
        if (distanceToTarget < stepSize) {
            return;
        }

        Vector3 moveDirection = Vector3.forward;
        transform.Translate(moveDirection * stepSize);
    }

    void Update() {
        if (dead) return;
        if (path.Count == 0 && !aggro) return;
        if (Input.GetMouseButtonDown(0)) {
            if (isObstructed) {
                print("Player is obstructed!");
                return;
            }
            isWalking = !isWalking;
            animator.SetBool(WALKING, isWalking);
        }

        if (Input.GetMouseButtonDown(1)) {
            if (legacy) {
                if (!isObstructed) {
                    print("No need to attack...");
                    return;
                }
                print("Breaking wall!");
                isObstructed = false;
                Destroy(currentObstruction);
                currentObstruction = null;
                isWalking = true;
                animator.SetTrigger(HIT);
                animator.SetBool(WALKING, isWalking);
            }
        }

        if (!isWalking) return;
        RotateTowardsTarget();
        MoveForward();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Waypoint")) return;
        if (!(other.name == $"{target.Index}")) return;
        /*if (index + 1 < path.Count) {
            index++;
        } else {

        }*/
        target = pathManager.GetNext(); 
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Breakable")) return;
        print("Player has stopped...");
        currentObstruction = collision.gameObject;
        isObstructed = true;
        isWalking = false;
        animator.SetBool(WALKING, isWalking);
    }

    public void Die() {
        if (dead) return;
        print($"{transform} died");
        dead = true;
        animator.CrossFade("Die_SwordShield", 0, 0);
    }

    public void Aggro() {
        aggro = true;
        isWalking = true;
        animator.SetBool(WALKING, isWalking);
    }
}
