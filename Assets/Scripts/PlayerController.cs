using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // the [SerializeField] makes the variable visible inside Unity inspector the same way as when using a public variable
    [SerializeField] private float ballRadius;
    [SerializeField] private float movementForce;
    [SerializeField] private float maxAngularVelocity;
    [SerializeField] private float jumpMaxDistanceToGround;
    [SerializeField] private float jumpImpulseForce;
    [SerializeField] private Transform playerCamera; 

    private Rigidbody ballRigidbody;
    private Collider ballCollider;
    private MeshRenderer meshRenderer;

    // TODO: private StateMachine powerUpStateMachine;

    private int playerNumber;
    private string inputPrefix;
    private Vector3 checkpoint;
    private Color defaultColor;
    private bool isFinished;

    private float powerUpTimeLeft;
    private string powerUpName;

    public void Awake() {
        ballRigidbody = GetComponent<Rigidbody>();
        ballRigidbody.maxAngularVelocity = maxAngularVelocity;
        ballCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        checkpoint = Vector3.up;
        // defaultColor = meshRenderer.material.color;
    }

    // this is called 50 times a second by Unity, everything related to physics should be done here instead of normal Update()
    public void FixedUpdate() {
        if(isFinished) return;

        HandlePlayerMovement();
        if(Input.GetButtonDown(inputPrefix + "Jump")) HandlePlayerJump();

        if(powerUpTimeLeft > 0f) {
            PowerUpUpdate();
            powerUpTimeLeft -= Time.fixedDeltaTime;
            if(powerUpTimeLeft <= 0f) PowerUpEnd();
        }
    }

    // this is called by Unity whenever the ball starts to touch some collider
    public void OnTriggerEnter(Collider other) {
        if(other.tag == "Checkpoint") OnCheckpointEnter(other.transform.position);
        else if(other.tag == "KillVolume") OnKillVolumeEnter();
        else if(other.tag == "FinishLine") OnFinishLineEnter();
        else if(other.tag == "SpeedBoost") OnSpeedBoostEnter(other);
        // add volume based power ups here
    }

    // this is called by Unity whenever the ball stops touching some collider
    public void OnTriggerExit(Collider other) {
        if(other.tag == "LiveVolume") OnLiveVolumeExit();
    }

    // this is called by GameController when spawning a player
    public void InitPlayer(int playerNumber, Material material) {
        this.playerNumber = playerNumber;
        inputPrefix = "Player" + playerNumber;
        meshRenderer.material = material;
        ballRigidbody.constraints = RigidbodyConstraints.FreezePosition;
    }

    // this is called by GameController when countdown is over and round starts
    public void Release() {
        ballRigidbody.constraints = RigidbodyConstraints.None;
    }

    // call this to apply a power up to the player
    public void AddPowerUp(string powerUpName, float duration) {
        if(powerUpTimeLeft > 0f) PowerUpEnd(); // if there's already an active power up, end it first
        this.powerUpName = powerUpName;
        powerUpTimeLeft = duration;
        PowerUpStart();
    }

    public bool IsFinished() {
        return isFinished;
    }

    private void HandlePlayerMovement() {
        // get horizontal camera forward direction
        Vector3 forwardDirection = playerCamera.forward;
        forwardDirection.y = 0f; // make it flat (horizontally)
        forwardDirection.Normalize(); // normalize so length is 1

        // quaternion rotation from global forward (positive z-axis) to forwardDirection
        Quaternion forwardRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);

        // player inputs as direction vector
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw(inputPrefix + "Vertical"), 0f, -Input.GetAxisRaw(inputPrefix + "Horizontal"));
        inputDirection.Normalize(); // normalized so the length is 1

        // calculate wanted torques by rotating inputDirection by forwardRotation and then multiplying with movementForce
        Vector3 torque = forwardRotation * inputDirection * movementForce;
        ballRigidbody.AddTorque(torque); // apply torque to rigidbody
    }

    private void HandlePlayerJump() {
        if(GetGroundDistanceDown() < jumpMaxDistanceToGround) {
            ballRigidbody.AddForce(Vector3.up * jumpImpulseForce, ForceMode.Impulse);
        }
    }

    // returns the distance to ground in down direction
    private float GetGroundDistanceDown() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 10f)) {
            return hit.distance - ballRadius;
        }
        return 10f;
    }

    private void OnCheckpointEnter(Vector3 checkpoint) {
        this.checkpoint = checkpoint;
    }

    private void OnKillVolumeEnter() {
        Respawn();
    }

    private void OnLiveVolumeExit() {
        Respawn();
    }

    private void OnFinishLineEnter() {
        isFinished = true;
        Destroy(ballRigidbody);
        Destroy(ballCollider);
    }

    private void OnSpeedBoostEnter(Collider other) {
        Vector3 direction = other.transform.forward;
        ballRigidbody.AddForce(direction * 40f, ForceMode.Impulse);
    }

    private void Respawn() {
        transform.position = checkpoint;
        ballRigidbody.velocity = Vector3.zero;
    }

    // this is called when a new power up starts
    private void PowerUpStart() {
        if(powerUpName == "SpeedUp") {
            meshRenderer.material.color = Color.blue;
            movementForce *= 2f;
        }
        else if(powerUpName == "SpeedDown") {
            meshRenderer.material.color = Color.red;
            movementForce /= 2f;
        }
    }

    // this is called every FixedUpdate when a power up is active
    private void PowerUpUpdate() {

    }

    // this is called when a power up ends
    private void PowerUpEnd() {
        if(powerUpName == "SpeedUp") {
            movementForce /= 2f;
        }
        else if(powerUpName == "SpeedDown") {
            movementForce *= 2f;
        }
        meshRenderer.material.color = defaultColor;
    }
}
