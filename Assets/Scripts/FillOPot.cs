using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillOPot : MonoBehaviour
{

    public GameObject fillBlock;
    public GameObject bottom;

    public GameObject popTop;
    private GameObject thePopTop;

    public float[] layers;

    public List<Pile> piles = new List<Pile>();

    private int topLayer = 0;

    void Start()
    {

        thePopTop = Instantiate(popTop);
        thePopTop.transform.position = transform.position + Vector3.up * 0.5f;

        thePopTop.GetComponent<Renderer>().enabled = false;
        thePopTop.GetComponent<CircleCollider2D>().enabled = false;

        for (int i=0; i<layers.Length; i++)
        {
            GameObject theFillBlock = Instantiate(fillBlock);
            theFillBlock.transform.position = bottom.transform.position + new Vector3(0f, i * 0.05f, 0f);
            theFillBlock.transform.localScale = new Vector3(layers[i], theFillBlock.transform.localScale.y, theFillBlock.transform.localScale.z);

            if (i > 0)
            {
                theFillBlock.GetComponent<Renderer>().enabled = false;
                theFillBlock.GetComponent<BoxCollider2D>().enabled = false;
            }

            piles.Add(theFillBlock.GetComponent<Pile>());
        }
    }

    public void Update()
    {

        if (topLayer < piles.Count - 1)
        {
            if (piles[topLayer].stack >= 2.25f)
            {
                piles[topLayer + 1].GetComponent<Renderer>().enabled = true;
                piles[topLayer + 1].GetComponent<BoxCollider2D>().enabled = true;

                //piles[topLayer].GetComponent<Renderer>().enabled = false;
                piles[topLayer].GetComponent<BoxCollider2D>().enabled = false;

                topLayer++;
            }
        }
        else
        {
            thePopTop.GetComponent<Renderer>().enabled = true;
            thePopTop.GetComponent<CircleCollider2D>().enabled = true;

            thePopTop.transform.position = Vector3.Lerp(thePopTop.transform.position, thePopTop.transform.position + Vector3.up, 0f);
        }
    }

}
