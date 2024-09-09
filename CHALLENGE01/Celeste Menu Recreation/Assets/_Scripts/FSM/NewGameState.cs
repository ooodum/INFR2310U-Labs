using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameState : State {
    public NewGameState(GameObject[] gameObjects, StateManager manager) : base(gameObjects, manager) {
    }

    public override void Enter() {
        gameObjects[2].SetActive(false);
        gameObjects[3].SetActive(true);
        gameObjects[0].GetComponent<ParticleSystem>().Play();
            
        gameObjects[1].GetComponent<CanvasGroup>().DOFade(1, .7f).SetEase(Ease.Linear).SetDelay(1.6f).OnComplete(() => BlackScreen.Instance.On());
    }

    public override void Exit() {
    }

    public override void Setup() {
    }

    public override void Update() {
    }
}
