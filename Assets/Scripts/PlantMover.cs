using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMover : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D cc;

    public Vector3 clickStart; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //cc = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        
    }

    public void Move (Vector3 dragPosition)
    {
        Vector3 relativePosition = clickStart - dragPosition;
        rb.velocity = -relativePosition;
    }
}
