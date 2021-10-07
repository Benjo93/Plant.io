using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilParticles : MonoBehaviour
{
    public GameObject shovel; 

    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;
    private ParticleSystem.ShapeModule shape;

    public LayerMask onlyGridBlocks;

    public static byte cubeType = 1;

    private bool emmitting; 

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        shape = ps.shape;

        collisionEvents = new List<ParticleCollisionEvent>();

        ps.Stop();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
        {
            // A particle is hitting a "non-cube" object.
            if (collisionEvents[i].colliderComponent.tag != "Cube")
            {
                // Get nearest grid block.
                Collider2D nearestBlock = Physics2D.OverlapCircle(collisionEvents[i].intersection, 0.1f, onlyGridBlocks);

                // There is a grid block nearby.
                if (nearestBlock != null)
                {
                    // Add a cube at the grid block index.
                    int index = nearestBlock.gameObject.GetComponent<GridBlock>().gridIndex;
                    Soil.soil.AddCube(index);
                }
            }
            // A particle is hitting a cube.
            else
            {
                if (collisionEvents[i].colliderComponent.GetComponent<Cube>())
                {
                    // Add a cube at the index above the hit cube.
                    int index = collisionEvents[i].colliderComponent.GetComponent<Cube>().cubeIndex + Soil.uWidth;
                    Soil.soil.AddCube(index);
                }
            }
            i++;
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ps.Play();
            emmitting = true;
        }

        if (Input.GetMouseButton(1))
        {
            //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shape.position = new Vector3(shovel.transform.position.x, 0f, -shovel.transform.position.y);
        }

        if (Input.GetMouseButtonUp(1))
        {
            ps.Stop();
            emmitting = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            cubeType++;
            if (cubeType == Soil.soil.cubeTypes.Length + 1) cubeType = 1;           
        }

        if (Input.GetKeyDown(KeyCode.D))
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
