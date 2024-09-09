using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] Transform character;
    Vector3 vel = Vector3.zero, offset = new Vector3(2.7f, 9);

    void Update() {
        transform.position = Vector3.SmoothDamp(transform.position, character.position + offset, ref vel, .7f);
    }
}
