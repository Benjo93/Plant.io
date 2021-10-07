using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public Sprite seed; 

    public GameObject block;
    private GameObject[,] blocks = new GameObject[2, 8];
    public GameObject[] items = new GameObject[16]; 

    private bool open; 

    void Start()
    {
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                GameObject the_block = Instantiate(block, new Vector3(transform.position.x + row * 1.5f, transform.position.y - col * 1.5f, 0f), Quaternion.identity);
                //the_block.GetComponent<SpriteRenderer>().material.color = Color.clear;
                blocks[row, col] = the_block;
                the_block.transform.parent = transform;

                if (items[(row * 8) + col])
                {
                    //GameObject the_item = Instantiate(items[(row * 8) + col], blocks[row, col].transform.position, Quaternion.identity);
                    GameObject the_item = Instantiate(new GameObject(), blocks[row, col].transform.position, Quaternion.identity);
                    the_item.AddComponent<SpriteRenderer>().sprite = seed; 
                    the_item.GetComponent<SpriteRenderer>().color = items[(row * 8) + col].GetComponent<Seed>().color; 
                    //the_item.GetComponent<SpriteRenderer>().material.color = Color.clear;
                }
            }
        }

        foreach (GameObject block in blocks) block.GetComponent<SpriteRenderer>().material.color = Color.clear;     
    }

    public void ToggleMenu()
    {
        if (!open)
        {
            foreach (GameObject block in blocks) block.GetComponent<SpriteRenderer>().material.color = Color.white;
            //foreach (GameObject item in items) if (item) item.GetComponent<SpriteRenderer>().material.color = Color.white;
        }
        if (open)
        {
            foreach (GameObject block in blocks) block.GetComponent<SpriteRenderer>().material.color = Color.clear;
            //foreach (GameObject item in items) if (item) item.GetComponent<SpriteRenderer>().material.color = Color.clear;
        }

        open = !open;
    }

    public void UpdateItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            //GameObject the_item = Instantiate(items[i], blocks[i].transform.position, Quaternion.identity);
        }
    }
}
