using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private int playerCount;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Color[] playerColors;
    [SerializeField] private float gameOverDelay;

    private AudioManager audioManager;
    private Animator animator;
    private StateMachine stateMachine;
    private GameObject[] players;
    private PlayerController[] playerControllers;
    private GameObject levelInstance;
    private float gameOverStartTime;
    private int finishedPlayersCounter;

    public void Awake() {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        animator = GetComponent<Animator>();

        stateMachine = new StateMachine();
        stateMachine.Add("mainMenu", MainMenuStart, null, MainMenuEnd);
        stateMachine.Add("loadLevel", LoadLevelStart, null, null);
        stateMachine.Add("spawnPlayers", SpawnPlayersStart, null, null);
        stateMachine.Add("countdown", CountdownStart, null, null);
        stateMachine.Add("main", MainStart, MainUpdate, MainEnd);
        stateMachine.Add("gameOver", GameOverStart, GameOverUpdate, GameOverEnd);
        stateMachine.ChangeState("mainMenu");
    }

    public void Start() {
        SetPlayerCount(playerCount);
    }

    public void Update() {
        stateMachine.Update();
    }

    public void SetPlayerCount(int playerCount) {
        this.playerCount = playerCount;
    }
    
    public int GetPlayerCount() {
        return playerCount;
    }

    public void StartNewGame() {
        stateMachine.ChangeState("loadLevel");
    }

    public int OnPlayerFinish() {
        finishedPlayersCounter++;
        return finishedPlayersCounter;
    }

    private void InstantiateLevel() {
        levelInstance = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
    }

    private void MainMenuStart() {
        mainMenu.SetActive(true);
        audioManager.StartMusic("menu");
    }

    private void MainMenuEnd() {
        mainMenu.SetActive(false);
        audioManager.StopMusic();
    }

    private void LoadLevelStart() {
        InstantiateLevel();
        stateMachine.ChangeStateNextFrame("spawnPlayers");
    }

    private void SpawnPlayersStart() {
        players = new GameObject[playerCount];
        playerControllers = new PlayerController[playerCount];
        finishedPlayersCounter = 0;
        // player spawns are defined with empty GameObjects that have the tag "PlayerSpawner"
        GameObject[] playerSpawners = GameObject.FindGameObjectsWithTag("PlayerSpawner");
        if(playerSpawners.Length < playerCount) Debug.Log("Not enough player spawners!");
        for(int i = 0; i < playerCount; i++) {
            Vector3 playerSpawnPosition = playerSpawners[i].transform.position;
            GameObject playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            players[i] = playerInstance;
            PlayerCameraController playerCameraController = playerInstance.GetComponentInChildren<PlayerCameraController>();
            playerCameraController.InitPlayerViewport(i+1, playerCount);
            PlayerController playerController = playerInstance.GetComponentInChildren<PlayerController>();
            playerControllers[i] = playerController;
            playerController.InitPlayer(i+1, playerColors[i]);
        }
        stateMachine.ChangeStateNextFrame("countdown");
    }

    private void CountdownStart() {
        // TODO: switch to split screen, start countdown (animation?)
        animator.SetTrigger("CountdownStart");
    }

    // animation event called by animator
    private void OnCountdown321() {
        audioManager.PlaySoundEffect("countdown321");
    }

    // animation event called by animator
    private void OnCountdownGo() {
        audioManager.PlaySoundEffect("countdownGo");
    }

    // animation event called by animator
    private void OnCountdownAnimationEnd() {
        stateMachine.ChangeState("main");
    }

    private void MainStart() {
        // TODO: release players
        foreach(PlayerController playerController in playerControllers) {
            playerController.Release();
        }
        audioManager.StartMusic("game");
    }

    private void MainUpdate() {
        if(GameOverTrigger()) stateMachine.ChangeState("gameOver");
    }

    private void MainEnd() {

    }

    private void GameOverStart() {
        gameOverStartTime = Time.time;
        audioManager.StartMusic("end", false);
    }

    private void GameOverUpdate() {
        if(Time.time > (gameOverStartTime + gameOverDelay)) stateMachine.ChangeState("mainMenu");
    }
    
    private void GameOverEnd() {
        Destroy(levelInstance);
        playerControllers = null;
        foreach(GameObject player in players) {
            Destroy(player);
        }
        players = null;
    }

    private bool GameOverTrigger() {
        return finishedPlayersCounter >= playerCount;
    }

}
