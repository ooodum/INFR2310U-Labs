using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;

public class Knucklotec : MonoBehaviour {
    [SerializeField] Transform head, lh, rh, mario;
    [SerializeField] Material normalMat, hitMat;
    [SerializeField] TextMeshProUGUI text;
    NavMeshAgent lhAgent, rhAgent, currentAgent;
    Vector3 lhPos, rhPos;
    bool isSeeking, damaged, right;
    private void Awake() {
        lhAgent = lh.GetComponent<NavMeshAgent>();
        rhAgent = rh.GetComponent<NavMeshAgent>();

        lhPos = lhAgent.transform.position;
        rhPos = rhAgent.transform.position;

        currentAgent = rhAgent;
        right = true;
    }
    private void Start() {
        Vector3 rotation = mario.position - head.position;
        rotation = new Vector3(rotation.x, 0, rotation.z);
        head.rotation = Quaternion.LookRotation(rotation, Vector3.up);

        IntroSequence();   
    }
    void Update() {
        Vector3 rotation = mario.position - head.position;
        rotation = new Vector3(rotation.x, 0, rotation.z);
        head.rotation = Quaternion.LookRotation(Vector3.RotateTowards(head.forward, rotation, Time.deltaTime, 0));

        text.text = currentAgent.remainingDistance.ToString();

        if (isSeeking) {
            print(currentAgent);
            currentAgent.destination = mario.position;
            if (currentAgent.remainingDistance < .3f && currentAgent.remainingDistance > 0) Punch();
        }
    }

    void Seek() {
        isSeeking = true;
    }

    void Punch() {
        isSeeking = false;
        Transform child = currentAgent.transform.GetChild(0);
        if (Physics.Raycast(child.position, Vector3.down, out RaycastHit info)) {
            if (info.transform.CompareTag("Ice")) {
                damaged = true;
                print("fdjsai");
            }
        }
        child.DOShakePosition(.6f, .2f).OnComplete(() => {
            child.DOLocalMoveY(1.3f, .4f).SetEase(Ease.InCirc).OnComplete(() => QueryLand(child));
        });
    }

    async void QueryLand(Transform child) {
        if (damaged) {
            print("wooo");
            Renderer renderer = child.GetComponent<Renderer>();
            renderer.material = hitMat;
            await Awaitable.WaitForSecondsAsync(.1f);
            renderer.material = normalMat;
            await Awaitable.WaitForSecondsAsync(.1f);
            renderer.material = hitMat;
            await Awaitable.WaitForSecondsAsync(.1f);
            renderer.material = normalMat;
        }
        damaged = false;
        Recover(child);
    }

    async void Recover(Transform child) {
        await Awaitable.WaitForSecondsAsync(.5f);
        child.DOLocalMoveY(4f, .7f).SetEase(Ease.InOutQuad);
        currentAgent.transform.DOMove(right ? rhPos : lhPos, 1).OnComplete(ChangeHand);
    }

    void ChangeHand() {
        right = !right;
        currentAgent.ResetPath();
        currentAgent = right ? rhAgent : lhAgent;
        Seek();
    }

    void IntroSequence() {
        lhAgent.transform.DOMoveX(5, 2f);
        rhAgent.transform.DOMoveX(-5,2f).OnComplete(() => {
            GameManager.Instance.CanMove = true;
            GameManager.Instance.CamFocus = true;
            Seek();
        });
    }
}
