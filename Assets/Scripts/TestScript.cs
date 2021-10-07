using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public GameObject target;

    void Update()
    {

        Debug.Log(transform.forward);

        //RaycastHit2D climbPoint = Physics2D.Linecast(transform.position, transform.position + Vector3.right * 5f);

        //Debug.DrawLine(transform.position, climbPoint.point, Color.green);
        //Debug.DrawLine(climbPoint.point, climbPoint.normal, Color.green);

        //Vector3 relPos = target.transform.position - transform.position;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, relPos);
    }
}
