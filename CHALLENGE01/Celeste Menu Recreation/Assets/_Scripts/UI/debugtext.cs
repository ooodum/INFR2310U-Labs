using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class debugtext : MonoBehaviour {
    TextMeshProUGUI text;
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>(); 
    }

    private void Update() {
        if (text != null) {
            text.text = $"{GameManager.Instance.currentState}";
        }
    }
}
