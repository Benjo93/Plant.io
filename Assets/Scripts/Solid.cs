using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GridBlock")
        {
            int index = collision.GetComponent<GridBlock>().gridIndex;
            Soil.soil.AddSolid(index);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "GridBlock")
        {
            int index = collision.GetComponent<GridBlock>().gridIndex;
            Soil.soil.RemoveSolid(index);
        }
    }
}
