using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleState : State {
    CanvasGroup splash, ui;
    ParticleSystem particles;
    Image logo;
    RectTransform uiRect, logoRect;
    GameObject cam;
    public TitleState(GameObject[] gameObjects, StateManager manager) : base(gameObjects, manager) { }
    public override void Setup() {
        splash = gameObjects[0].GetComponent<CanvasGroup>();
        ui = gameObjects[1].GetComponent<CanvasGroup>();
        uiRect = ui.GetComponent<RectTransform>();
        logo = splash.transform.GetChild(1).GetComponent<Image>();
        logoRect = logo.rectTransform;
        particles = gameObjects[2].GetComponent<ParticleSystem>();
        cam = gameObjects[3];

        splash.alpha = 1;
        ui.alpha = 1;
    }
    public override void Enter() {
        cam.SetActive(true);
        splash.blocksRaycasts = true;
        ui.blocksRaycasts = true;

        logo.DOFade(1, .5f).SetEase(Ease.Linear);
        splash.DOFade(1, .5f).SetEase(Ease.Linear);
        logoRect.DOScale(1, 1.5f).SetEase(Ease.InOutCubic);
        ui.DOFade(1, .5f).SetEase(Ease.Linear);

        uiRect.DOAnchorPosY(0, .5f).SetEase(Ease.InCubic);
        particles.Play();
        GameManager.Instance.menuSlideTime = 1.2f;
    }
    public override void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GameManager.Instance.SwitchState(manager.Menu());
        }
    }
    public override void Exit() {
        particles.Stop();
        splash.blocksRaycasts = false;
        ui.blocksRaycasts = false;

        uiRect.DOAnchorPosY(-200, .5f).SetEase(Ease.InCubic);
        ui.DOFade(0, 1).SetEase(Ease.Linear);

        logo.DOFade(0, .7f).SetEase(Ease.Linear).OnComplete(() => {
            splash.DOFade(0, .3f).SetEase(Ease.Linear);
        });
        logoRect.DOScale(0, 1).SetEase(Ease.InCubic);
        cam.SetActive(false);
    }
}
