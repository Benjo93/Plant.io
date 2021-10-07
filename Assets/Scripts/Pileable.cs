using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pileable : MonoBehaviour
{
    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {

        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
        {
            Vector3 pos = collisionEvents[i].intersection;

            if (collisionEvents[i].colliderComponent.GetComponent<Pile>() != null)
            {
                collisionEvents[i].colliderComponent.GetComponent<Pile>().AddToStack(0.0004f);
            }

            i++;
        }
    }
}
