using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour
{

    // Default Color.
    private Color dColor;

    public GameObject cloudStart;
    public GameObject cloudEnd;
    public GameObject cloud;
    private List<GameObject> clouds = new List<GameObject>(); 

    // Weather.
    public float temperature;
    public float humidity;
    public float wind;
    public static float WIND;
    public float rainFall;
    public static float RAINFALL; 

    // Time. 
    public float daySpeed; 
    private float currentTime = 0f; 
    private float d = -1f; 

    void Start()
    {
        WIND = wind;
        RAINFALL = rainFall;

        dColor = GetComponent<SpriteRenderer>().color;
        Camera.main.backgroundColor = dColor;

        InvokeRepeating("MakeCloud", 0.0f, 5f);

    }

    void Update()
    {
        WIND = wind;
        RAINFALL = rainFall;

        // Night and Day Cycle.
        //GetComponent<SpriteRenderer>().color = new Color(0f, (dColor.g / 2f) + currentTime * 0.3f, (dColor.b / 2f) + currentTime * 0.5f, dColor.a);
        Camera.main.backgroundColor = new Color(0f, (dColor.g / 2f) + currentTime * 0.3f, (dColor.b / 2f) + currentTime * 0.5f, dColor.a);

        // Time Cycle.
        if (currentTime < 0f || currentTime > 1f)
        {           
            if (currentTime < 0)
            {
                d = 1f; 
                currentTime = 0f;
            }
            if (currentTime > 1f)
            {
                d = -1f; 
                currentTime = 1f;
            }
        }
        else currentTime += Time.deltaTime * d * daySpeed;
    }

    private void MakeCloud()
    {
        float cloudSpan = -5f; 
        float distance = Random.Range(cloudSpan, 0f);

        Vector3 start = new Vector3(cloudStart.transform.position.x, cloudStart.transform.position.y + distance, cloudStart.transform.position.z);

        GameObject theCloud = Instantiate(cloud, start, Quaternion.identity);       

        Cloud cld = theCloud.GetComponent<Cloud>();

        // Set cloud destination.
        cld.destination = new Vector3(cloudEnd.transform.position.x, cloudEnd.transform.position.y + distance, cloudEnd.transform.position.z);

        cld.distance = distance / cloudSpan; 
        //cld.rain = rainFall; 
    }
}
