using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticket : MonoBehaviour {
    [SerializeField] RectTransform inside, outside;
    public bool interactable;
    Ease ease = Ease.InOutQuad;
    float time = .3f, openAmount = 280;

    private void Awake() {
        interactable = false;
        inside.DOAnchorPosX(0, 0);
        outside.DOAnchorPosX(0, 0);
    }

    public void Hover() {
        if (!interactable) return;
        inside.DOAnchorPosX(openAmount, time).SetEase(ease);
        outside.DOAnchorPosX(-openAmount, time).SetEase(ease);
    }

    public void UnHover() {
        if (!interactable) return;
        inside.DOAnchorPosX(0, time).SetEase(ease);
        outside.DOAnchorPosX(0, time).SetEase(ease);
    }
}
