using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Crosshair : MonoBehaviour {
    CanvasGroup canvasGroup;
    [SerializeField] EventManager eventManager;
    [SerializeField] TextMeshProUGUI text;
    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void OnEnable() {
        eventManager.OnMessageBroadcast += ProcessEvent;
    }
    private void OnDisable() {
        eventManager.OnMessageBroadcast -= ProcessEvent;
    }

    void ProcessEvent(Message message) {
        if (message.message == EventCodes.INTERACTABLE_HOVERED) {
            text.text = "E to " + message.value;
            canvasGroup.alpha = 1;
        } else if (message.message == EventCodes.INTERACTABLE_UNHOVERED) {
            canvasGroup.alpha = 0;
        }
    }
}
