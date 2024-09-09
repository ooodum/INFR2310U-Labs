using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseCast : Singleton<MouseCast> {
    Camera cam;
    [SerializeField] TextMeshProUGUI text;
    PathController currentController;
    public List<PathController> allControllers = new List<PathController>();

    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            text.text = hit.transform.name;
            if (hit.transform.TryGetComponent(out PathController controller)) currentController = controller;
        } else {
            text.text = "...";
            currentController = null;
        }
        if (Input.GetMouseButtonDown(1)) {
            if (currentController != null) currentController.Die();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (PathController controller in allControllers) {
                controller.Aggro();
            }
        }
    }
}
