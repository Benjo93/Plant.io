using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pile : MonoBehaviour
{

    public float stack = 0f;
    public bool canStack = true;

    public GameObject pileCube;

    public Pile leftPile;
    public Pile rightPile;

    public LayerMask everythingButMound;

    public void Start()
    {
        //transform.localScale = new Vector3(transform.localScale.x, 0.1f + stack, transform.localScale.z);
    }

    public void AddToStack(float mass)
    {
        stack += mass;       
    }

    public void AddLayer()
    {

    }

    public void BuildPile ()
    {
        //if (!Physics2D.Linecast(transform.position, transform.position + transform.localScale * 0.8f, everythingButMound))
        //{
            transform.localScale = new Vector3(transform.localScale.x, 0.1f + stack, transform.localScale.z);
        //}
    }
}
