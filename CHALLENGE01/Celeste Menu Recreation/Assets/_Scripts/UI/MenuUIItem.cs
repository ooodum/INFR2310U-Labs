using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MenuUIItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] MenuOverride menuOverride;

    RectTransform rect, mountain;
    public float anchorY {  get; private set; }
    int ID;
    public bool clicked, interactable;
    enum MenuOverride {
        None,
        Climb
    }
    private void Awake() {
        rect = transform.GetChild(0).GetComponent<RectTransform>();
        mountain = rect.GetChild(0).GetComponent<RectTransform>();
        anchorY = rect.anchoredPosition.y;
        ID = GetInstanceID();
        interactable = true;   
    }
    public void Hover() {
        if (!interactable) return;
        DOTween.Kill(ID);
        text.DOColor(Color.cyan, .2f);
        if (menuOverride == MenuOverride.None) {
            rect.DOAnchorPosX(30, .25f).SetEase(Ease.OutQuint).SetDelay(.1f).SetId(ID);
            Sequence wiggleSequence = DOTween.Sequence();
            wiggleSequence.Append(rect.DOAnchorPosY(anchorY - 5, .07f).SetEase(Ease.OutBack)).SetId(ID);
            wiggleSequence.Append(rect.DOAnchorPosY(anchorY + 7, .1f).SetEase(Ease.OutBack)).SetId(ID);
            wiggleSequence.Append(rect.DOAnchorPosY(anchorY, .2f).SetEase(Ease.OutCirc)).SetId(ID);
        } else if (menuOverride == MenuOverride.Climb) {
            Sequence wiggleSequence = DOTween.Sequence();
            wiggleSequence.Append(rect.DOAnchorPosY(anchorY - 10, .07f).SetEase(Ease.OutCirc)).SetId(ID);
            wiggleSequence.Append(rect.DOAnchorPosY(anchorY + 10, .1f).SetEase(Ease.OutCirc)).SetId(ID);
            wiggleSequence.Append(rect.DOAnchorPosY(anchorY, .2f).SetEase(Ease.OutCirc)).SetId(ID);
        }
    }

    public void UnHover() {
        if (!interactable) return;
        DOTween.Kill(ID);
        text.DOColor(Color.white, .2f);
        if (menuOverride == MenuOverride.None) {
            rect.DOAnchorPosX(0, .25f).SetEase(Ease.OutQuint).SetId(ID);
        } 
    }

    public void Error() {
        text.DOColor(new Color(1,.2f,.2f), .2f);
        rect.DOPunchPosition(Vector3.right * 15, .5f).SetId(ID);
    }

    public void Click() {
        DOTween.Kill(ID);
        interactable = false;
        if (menuOverride == MenuOverride.None) {

        } else if (menuOverride == MenuOverride.Climb) {
            interactable = false;
            text.GetComponent<RectTransform>().DOPunchPosition(Vector3.up * 6, .3f).SetId(ID);
            mountain.DOPunchPosition(Vector3.up * 10, .5f).SetId(ID);
            mountain.DOPunchRotation(Vector3.forward * 12, .5f, 8).SetId(ID).OnComplete(() => {
                rect.DOAnchorPosX(-500, .4f).OnComplete(() => {
                    rect.parent.parent.GetComponent<RectTransform>().anchoredPosition = Vector3.right * -500;
                    rect.DOAnchorPosX(0, 0);
                    foreach (RectTransform t in rect.parent.parent) { t.GetChild(0).GetComponent<RectTransform>().DOAnchorPosX(0, 0); }
                    interactable = true;
                    UnHover();
                });
            });
        }
    }
}
