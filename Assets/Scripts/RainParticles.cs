using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainParticles : MonoBehaviour
{
    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;

    private bool emmitting;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

        //ps.Stop();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Cube")
        {
            if (other.GetComponent<Cube>())
            {
                int index = other.GetComponent<Cube>().cubeIndex;
                Soil.soil.AddWater(index);
            }
        }       
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (emmitting)
            {
                ps.Stop();
                emmitting = false;
            }
            else
            {
                ps.Play();
                emmitting = true;
            }
        }
    }
}
