using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
    
    [SerializeField] private Camera mainMenuCamera;
    [SerializeField] private Transform[] playerCountBoxes;
    [SerializeField] private Transform[] playerCountTexts;
    [SerializeField] private Transform startGameButtonBox;
    [SerializeField] private Transform startGameButtonText;

    private GameController gameController;
    private AudioManager audioManager;
    private Transform lastHoveredButton;

    public void Start() {
        gameController = GameObject.FindObjectOfType<GameController>();
        audioManager = GameObject.FindObjectOfType<AudioManager>();
        SetPlayerCount(1, false);
    }

    public void Update() {
        if(Input.GetButtonDown("MouseLeftButton")) CheckClickHit();
        CheckHoverHit();
    }

    private void CheckClickHit() {
        RaycastHit hit;
        Ray ray = mainMenuCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("MainMenu"))) {
            Transform h = hit.transform;
            for(int i = 0; i < 4; i++) {
                if(h == playerCountBoxes[i]) SetPlayerCount(i+1);
            }
            if(h == startGameButtonBox) StartGame();
        }
    }

    private void CheckHoverHit() {
        Transform lastHoveredButtonBefore = lastHoveredButton;
        RaycastHit hit;
        Ray ray = mainMenuCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("MainMenu"));
        Transform h = hit.transform;
        for(int i = 0; i < 4; i++) {
            Renderer textRenderer = playerCountTexts[i].GetComponent<MeshRenderer>();
            if(h == playerCountBoxes[i]) {
                textRenderer.material.SetFloat("Boolean_hover", 1f);
                lastHoveredButton = h;
            }
            else textRenderer.material.SetFloat("Boolean_hover", 0f);
        }
        Renderer startBoxRenderer = startGameButtonBox.GetComponent<MeshRenderer>();
        Renderer startTextRenderer = startGameButtonText.GetComponent<MeshRenderer>();
        if(h == startGameButtonBox) {
            startBoxRenderer.material.SetFloat("Boolean_selected", 1f);
            startTextRenderer.material.SetFloat("Boolean_selected", 1f);
            lastHoveredButton = h;
        }
        else {
            startBoxRenderer.material.SetFloat("Boolean_selected", 0f);
            startTextRenderer.material.SetFloat("Boolean_selected", 0f);
        }
        if(lastHoveredButtonBefore != lastHoveredButton) OnHoverButton();
    }

    private void OnHoverButton() {
        audioManager.PlaySound("Blib2");
    }

    private void UpdateButtonStates(int playerCount) {
        for(int i = 0; i < 4; i++) {
            Renderer boxRenderer = playerCountBoxes[i].GetComponent<MeshRenderer>();
            Renderer textRenderer = playerCountTexts[i].GetComponent<MeshRenderer>();
            if((i + 1) == playerCount) {
                boxRenderer.material.SetFloat("Boolean_selected", 1f);
                textRenderer.material.SetFloat("Boolean_selected", 1f);
            }
            else {
                boxRenderer.material.SetFloat("Boolean_selected", 0f);
                textRenderer.material.SetFloat("Boolean_selected", 0f);
            }
        }
    }

    private void SetPlayerCount(int playerCount, bool playSound = true) {
        gameController.SetPlayerCount(playerCount);
        UpdateButtonStates(playerCount);
        if(playSound) audioManager.PlaySound("Blib3");
    }

    private void StartGame() {
        gameController.StartNewGame();
        audioManager.PlaySound("Wobbles1");
    }
}
