using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint {
    [SerializeField] private Vector3 position;
    private int index;
    public Vector3 Position { get { return position; } set { position = value; } }
    public int Index { get { return index; } set { index = value; } } 

    public Waypoint() {
        position = Vector3.zero;
    }
}
