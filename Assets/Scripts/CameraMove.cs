using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public float speed; 

    void Update()
    {

        transform.position = Vector3.Lerp(
            transform.position, 
            new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal"), transform.position.y, transform.position.z), 
            Time.deltaTime * speed);   
    }
}
