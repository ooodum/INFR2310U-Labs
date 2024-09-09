using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayer : Singleton<TempPlayer> {
    public Vector3 Position;
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        Position = transform.position;
    }
}
