using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {

    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;

    private Camera playerCamera;
    private Rigidbody playerRigidbody;

    public void Awake() {
        playerCamera = GetComponent<Camera>();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    public void LateUpdate() {
        transform.position = player.position + offset;
    }

    public void InitPlayerViewport(int playerNumber, int playerCount) {
        playerCamera.rect = GetPlayerViewportRect(playerNumber, playerCount);
    }

    // this function defines how the screen should be split depending on player count
    private Rect GetPlayerViewportRect(int playerNumber, int playerCount) {
        if(playerNumber == 1) {
            if(playerCount == 1) return new Rect(0f, 0f, 1f, 1f);
            else if(playerCount == 2) return new Rect(0f, 0.5f, 1f, 0.5f);
            else if(playerCount == 3) return new Rect(0f, 0.5f, 1f, 0.5f);
            else return new Rect(0f, 0.5f, 0.5f, 0.5f);
        }
        else if(playerNumber == 2) {
            if(playerCount == 2) return new Rect(0f, 0f, 1f, 0.5f);
            else if(playerCount == 3) return new Rect(0f, 0f, 0.5f, 0.5f);
            else return new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else if(playerNumber == 3) {
            if(playerCount == 3) return new Rect(0.5f, 0f, 0.5f, 0.5f);
            else return new Rect(0f, 0f, 0.5f, 0.5f);
        }
        else return new Rect(0.5f, 0f, 0.5f, 0.5f);
    }
}
