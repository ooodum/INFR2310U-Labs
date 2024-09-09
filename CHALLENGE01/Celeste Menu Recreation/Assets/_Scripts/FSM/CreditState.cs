using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditState : State {
    public CreditState(GameObject[] gameObjects, StateManager manager) : base(gameObjects, manager) {}
    Transform rotator;
    Image vignette;
    TextMeshProUGUI text;
    RectTransform prompt;
    float vignetteAlpha = .53f, rotateSpeed = 10;

    public override void Enter() {
        vignette.DOFade(vignetteAlpha, .6f).SetEase(Ease.Linear);
        text.DOFade(1, .8f).SetDelay(.3f).SetEase(Ease.Linear);
        prompt.DOAnchorPosY(40, .5f).SetDelay(.5f);
    }

    public override void Exit() {
        text.DOFade(0, .4f).SetEase(Ease.Linear);
        vignette.DOFade(0, .4f).SetEase(Ease.Linear);
        prompt.DOAnchorPosY(-110, .5f);
    }

    public override void Setup() {
        vignette = gameObjects[0].GetComponent<Image>();
        vignette.DOFade(0, 0);

        rotator = gameObjects[1].transform;

        text = gameObjects[2].GetComponent<TextMeshProUGUI>();
        text.DOFade(0, 0);

        prompt = gameObjects[3].GetComponent<RectTransform>();
        prompt.anchoredPosition = new Vector3(-40, -110, 0);
    }

    public override void Update() {
        rotator.Rotate(Vector3.up * -Time.deltaTime * rotateSpeed);

        if (Input.GetKeyDown(KeyCode.Escape)) {
            GameManager.Instance.SwitchState(manager.Menu());
        }
    }
}
