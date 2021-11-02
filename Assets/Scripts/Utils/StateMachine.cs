using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StateCallback();

public class StateMachine {

    private string currentState;
    private string nextFrameState;
    private HashSet<string> states;
    private Dictionary<string, StateCallback> startCallbacks;
    private Dictionary<string, StateCallback> updateCallbacks;
    private Dictionary<string, StateCallback> endCallbacks;

    public StateMachine() {
        states = new HashSet<string>();
        startCallbacks = new Dictionary<string, StateCallback>();
        updateCallbacks = new Dictionary<string, StateCallback>();
        endCallbacks = new Dictionary<string, StateCallback>();
    }

    public void Add(string state, StateCallback startCallback, StateCallback updateCallback, StateCallback endCallback) {
        states.Add(state);
        if(startCallback != null) startCallbacks.Add(state, startCallback);
        if(updateCallback != null) updateCallbacks.Add(state, updateCallback);
        if(endCallback != null) endCallbacks.Add(state, endCallback);
    }

    public void ChangeState(string newState) {
        if(!states.Contains(newState)) Debug.Log("StateMachine does not contain state: " + newState);
        if(currentState != null && endCallbacks.ContainsKey(currentState)) endCallbacks[currentState]();
        currentState = newState;
        if(newState != null && startCallbacks.ContainsKey(newState)) startCallbacks[newState]();
    }

    public void ChangeStateNextFrame(string newState) {
        if(currentState != null && endCallbacks.ContainsKey(currentState)) endCallbacks[currentState]();
        nextFrameState = newState;
    }

    public void Update() {
        if(nextFrameState != null) {
            currentState = nextFrameState;
            nextFrameState = null;
            if(currentState != null && startCallbacks.ContainsKey(currentState)) startCallbacks[currentState]();
        }
        if(currentState != null && updateCallbacks.ContainsKey(currentState)) updateCallbacks[currentState]();
    }

}
