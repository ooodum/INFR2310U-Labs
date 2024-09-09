using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketManager : MonoBehaviour {
    List<Ticket> tickets = new List<Ticket>();
    List<RectTransform> ticketRects = new List<RectTransform>();
    List<Vector3> ticketPositions = new List<Vector3>();
    [SerializeField] float selectedPos, outPosUp, outPosDown, selectionSelectPos;
    public bool selecting;
    public int index;
    private void Awake() {
        foreach(Transform child in transform) {
            tickets.Add(child.GetComponent<Ticket>());
            ticketRects.Add(child.GetComponent<RectTransform>());
            ticketPositions.Add(child.GetComponent<RectTransform>().anchoredPosition);
        }
    }

    public void Select(int index) {
        this.index = index;
        selecting = true;
        for (int i = 0; i < tickets.Count; i++) {
            if (i == index) {
                ticketRects[i].DOAnchorPosY(selectedPos, .2f);
                tickets[i].Hover();
                tickets[i].interactable = false;
                ticketRects[i].GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(selectionSelectPos, .3f);
            } else {
                ticketRects[i].DOAnchorPosY((i < index) ? outPosUp : outPosDown, .2f);
            }
        }
    }

    private void Update() {
        if (GameManager.Instance.currentState != GameManager.Instance.stateManager.Save()) return;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!selecting) {
                GameManager.Instance.SwitchState(GameManager.Instance.stateManager.Menu());
                for (int i = 0; i < tickets.Count; i++) {
                    tickets[i].interactable = true;
                    tickets[i].UnHover();
                    tickets[i].interactable = false;
                }
            } else {
                selecting = false;
                for (int i = 0;i < tickets.Count;i++) {
                    ticketRects[i].DOAnchorPos(ticketPositions[i], .2f);
                    tickets[i].interactable = true;
                    tickets[i].UnHover();
                    ticketRects[i].GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(0, .3f);
                }
            }
        }
    }
}
