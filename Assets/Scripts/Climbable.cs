using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour
{

    public GameObject top;
    public GameObject bot;

    public Vector2 direction;

    void Start()
    {
        direction = (top.transform.position - bot.transform.position).normalized; 
    }

    public Vector2 getDirection()
    {
        return (top.transform.position - bot.transform.position).normalized;
    }
}
