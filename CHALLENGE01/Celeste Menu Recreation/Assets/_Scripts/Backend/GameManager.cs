using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public State currentState {  get; private set; }  
    public StateManager stateManager { get; private set; }  

    public GameObject[] TitleObjects, MenuObjects, CreditObjects, SaveObjects, NewGameObjects, LevelSelectObjects;
    public GameObject[] VCams;
    public bool Switching, MaintainCard;
    public float menuSlideTime;

    public event Action<int> MenuInteractEvent;
    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this); else Instance = this;
        stateManager = new StateManager(this);
    }
    private void Start() {
        currentState = stateManager.Title();
        currentState.Enter();
    }

    public void SwitchState(State state) {
        currentState.ExitSequence();
        currentState = state;
        currentState.Enter();
    }

    private void Update() {
        if (Switching) {
            Switching = false;
            return;
        }
        currentState.UpdateSequence();
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }
    }

    public void Interact(int index) {
        MenuInteractEvent?.Invoke(index);
    }
}
