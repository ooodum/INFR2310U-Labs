using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectState : State {
    float[] positions = { 460, 150, -150, -460};
    RectTransform container, iconBar;
    int selectedLevel, ID, outPos = 200;
    GameObject[] cams = GameManager.Instance.VCams;
    public LevelSelectState(GameObject[] gameObjects, StateManager manager) : base(gameObjects, manager) {
    }

    public override void Setup() {
        container = gameObjects[0].GetComponent<RectTransform>();
        iconBar = container.GetChild(1).GetComponent<RectTransform>();
        container.anchoredPosition = Vector2.up * outPos;
        iconBar.anchoredPosition = Vector2.right * positions[0];
        ID = container.GetInstanceID();
    }

    public override void Enter() {
        container.DOAnchorPosY(0, .5f);
        SelectLevel();
    }

    public override void Exit() {
        container.DOAnchorPosY(outPos, .5f);
    }

    public override void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (selectedLevel == 0) return;
            selectedLevel--;
            SelectLevel();
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if ( selectedLevel == positions.Length - 1) return;
            selectedLevel++;
            SelectLevel();
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            GameManager.Instance.MaintainCard = true;
            GameManager.Instance.SwitchState(manager.Save());
            foreach (GameObject cam in cams) {
                cam.SetActive(false);
            }
            CinemachineCore.Instance.GetActiveBrain(0).m_DefaultBlend.m_Time = 1.6f;
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            BlackScreen.Instance.On();
        }
    }

    void SelectLevel() {
        foreach(GameObject cam in cams) {
            cam.SetActive(false);
        }
        iconBar.DOAnchorPosX(positions[selectedLevel], .4f).SetId(ID);
        cams[selectedLevel].SetActive(true);
    }
}
