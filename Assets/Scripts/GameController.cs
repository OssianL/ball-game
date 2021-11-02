using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private int playerCount;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Material[] playerMaterials;

    private Animator animator;

    private StateMachine stateMachine;
    private GameObject[] players;
    private PlayerController[] playerControllers;

    public void Awake() {
        stateMachine = new StateMachine();
        stateMachine.Add("warmup", WarmupStart, null, null);
        stateMachine.Add("countdown", CountdownStart, null, null);
        stateMachine.Add("main", MainStart, MainUpdate, MainEnd);
        stateMachine.Add("gameOver", GameOverStart, GameOverUpdate, null);
        stateMachine.ChangeState("warmup");

        animator = GetComponent<Animator>();
    }

    public void Start() {
        SpawnPlayers();
    }

    public void Update() {
        stateMachine.Update();
    }

    private void SpawnPlayers() {
        players = new GameObject[playerCount];
        playerControllers = new PlayerController[playerCount];
        // player spawns are defined with empty GameObjects that have the tag "PlayerSpawner"
        GameObject[] playerSpawners = GameObject.FindGameObjectsWithTag("PlayerSpawner");
        if(playerSpawners.Length < playerCount) Debug.Log("Not enough player spawners!");
        for(int i = 0; i < playerCount; i++) {
            Vector3 playerSpawnPosition = playerSpawners[i].transform.position;
            GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            PlayerCameraController playerCameraController = playerInstance.GetComponentInChildren<PlayerCameraController>();
            playerCameraController.InitPlayerViewport(i+1, playerCount);
            PlayerController playerController = playerInstance.GetComponentInChildren<PlayerController>();
            playerControllers[i] = playerController;
            playerController.InitPlayer(i+1, playerMaterials[i]);
        }
    }

    private void WarmupStart() {
        // TODO: play camera animation, one camera
        stateMachine.ChangeStateNextFrame("countdown");
    }

    private void CountdownStart() {
        // TODO: switch to split screen, start countdown (animation?)
        animator.SetTrigger("CountdownStart");
    }

    // animation event called by animator
    private void OnCountdownAnimationEnd() {
        Debug.Log("count down animation end");
        stateMachine.ChangeState("main");
    }

    private void MainStart() {
        // TODO: release players
        foreach(PlayerController playerController in playerControllers) {
            Debug.Log("release");
            playerController.Release();
        }
    }

    private void MainUpdate() {
        if(AllPlayersFinished()) stateMachine.ChangeState("gameOver");
    }

    private void MainEnd() {

    }

    private void GameOverStart() {
        // TODO: score board?
        Debug.Log("game over mään");
    }

    private void GameOverUpdate() {
        // TODO: restart game
    }

    private bool AllPlayersFinished() {
        foreach(PlayerController player in playerControllers) {
            if(!player.IsFinished()) return false;
        }
        return true;
    }

}
