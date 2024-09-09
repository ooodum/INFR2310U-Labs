using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.FiniteStateMachine;
using Worq;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.AI;

public class AIStateMachine : AbstractFiniteStateMachine {
    [HideInInspector]
    public AWSPatrol patrol;
    public LayerMask obstructionMask;
    public Transform player;
    public GameObject hitbox;
    public Animator animator;
    public Image questionImage;
    public CanvasGroup questionCanvas, alertCanvas;
    public NavMeshAgent agent;
    public bool invincible = false;
    public enum AIState {
        PATROL,
        DETECTION,
        ALERT,
        ATTACK,
        HIT
    }
    private void Awake() {
        Init(AIState.PATROL,
            AbstractState.Create<PatrolState, AIState>(AIState.PATROL, this),
            AbstractState.Create<DetectionState, AIState>(AIState.DETECTION, this),
            AbstractState.Create<AlertState, AIState>(AIState.ALERT, this),
            AbstractState.Create<AttackState, AIState>(AIState.ATTACK, this),
            AbstractState.Create<HitState, AIState>(AIState.HIT, this)
        );
        patrol = GetComponent<AWSPatrol>();
        hitbox.SetActive(false);
    }
    public class PatrolState : AbstractState {
        AIStateMachine parent;
        public override void OnEnter() {
            parent = GetStateMachine< AIStateMachine>();
        }
        public override void OnUpdate() {
            parent.animator.SetBool("Walking", parent.patrol.isWaiting);
        }
        public override void OnFixedUpdate() {
            if (Physics.Raycast(parent.transform.position, parent.player.position - parent.transform.position, out RaycastHit info, 5, parent.obstructionMask)) {
                if (info.transform.CompareTag("Player")) {
                    if (Vector3.Dot(parent.transform.forward, parent.player.position - parent.transform.position) > 0) {
                        TransitionToState(AIState.DETECTION);
                    }
                }
            }
        }
        public override void OnExit() {
        }
    }
    public class DetectionState : AbstractState {
        AIStateMachine parent;
        float detectionAmount; //0 is min, 1 is max
        float fillTime = 3;
        float fillFactor;
        float distance = 8;
        public override void OnEnter() {

            parent = GetStateMachine< AIStateMachine>();

            fillFactor = 1f / fillTime;

            parent.patrol.InterruptPatrol();
            detectionAmount = 0;
            parent.questionCanvas.alpha = 1;

            parent.animator.SetBool("Walking", false);
            parent.animator.SetBool("Looking", true);
        }
        public override void OnUpdate() {
        }
        public override void OnFixedUpdate() {
            if (Physics.Raycast(parent.transform.position, parent.player.position - parent.transform.position, out RaycastHit info, distance, parent.obstructionMask)) {
                if (info.transform.CompareTag("Player")) {
                    detectionAmount += Time.fixedDeltaTime * fillFactor * (distance / (parent.player.position - parent.transform.position).magnitude);
                } else {
                    detectionAmount -= Time.fixedDeltaTime * fillFactor * .5f; //depletes slower
                }
            } else {
                detectionAmount -= Time.fixedDeltaTime * fillFactor * .5f;
            }
            parent.questionImage.fillAmount = detectionAmount;
            if (detectionAmount >= 1) {
                TransitionToState(AIState.ALERT);
            }
            if (detectionAmount <= 0) {
                TransitionToState(AIState.PATROL);
                parent.patrol.ResetPatrol();
            }
        }
        public override void OnExit() {
            parent.animator.SetBool("Looking", false);
            parent.questionCanvas.alpha = 0;
        }
    }
    public class AlertState : AbstractState {
        AIStateMachine parent;
        NavMeshAgent agent;

        public override void OnEnter() {
            parent = GetStateMachine< AIStateMachine>();
            parent.alertCanvas.alpha = 1;
            agent = parent.agent;

            parent.patrol.enabled = false;

            parent.animator.SetBool("Running", true);
            agent.speed = 4.2f;

            agent.ResetPath();
            agent.isStopped = false;

        }
        public override void OnUpdate() {
            agent.SetDestination(parent.player.transform.position);
            if ((parent.transform.position - parent.player.position).magnitude < .8f) {
                TransitionToState(AIState.ATTACK);
            }
        }
        public override void OnFixedUpdate() {
        }
        public override void OnExit() {
            parent.animator.SetBool("Running", false);
            agent.speed = 3.5f;
            parent.patrol.ResetPatrol();
            parent.patrol.enabled = true;
        }
    }
    public class AttackState : AbstractState {
        AIStateMachine parent;
        float timer, pretimer;

        public override void OnEnter() {
            parent = GetStateMachine< AIStateMachine>();

            parent.alertCanvas.alpha = 0;
            parent.questionCanvas.alpha = 0;

            parent.patrol.InterruptPatrol();

            parent.patrol.enabled = false;

            parent.agent.isStopped = true;

            parent.animator.SetTrigger("Hit");
            parent.animator.SetBool("Walking", false);

            pretimer = .2f;
            timer = .6f;
        }
        public override void OnUpdate() {
            if (pretimer > 0) {
                pretimer -= Time.deltaTime;
                return;
            } else if (timer > 0) {
                parent.hitbox.SetActive(true);
                timer -= Time.deltaTime;
                return;
            }
            parent.hitbox.SetActive(false);
        }
        public override void OnFixedUpdate() {
        }
        public override void OnExit() {
        }
    }
    public class HitState : AbstractState {
        AIStateMachine parent;

        public override void OnEnter() {
            parent = GetStateMachine< AIStateMachine>();

            parent.patrol.InterruptPatrol();
            parent.agent.isStopped = true;

            parent.animator.SetTrigger("Die");
        }
        public override void OnUpdate() {
            
        }
        public override void OnFixedUpdate() {
        }
        public override void OnExit() {
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            TransitionToState(AIState.HIT);
        }
    }
}
