using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEMP_AnimationChanger : MonoBehaviour {
    Animator animator;
    bool mirrored = false;
    [SerializeField] Slider slider, crouchSlider;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    void Start() {
        animator.SetFloat("Blend", 0);
        slider.onValueChanged.AddListener(SetBlend);
        crouchSlider.onValueChanged.AddListener(SetCrouch);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            mirrored = !mirrored;
            animator.SetBool("Mirrored", mirrored);
        }
    }

    void SetBlend(float v) {
        animator.SetFloat("Blend", v);
    }

    void SetCrouch(float v) {
        animator.SetLayerWeight(2, v);
    }
}
