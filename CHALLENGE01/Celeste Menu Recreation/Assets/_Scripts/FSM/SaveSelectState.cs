using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSelectState : State {
    List<RectTransform> tickets = new List<RectTransform>();
    Transform rotator;
    CanvasGroup container;
    float rotateSpeed = 10;

    public SaveSelectState(GameObject[] gameObjects, StateManager manager) : base(gameObjects, manager) { }

    public override void Enter() {
        TicketManager ticketManager = gameObjects[0].GetComponent<TicketManager>();
        for (int i = 0; i < tickets.Count; i++) {
            RectTransform tempTicket = tickets[i];
            if (ticketManager.selecting) {
                Debug.Log("selecting");
                if (ticketManager.index == i) {
                    Debug.Log("woo");
                    tempTicket.GetComponent<Ticket>().interactable = true;
                    tempTicket.GetComponent<Ticket>().Hover();
                    tempTicket.GetComponent<Ticket>().interactable = false;
                    tempTicket.DOAnchorPosX(960, .7f).SetDelay(i * .1f);
                } else
                {
                    tempTicket.GetComponent<Ticket>().interactable = true;
                    tempTicket.DOAnchorPosX(960, .7f).SetDelay(i * .1f);
                }
            } else {
                tempTicket.GetComponent<Ticket>().interactable = true;
                tempTicket.DOAnchorPosX(960, .7f).SetDelay(i * .1f);
            }
        }
        container.blocksRaycasts = true;
    }

    public override void Exit() {
        for (int i = 0; i < tickets.Count; i++) {
            Ticket temp = tickets[i].GetComponent<Ticket>();
            temp.interactable = true;
            temp.UnHover();
            temp.interactable = false;
            tickets[i].DOAnchorPosX(2250, .7f).SetDelay(i * .1f);
        }
        container.blocksRaycasts = false;
    }

    public override void Setup() {
        foreach (Transform ticket in gameObjects[0].transform) {
            this.tickets.Add(ticket.GetComponent<RectTransform>());
        }
        foreach (var ticket in tickets) {
            ticket.DOAnchorPosX(2250, 0);
        }
        rotator = gameObjects[1].transform;
        container = gameObjects[2].GetComponent<CanvasGroup>();
        container.blocksRaycasts = false;
    }

    public override void Update() {
        rotator.Rotate(Vector3.up * -Time.deltaTime * rotateSpeed);
        // CANCEL/BACKMENU LOGIC IS LOCATED IN TICKETMANAGER.CS
    }
}
