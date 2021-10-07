using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : MonoBehaviour
{

    private bool digMode = true;
    private bool stirMode = false;
    public LayerMask onlyCubes;
    public LayerMask cubesAndSolids;
    private float radius;

    private GameObject holding;

    private GameObject grabPoint; 

    void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;

        if (holding)
        {
            holding.transform.position = transform.position;
        }

        if (grabPoint)
        {
            grabPoint.GetComponent<PlantMover>().Move(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            digMode = !digMode;
            stirMode = !stirMode;
        }

        if (Input.GetMouseButton(0))
        {
            if (digMode)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, cubesAndSolids);

                foreach (Collider2D hit in hits)
                {
                    if (hit.GetComponent<Cube>())
                    {
                        Soil.soil.RemoveCube(hit.GetComponent<Cube>().cubeIndex);
                    }

                    if (hit.GetComponent<Seed>())
                    {
                        hit.GetComponent<Rigidbody2D>().Sleep();
                        holding = hit.gameObject;
                    }

                    if (hit.GetComponent<PlantMover>())
                    {
                        grabPoint = hit.gameObject;
                        grabPoint.GetComponent<PlantMover>().clickStart = transform.position;
                    }
                }
            }           
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (holding != null)
            {
                if (holding.GetComponent<Rigidbody2D>())
                {
                    holding.GetComponent<Rigidbody2D>().WakeUp();
                }

                holding = null;

            }

            if (grabPoint)
            {               
                grabPoint = null;
            }

            if (stirMode)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f, onlyCubes);

                //int[] mixCubes = new int[hits.Length];
                List<int> mixCubes = new List<int>();

                for (int m = 0; m < hits.Length; m++)
                {
                    mixCubes.Add(hits[m].GetComponent<Cube>().cubeIndex);
                }

                if (mixCubes.Count > 0) Soil.soil.MixCubes(mixCubes);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlantMover>())
        {
            grabPoint = collision.gameObject;
            grabPoint.GetComponent<PlantMover>().clickStart = transform.position;
        }
    }
}
