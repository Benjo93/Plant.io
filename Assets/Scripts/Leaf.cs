using UnityEngine;

public class Leaf : MonoBehaviour
{
    private SpriteRenderer sr;

    public Vector3 direction { get; set; } 
    public Vector3 startPosition { get; set; }
    public bool isEndLeaf { get; set; } 
    public bool isHangingFruit { get; set; }
    public bool isEndFruit { get; set; }

    public Branch branch;
    public Sprite leaf;
    public float maxLeafSize; 
    private GameObject theLeaf;
    public Sprite bulb;
    private GameObject theBulb;
    public float maxBulbSize; 
    public Sprite flower;
    private GameObject theFlower;
    public float maxFlowerSize;
    private GameObject current; 

    public float maxSize;
    public float growthRate;

    public bool justLeaf;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = null;

        MakeLeaf();

        InvokeRepeating("Grow", 0.0f, 0.2f);
    }

    private void MakeLeaf()
    {
        if (current) Destroy(current);

        theLeaf = Instantiate(new GameObject(), transform.position, transform.rotation, transform);
        current = theLeaf;
        SpriteRenderer lsr = theLeaf.AddComponent<SpriteRenderer>();
        lsr.sprite = leaf;
        lsr.sortingOrder = 199;
        theLeaf.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }

    private void MakeBulb()
    {
        if (current) Destroy(current);

        theBulb = Instantiate(new GameObject(), transform.position, transform.rotation, transform);
        if (isHangingFruit) theBulb.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(0f, -1f, 0f));
        current = theBulb;

        SpriteRenderer bsr = theBulb.AddComponent<SpriteRenderer>();
        bsr.sprite = bulb;
        bsr.sortingOrder = 200;
        theBulb.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        branch.flowerCount++;
    }

    private void MakeFlower()
    {
        if (current) Destroy(current);

        theFlower = Instantiate(new GameObject(), transform.position, transform.rotation, transform);
        if (isHangingFruit) theFlower.transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(0f, -1f, 0f));
        current = theFlower;

        SpriteRenderer fsr = theFlower.AddComponent<SpriteRenderer>();
        fsr.sprite = flower;
        fsr.sortingOrder = 200;
        theFlower.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }

    private void Grow()
    {
        if (theLeaf) {

            // Grow the Leaf.
            if (theLeaf.transform.localScale.x <= maxLeafSize)
            {
                theLeaf.transform.localScale = new Vector3(
                    theLeaf.transform.localScale.x + growthRate,
                    theLeaf.transform.localScale.y + growthRate,
                    theLeaf.transform.localScale.z);
            }
            else // Leaf is fully grown.
            {
                if (!theBulb && current && !justLeaf)
                {
                    if (branch.flowerCount < branch.flowerFreq && Random.Range(0, 100) >= 99 || isEndFruit)
                    {
                        MakeBulb();
                    }
                }
            }
        }

        if (theBulb)
        {
            // Grow the Bulb.
            if (theBulb.transform.localScale.x <= maxBulbSize)
            {
                theBulb.transform.localScale = new Vector3(
                    theBulb.transform.localScale.x + growthRate,
                    theBulb.transform.localScale.y + growthRate,
                    theBulb.transform.localScale.z);
            }
            else
            {
                // Create a Flower.
                if (!theFlower && current)
                {
                    MakeFlower();                   
                }
            }
        }

        if (theFlower)
        {
            // Grow the flower.
            if (theFlower && theFlower.transform.localScale.x <= maxFlowerSize)
            {
                theFlower.transform.localScale = new Vector3(
                    theFlower.transform.localScale.x + growthRate,
                    theFlower.transform.localScale.y + growthRate,
                    theFlower.transform.localScale.z);
            }
        }

        // Check for fall.
        if (current && Random.Range(0, 1000) >= 999 && !isEndLeaf)
        {
            Fall(current);
            if (current == theFlower || current == theBulb) branch.flowerCount--;
            current = null;
        }

        // Get branch health to determine regen rate.
        if (!current && Random.Range(0, 1000) >= 500)
        {
            MakeLeaf();
        }
    }
         
    private void Fall(GameObject item)
    {
        CircleCollider2D cc = item.AddComponent<CircleCollider2D>();
        cc.radius = 0.1f;

        Rigidbody2D rb = item.AddComponent<Rigidbody2D>();
        rb.mass = 0.1f;
        rb.angularDrag = 10f;
        rb.gravityScale = 0.1f;

        SelfDestruct sd = item.AddComponent<SelfDestruct>();
        sd.seconds = 12f; 
    }
}
