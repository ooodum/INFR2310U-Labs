using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TicketSelection : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;

    RectTransform rect;
    public enum Type {
        Error,
        Level,
        New
    }
    [SerializeField] Type type;
    public Vector3 anchor { get; private set; }
    int ID;
    private void Awake() {
        rect = GetComponent<RectTransform>();
        anchor = rect.anchoredPosition;
        ID = GetInstanceID();
    }
    public void Hover() {
        DOTween.Kill(ID);
        text.DOColor(Color.cyan, .2f);
        rect.DOPunchPosition(Vector3.right * 15, .4f, 8).SetId(ID);
    }

    public void UnHover() {
        DOTween.Kill(ID);
        text.DOColor(Color.white, .2f);
        rect.DOAnchorPosX(anchor.x, .1f);
    }

    public void Click() {
        DOTween.Kill(ID);
        switch (type) {
            case Type.Error:
                Error();
                break;
            case Type.Level:
                CinemachineCore.Instance.GetActiveBrain(0).m_DefaultBlend.m_Time = .5f;
                GameManager.Instance.SwitchState(GameManager.Instance.stateManager.LevelSelect());
                break;
            case Type.New:
                CinemachineCore.Instance.GetActiveBrain(0).m_DefaultBlend.m_Time = .7f;
                GameManager.Instance.SwitchState(GameManager.Instance.stateManager.NewGame());
                break;
        }
    }

    public void Error() {
        text.DOColor(new Color(1, .2f, .2f), .2f);
        rect.DOPunchPosition(Vector3.right * 15, .5f).SetId(ID);
    }

    /*public void Click() {
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
    }*/
}
