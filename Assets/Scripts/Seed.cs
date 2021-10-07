using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private Rigidbody2D rb;
    public LayerMask onlyCubesAndGrids;
    public LayerMask rootsDetect; 

    private bool planting = true;
    private bool growing = false;

    // Roots.
    private List<List<int>> roots = new List<List<int>>();
    private List<GameObject> theRoots = new List<GameObject>();

    private Branch plant;

    public Material[] branch;
    public GameObject leaf;
    public GameObject flower; 

    // Size.
    public int[] plantSize;
    public int variation; 
    public Vector2[] startWidth;
    public Vector2[] growWidth;

    // Direction.
    public Vector2[] plantDirection;

    // Orienation.
    public float[] sway;
    public int[] jitter; 
    public float[] constraint; 
    public bool[] symmetrical;

    // Branches.
    public bool[] tile; 
    public int[] branchStart; 
    public int[] branchSpace;
    public bool fillSubBranches;

    public bool roundEnds;
    public float[] gravFactor; 

    // Leaves.
    public float leafAngle;
    public int leafSpace;
    public int[] leafDepth;
    public bool hasEndLeaf;
    public bool isEndFruit;
    public bool expandLeaves;

    // Flowers.
    public int flowerFreq;
    public bool fruitIsHanging;

    // Growth.
    public float growRate; 
    public int growSpeed;
    public int growPoint;
    public bool climbable;

    // Active attributes.
    private float growthCycle = GameManager.GROWTH_CYCLE;
    private float absorbCycle = 1f;
    private int water = 0;
    private int energy = 0;

    public Color color; 

    private bool plantReady = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        color = GetComponent<SpriteRenderer>().material.color;
    }

    void Update()
    {

        if (Mathf.Abs(rb.velocity.y) > 0) { plantReady = true; }

        if (planting)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 4f, onlyCubesAndGrids);

            if (hits.Length >= 20 || !GameManager.PLANT_MODE)
            //if (hits.Length >= 0)
            {
                if (rb.velocity.x == 0 && rb.velocity.y == 0)
                {                                 
                    if (!plantReady) return;             
                    rb.isKinematic = true;

                    if (GameManager.SHOW_ROOTS)
                    {
                        for (int r = 0; r <= 5; r++)
                        {
                            float rootLength = 0.5f;
                            float ran = Random.Range(-1f, 1f);
                            Vector3 rootDirection = new Vector3(ran, -1f + Mathf.Abs(ran), 0f);
                            RaycastHit2D[] solidHits = Physics2D.RaycastAll(transform.position, rootDirection, rootLength, rootsDetect);

                            Vector3 endPoint = transform.position + rootDirection * rootLength;

                            foreach (RaycastHit2D solidHit in solidHits)
                            {
                                if (!solidHit.transform.gameObject.GetComponent<Cube>())
                                {
                                    //Instantiate(testPoint, solidHit.point, Quaternion.identity);
                                    endPoint = solidHit.point;
                                }
                            }

                            RaycastHit2D[] cubeHits = Physics2D.LinecastAll(transform.position, endPoint, rootsDetect);
                            endPoint = endPoint - transform.position;

                            // Add a new root index list to absorb from later.
                            List<int> rootToAdd = new List<int>();

                            foreach (RaycastHit2D cubeHit in cubeHits)
                            {
                                if (cubeHit.transform.gameObject.GetComponent<Cube>())
                                {
                                    Cube theCube = cubeHit.transform.gameObject.GetComponent<Cube>();

                                    // Add the index of the cube hit.
                                    rootToAdd.Add(theCube.cubeIndex);
                                }
                            }

                            // Add the root index list. 
                            roots.Add(rootToAdd);

                            GameObject theRoot = Instantiate(new GameObject());
                            theRoot.name = "Root";
                            theRoot.transform.parent = transform;
                            theRoot.transform.position = transform.position;

                            // Add root object to update later. 
                            theRoots.Add(theRoot);

                            LineRenderer lr = theRoot.AddComponent<LineRenderer>();
                            lr.useWorldSpace = false;
                            lr.startWidth = 0.15f;
                            lr.endWidth = 0.15f;
                            lr.positionCount = 2;
                            lr.numCapVertices = 30;
                            lr.material = Plant.branchMaterials[0];

                            Vector3[] positions = new Vector3[2];
                            positions[0] = new Vector3(0f, 0f, -2);
                            positions[1] = new Vector3(endPoint.x, endPoint.y, -2);

                            lr.SetPositions(positions);

                        }
                    }
                    
                    // The Plant.
                    GameObject thePlant = new GameObject();
                    plant = thePlant.AddComponent<Branch>();
                    plant.branch = branch;

                    // Size.                    
                    plant.origin = transform.position;
                    plant.size = plantSize;
                    plant.variation = variation;
                    plant.startWidth = startWidth;
                    plant.growWidth = growWidth;
                    
                    // Direction.
                    plant.direction = plantDirection;
                    plant.constraint = constraint;
                    plant.subBranchDirection = 1;
                    plant.perpToBranch = new Vector2(0f, 1f);
                    plant.paraToBranch = new Vector2(1f, 0f);

                    plant.growDirection = new Vector2(0f, 1f);

                    // Branches.
                    plant.tile = tile;
                    plant.branchStart = branchStart;
                    plant.branchSpace = branchSpace;
                    plant.fillSubBranches = fillSubBranches;

                    plant.roundEnds = roundEnds;
                    plant.gravFactor = gravFactor;

                    // Leaves. 
                    plant.leaf = leaf;
                    plant.leafAngle = leafAngle;
                    plant.leafSpace = leafSpace;
                    plant.leafDepth = leafDepth;
                    plant.hasEndLeaf = hasEndLeaf;
                    plant.isEndFruit = isEndFruit;
                    plant.expandLeaves = expandLeaves;

                    // Flowers.
                    plant.flowerFreq = flowerFreq;
                    plant.fruitIsHanging = fruitIsHanging; 

                    // Orientation.
                    plant.sway = sway;
                    plant.jitter = jitter; 
                    plant.symmetrical = symmetrical;

                    // Growth.
                    plant.growRate = growRate;
                    plant.growSpeed = growSpeed;
                    plant.growPoint = growPoint;
                    plant.climbable = climbable;
                    plant.Create();

                    planting = false;
                    growing = true;
                }
            }
        }

        if (growing)
        {
            if (absorbCycle <= 0f)
            {
                absorbCycle = 0.25f; 

                int[] nutrients = Soil.soil.absorbRoots(roots);
                water += nutrients[0];
                energy += nutrients[1];
            }
            absorbCycle -= Time.deltaTime;

            if (growthCycle <= 0f)
            {
                growthCycle = GameManager.GROWTH_CYCLE;

                if (energy > 0)
                {
                    energy--;
                    plant.Grow();

                    if (GameManager.SHOW_ROOTS)
                    {
                        GrowRoots();
                    }
                }

                plant.Grow();

                if (GameManager.SHOW_ROOTS)
                {
                    GrowRoots();
                }
            }
            growthCycle -= Time.deltaTime;
        }
    }

    private void GrowRoots()
    {
        foreach (GameObject root in theRoots)
        {
            LineRenderer lr = root.GetComponent<LineRenderer>();
            Vector3 startPosition =  root.transform.position + lr.GetPosition(lr.positionCount - 1);

            float rootLength = 0.5f;
            float ran = Random.Range(-1f, 1f);
            Vector3 rootDirection = new Vector3(ran, -1f + Mathf.Abs(ran), 0f);
            RaycastHit2D[] solidHits = Physics2D.RaycastAll(startPosition, rootDirection, rootLength, rootsDetect);

            Vector3 endPoint = startPosition + rootDirection * rootLength;

            foreach (RaycastHit2D solidHit in solidHits)
            {
                if (!solidHit.transform.gameObject.GetComponent<Cube>())
                {
                    endPoint = solidHit.point;                    
                }
            }

            RaycastHit2D[] cubeHits = Physics2D.LinecastAll(startPosition, endPoint, rootsDetect);
            endPoint = endPoint - startPosition;

            // Add a new root index list to absorb from later.
            int index = theRoots.IndexOf(root);

            foreach (RaycastHit2D cubeHit in cubeHits)
            {
                if (cubeHit.transform.gameObject.GetComponent<Cube>())
                {
                    Cube theCube = cubeHit.transform.gameObject.GetComponent<Cube>();

                    // Add the index of the cube hit.
                    roots[index].Add(theCube.cubeIndex);
                }
            }

            //lr.startWidth += 0.005f;
            //lr.endWidth += 0.005f;

            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, lr.GetPosition(lr.positionCount - 2) + endPoint);
        }
    }
}
