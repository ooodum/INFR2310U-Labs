using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuState : State {
    Transform rotator;
    RectTransform menuBar;
    GameObject cam;
    List<MenuUIItem> menuItems = new List<MenuUIItem>();
    float rotateSpeed = 10;
    bool switching;
    public MenuState(GameObject[] gameObjects, StateManager manager) : base(gameObjects, manager) { }
    public override void Setup() {
        rotator = gameObjects[0].transform;
        cam = rotator.GetChild(0).gameObject;
        menuBar = gameObjects[1].GetComponent<RectTransform>();
        menuBar.anchoredPosition = Vector3.left * 500;
        foreach (var item in menuBar.GetComponentsInChildren<MenuUIItem>()) {
            menuItems.Add(item);
        }
    }
    public override void Enter() {
        switching = false;
        cam.SetActive(true);
        menuBar.DOAnchorPosX(0, .4f).SetDelay(GameManager.Instance.menuSlideTime).SetEase(Ease.OutCirc);
        GameManager.Instance.menuSlideTime = .4f;
        GameManager.Instance.MenuInteractEvent += Interact;
    }
    public override void Update() {
        rotator.Rotate(Vector3.up * -Time.deltaTime * rotateSpeed);
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("whefioewf");
            cam.SetActive(false);
            menuBar.DOAnchorPosX(-500, .4f);
            GameManager.Instance.SwitchState(manager.Title());
        }
    }
    public override void Exit() {
        GameManager.Instance.MenuInteractEvent -= Interact;
    }

    void Interact(int index) {
        switch (index) {
            case 0:
                menuItems[index].Click();
                for (int i = 0; i < menuItems.Count; i++) {
                    if (i == 0) continue;
                    menuBar.GetChild(i).GetChild(0).GetComponent<RectTransform>().DOAnchorPosX(-500, .4f).OnComplete(() => {
                        SwitchState();
                    });
                }
                break;
            case 3:
                menuBar.DOAnchorPosX(-500, .7f).OnComplete(() => GameManager.Instance.SwitchState(manager.Credit()));
                break;
            case 4:
                menuBar.DOAnchorPosX(-500, .7f);
                BlackScreen.Instance.On();
                break;
            default:
                menuItems[index].Error();
                break;
        }
    }

    void SwitchState() {
        if (switching) return;
        switching = true;
        GameManager.Instance.SwitchState(manager.Save());
    }
}
