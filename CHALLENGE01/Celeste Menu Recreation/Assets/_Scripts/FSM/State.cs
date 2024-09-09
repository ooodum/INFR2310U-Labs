using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State {
    protected GameObject[] gameObjects;
    protected StateManager manager;
    public State(GameObject[] gameObjects, StateManager manager) {
        this.gameObjects = gameObjects;
        this.manager = manager;
    }
    public abstract void Setup();
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();


    public void UpdateSequence() {
        GameManager.Instance.Switching = false;
        Update();
    }
    public void ExitSequence() {
        GameManager.Instance.Switching = true;
        Exit();
    }
}
