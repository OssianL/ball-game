using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private int playerCount;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Material[] playerMaterials;

    public void Start() {
        SpawnPlayers();
    }

    private void SpawnPlayers() {
        // player spawns are defined with empty objects that have the tag "PlayerSpawner"
        GameObject[] playerSpawners = GameObject.FindGameObjectsWithTag("PlayerSpawner");
        if(playerSpawners.Length < playerCount) Debug.Log("Not enough player spawners!");
        for(int i = 0; i < playerCount; i++) {
            Vector3 playerSpawnPosition = playerSpawners[i].transform.position;
            GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            PlayerCameraController playerCameraController = playerInstance.GetComponentInChildren<PlayerCameraController>();
            playerCameraController.InitPlayerViewport(i+1, playerCount);
            PlayerController playerController = playerInstance.GetComponentInChildren<PlayerController>();
            playerController.InitPlayer(i+1, playerMaterials[i]);
        }
    }

}
