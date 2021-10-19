using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTrigger : MonoBehaviour {

    [SerializeField] private string powerUpName;
    [SerializeField] private float powerUpDuration;

    public void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            PlayerController playerController = other.transform.GetComponent<PlayerController>();
            playerController.AddPowerUp(powerUpName, powerUpDuration);
        }
    }

}
