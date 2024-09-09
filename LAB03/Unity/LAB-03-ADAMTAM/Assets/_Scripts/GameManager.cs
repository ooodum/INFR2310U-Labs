using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool CanMove, CamFocus;
    public Mario mario;
    private void Awake() {
        if (Instance != null) Destroy(Instance);
        Instance = this;
    }

    private void Start() {
    }
    public void SetMove(bool move) {
        CanMove = move;
    }
}
