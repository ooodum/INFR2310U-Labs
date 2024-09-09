using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event Manager", menuName = "Scriptable Objects/Event Manager")]
public class EventManager : ScriptableObject {
    public event Action<Message> OnMessageBroadcast;
    public void BroadcastMessage(Message message) {
        OnMessageBroadcast?.Invoke(message);
    }
}

public struct Message {
    public string message;
    public string value;
    public Transform transform;
    
    public Message(string message) {
        this.message = message;
        value = "";
        transform = null;
    }

    public Message(string message, string value) {
        this.message = message;
        this.value = value;
        transform = null;
    }

    public Message(string message, Transform transform) {
        this.message = message;
        this.transform = transform;
        value = "";
    }

    public Message(string message, string value, Transform transform) {
        this.message = message;
        this.value = value;
        this.transform = transform;
    }
}
