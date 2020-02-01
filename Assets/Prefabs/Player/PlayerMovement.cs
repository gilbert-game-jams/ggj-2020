using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3.5f;
     private float X;
     private float Y;
    
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
 
     void Update() {
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * speed, Input.GetAxis("Mouse X") * speed, 0));
        X = transform.rotation.eulerAngles.x;
        Y = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(X, Y, 0);
     }
}
