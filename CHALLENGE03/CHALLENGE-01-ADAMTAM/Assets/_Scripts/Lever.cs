using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable {
    [SerializeField] EventManager eventManager;
    [SerializeField] Transform lever, ikTarget;
    string descrption = "pull lever";
    bool canInteract = true, active;
    public string Description { get { return descrption; } set { name = value; } }

    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }

    public void Interact(PlayerController controller) {
        canInteract = false;
        eventManager.BroadcastMessage(new Message(EventCodes.LEVEL_PULLED, transform));
        controller.AnimateHandIK(false, ikTarget, .3f);
        lever.DORotate(Vector3.right * (active ? -40 : 40), 1).SetDelay(.5f).OnComplete(() => {
            controller.CanMove = true;
            controller.ResetHands();
            active = !active;
            canInteract = true;
        });
    }
}
