using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileGenerator : MonoBehaviour
{
    public int length;
    public GameObject pileCube;

    public List<GameObject> pileCubes = new List<GameObject>();
    public LayerMask everythingButMound;

    void Start()
    {
        for (int i = 1; i <= length; i++)
        {
            GameObject thePileCube = Instantiate(pileCube);
            thePileCube.transform.position = new Vector2(transform.position.x + i * (transform.localScale.x / 4f), transform.position.y);

            pileCubes.Add(thePileCube);
        }
    }

    public void Update()
    {
        List<Pile> piles = new List<Pile>();
        foreach (GameObject thePileCube in pileCubes) piles.Add(thePileCube.GetComponent<Pile>());

        Pile leftEdge = piles[0];
        Pile rightEdge = piles[piles.Count - 1];

        if (leftEdge.stack > 0.1f)
        {

            Vector3 leftEdgeMiddle = leftEdge.transform.position + new Vector3(0f, leftEdge.transform.localScale.y, 0f);

            if (!Physics2D.Linecast(leftEdgeMiddle, leftEdgeMiddle + Vector3.left * 0.05f, everythingButMound))
            {
                GameObject thePileCube = Instantiate(pileCube);
                thePileCube.transform.position = new Vector2(leftEdge.transform.position.x - (leftEdge.transform.localScale.x / 4f), transform.position.y);

                pileCubes.Insert(0, thePileCube);
            }
        }

        if (rightEdge.stack > 0.1f)
        {

            Vector3 rightEdgeMiddle = rightEdge.transform.position + new Vector3(0f, rightEdge.transform.localScale.y, 0f);

            if (!Physics2D.Linecast(rightEdgeMiddle, rightEdgeMiddle + Vector3.right * 0.05f, everythingButMound))
            {
                GameObject thePileCube = Instantiate(pileCube);
                thePileCube.transform.position = new Vector2(rightEdge.transform.position.x + (rightEdge.transform.localScale.x / 4f), transform.position.y);

                pileCubes.Add(thePileCube);
            }
        }

        //Forward Mound.

        for (int p=0; p<piles.Count - 1; p++)
        {
            if (piles[p].stack < piles[p+1].stack)
            {
                piles[p].stack += (piles[p + 1].stack - piles[p].stack) / 10f;
                piles[p + 1].stack -= (piles[p + 1].stack - piles[p].stack) / 10f;
            }

            //if (!Physics2D.Linecast(piles[p].transform.position, piles[p].transform.position + piles[p].transform.localScale * 0.8f, everythingButMound))
            //{
                piles[p].BuildPile();
            //}
            //else
            //{
                //pileCubes.RemoveRange(0, p);
            //}
        }

        // Reverse Mound.

        for (int p = piles.Count - 1; p > 0; p--)
        {
            if (piles[p].stack < piles[p - 1].stack)
            {
                piles[p].stack += (piles[p - 1].stack - piles[p].stack) / 10f;
                piles[p - 1].stack -= (piles[p - 1].stack - piles[p].stack) / 10f;
            }

            //if (!Physics2D.Linecast(piles[p].transform.position, piles[p].transform.position + piles[p].transform.localScale * 0.8f, everythingButMound))
            //{
                piles[p].BuildPile();
            //}
            //else
            //{
                //pileCubes.RemoveRange(p, pileCubes.Count-1);
            //}
        }
    }
}
