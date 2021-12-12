using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public void Update() {
        Camera currentCamera = Camera.current;
        if(currentCamera != null) transform.LookAt(currentCamera.transform.position);
    }

}
