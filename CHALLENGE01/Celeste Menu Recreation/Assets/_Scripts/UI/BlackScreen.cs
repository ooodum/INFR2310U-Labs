using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour {
    public static BlackScreen Instance;
    [SerializeField] CanvasGroup screen;
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); else Instance = this;
    }
    // Start is called before the first frame update
    void Start() {
        screen.alpha = 0;
        screen.blocksRaycasts = false;
        screen.interactable = false;
    }

    public void On() {
        Time.timeScale = 0;
        screen.DOFade(1, 1.3f).SetUpdate(true).SetEase(Ease.Linear);
    }
}
