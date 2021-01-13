using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZ : MonoBehaviour {
    
    public float rotationSpeed = 90f;

    void FixedUpdate() {
        
        transform.Rotate(0f, 0f, rotationSpeed * Time.fixedDeltaTime);
    }
}
