using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public static int count = 0;

    public LineRenderer lr;

    public Material[] branch; 
    public GameObject leaf;

    public int depth = 0;
    public Vector3 origin { get; set; }
    private float segmentSize = 0.25f;

    // Size.
    public int[] size { get; set; }
    public int variation { get; set; }
    private int vary = 0; 
    public Vector2[] startWidth { get; set; }
    public Vector2[] growWidth { get; set; }

    // Orientation.
    public int[] jitter { get; set; }
    public float[] sway { get; set; }
    private Vector2 currentSway;

    public bool[] symmetrical { get; set; } 

    // Direction.
    public Vector2[] direction { get; set; }
    private Vector3 finalDirection;
    public Vector3 growDirection { get; set; } 
    public Vector3 startingDirection { get; set; }
    private Vector3 segment;
    public float[] constraint; 
    public Vector2 perpToBranch;
    public Vector2 paraToBranch;
    public int subBranchDirection { get; set; }
    private int branchDirection { get; set; }

    // Branches.
    public List<Branch> subBranches = new List<Branch>();
    public bool[] tile { get; set; }
    public int[] branchStart { get; set; }
    private int branchIndex;
    public int[] branchSpace { get; set; }
    private int currentSpace = 0;
    public bool fillSubBranches;

    public bool roundEnds { get; set; }
    public float[] gravFactor { get; set; }
    private float gravSum = 0f;

    // Leaves.
    private List<GameObject> leaves = new List<GameObject>();
    public int leafSpace { get; set; }
    public float leafAngle { get; set; }   
    private int leafDirection = 1;
    public int[] leafDepth;
    public bool hasEndLeaf { get; set; }
    public bool isEndFruit { get; set; }
    public bool expandLeaves { get; set; }

    // Flowers.
    public int flowerFreq { get; set; }
    public int flowerCount = 0;

    // Growth.
    public float growFactor;
    public float growRate; 
    public int growSpeed { get; set; }
    public int growPoint { get; set; }
    private int growIndex = 0;
    public bool climbable;
    public bool fruitIsHanging;

    public void Create()
    {
        vary = Random.Range(0, variation);
        transform.name = "Branch " + count + ", depth=" + depth;
        branchIndex = branchStart[depth];
        branchDirection = subBranchDirection;
        // TODO make starting point relative to current width.
        transform.position = origin;

        startingDirection = growDirection;

        // Initialize the sway. 
        currentSway = new Vector2(
            Random.Range(-sway[depth], sway[depth]), 
            Random.Range(-sway[depth], sway[depth]));

        // Initialize the line renderer.
        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.startWidth = startWidth[depth].x;
        lr.endWidth = startWidth[depth].y;
        lr.positionCount = 2;
        lr.numCapVertices = roundEnds ? 10 : 0;
        lr.textureMode = tile[depth] ? LineTextureMode.Tile : LineTextureMode.Stretch; 
        lr.material = branch[depth];
        lr.sortingOrder = depth;

        Vector3[] position = new Vector3[2];
        position[0] = new Vector3(0, 0, 0);
        //position[1] = new Vector3(0, 0, 0);
        position[1] = growDirection * segmentSize;

        lr.SetPositions(position);

        count++;
    }

    public void Grow()
    {
        if (lr.positionCount <= size[depth] + vary)
        {
            // Get current segment position.
            segment = new Vector3(lr.GetPosition(lr.positionCount - 1).x, lr.GetPosition(lr.positionCount - 1).y, 0f);

            // Calculate the sway factor.
            //float x = direction[depth].x + currentSway.x;
            //float y = direction[depth].y + currentSway.y;

            // Get a new sway.
            //if (Random.Range(0, 10) >= 8) currentSway = new Vector2(Random.Range(-sway[depth], sway[depth]), Random.Range(0f, sway[depth]));

            /*
            // Calculate the relative x and y vector. 
            Vector2 xPos = paraToBranch * x * gravSum;
            Vector2 yPos = perpToBranch * y * branchDirection;
            gravSum -= gravFactor[depth];
            finalDirection = xPos + yPos;
            finalDirection = finalDirection.normalized;
            finalDirection = finalDirection * segmentSize;
            */

            // Get the new direction of the branch.
            growDirection = (lr.GetPosition(lr.positionCount - 1) - lr.GetPosition(lr.positionCount - 2)).normalized;

            // Calculate the modifiers. 
            Vector3 swayVector = new Vector3(currentSway.x, currentSway.y, 0f);
            growDirection += swayVector;

            //gravSum = gravSum <= -1 ? -1 : gravSum - gravFactor[depth];
            gravSum -= Random.Range(0f, gravFactor[depth]);
            Vector3 gravityVector = new Vector3(0f, gravSum, 0f);

            // Set the new sway value.
            if (Random.Range(0, 10) >= (10-jitter[depth])) currentSway = new Vector2(Random.Range(-sway[depth], sway[depth]), Random.Range(-sway[depth], sway[depth]));

            //Debug.DrawLine(transform.position + segment, transform.position + segment + growDirection, Color.red);
            //Debug.DrawLine(transform.position + segment, transform.position + segment + startingDirection, Color.green);

            // Constrain the new point.
            //growDirection = ((startingDirection * constraint[depth] + growDirection) / 2f).normalized; 

            Vector3 influenceVector = startingDirection;

            // Stake.
            if (climbable)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + segment, 0.5f);
                foreach (Collider2D hit in hits)
                {
                    if (hit.tag == "Climbable")
                    {
                        Climbable climb = hit.GetComponent<Climbable>();
                        Debug.DrawLine(transform.position + segment, transform.position + segment + (Vector3)climb.direction, Color.magenta);
                        influenceVector = climb.direction;
                        gravSum = 0f; 
                    }
                }
            }

            growDirection = ((influenceVector * constraint[depth] + growDirection) / 2f).normalized; 

            //Debug.DrawLine(transform.position + segment, transform.position + segment + growDirection, Color.yellow);

            growDirection += gravityVector;
            
            // Set the new direction. 
            //finalDirection = (growDirection + swayVector + gravityVector) * segmentSize;
            finalDirection = growDirection.normalized * segmentSize;

            // Add the new point.
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, segment + finalDirection);

            // Update the branch width.
            lr.startWidth += growWidth[depth].x;
            lr.endWidth += growWidth[depth].y;

            // Leaf Action.
            if (depth >= leafDepth[0] && depth <= leafDepth[1])
            {              
                if (lr.positionCount != size[depth] + vary)
                {
                    if (lr.positionCount % leafSpace == 0)
                    {
                        // Add side leaf.
                        GameObject theLeaf = Instantiate(leaf);
                        theLeaf.name = "Side Leaf " + depth;
                        theLeaf.transform.parent = transform;
                        theLeaf.GetComponent<Leaf>().branch = this;
                        if (fruitIsHanging) theLeaf.GetComponent<Leaf>().isHangingFruit = true;
                        theLeaf.GetComponent<Leaf>().justLeaf = isEndFruit;
                        theLeaf.transform.rotation = Quaternion.LookRotation(Vector3.forward, finalDirection);
                        theLeaf.transform.Rotate(0f, 0f, leafAngle * leafDirection);                                         
                        theLeaf.GetComponent<Leaf>().direction = theLeaf.transform.up.normalized;
                        theLeaf.transform.position = transform.position + lr.GetPosition(lr.positionCount - 1);
                        theLeaf.GetComponent<Leaf>().startPosition = theLeaf.transform.position - transform.position;
                        leaves.Add(theLeaf);

                        leafDirection *= -1;
                    }
                }               
                else
                {
                    if (hasEndLeaf)
                    {
                        // Add ending leaf.                 
                        GameObject theLeaf = Instantiate(leaf);
                        theLeaf.name = "End Leaf " + depth;
                        theLeaf.transform.parent = transform;
                        theLeaf.GetComponent<Leaf>().branch = this;
                        if (isEndFruit)
                        {
                            //Debug.Log(lr.positionCount + ", " + (size[depth] + variation));
                            float currentFlowerSize = theLeaf.GetComponent<Leaf>().maxFlowerSize; 
                            theLeaf.GetComponent<Leaf>().maxFlowerSize = lr.positionCount / (float) (size[depth] + variation) * currentFlowerSize;
                            theLeaf.GetComponent<Leaf>().isEndFruit = true;
                        }
                        if (fruitIsHanging) theLeaf.GetComponent<Leaf>().isHangingFruit = true;
                        theLeaf.transform.position = transform.position + lr.GetPosition(lr.positionCount - 1) + finalDirection;
                        theLeaf.transform.rotation = Quaternion.LookRotation(Vector3.forward, finalDirection);
                        theLeaf.GetComponent<Leaf>().isEndLeaf = true;
                        leaves.Add(theLeaf);
                    }
                }
            }

            if (expandLeaves)
            {
                // Expand the leaves along the width.
                foreach (GameObject leaf in leaves)
                {
                    if (!leaf.GetComponent<Leaf>().isEndLeaf)
                    {
                        leaf.transform.position =
                            leaf.GetComponent<Leaf>().branch.transform.position
                            + leaf.GetComponent<Leaf>().startPosition
                            + leaf.GetComponent<Leaf>().direction * (lr.startWidth * 0.3f);
                    }
                }
            }

            // Branch Action.
            if (branchIndex <= 0)
            {
                if (currentSpace <= 0)
                {
                    // Create branch. 
                    Split();

                    // If symmetrical, add another opposing branch. 
                    if (symmetrical[depth]) Split();

                    currentSpace = branchSpace[depth] + Random.Range(-branchSpace[depth] / 3, branchSpace[depth] / 3);
                }
                else currentSpace--;
            }
            else branchIndex--;        

            // Generate 'grow-speed' segments per 'grow-point'
            // Plants with branches that only grow proportionally to the growth of the plant.
            if (!fillSubBranches)
            {               
                if (growIndex >= growPoint)
                {
                    // Grow every 'grow-speed' amount..
                    for (int i = 0; i < growSpeed; i++)
                    {
                        // Grow all sub branches.
                        foreach (Branch subBranch in subBranches) subBranch.Grow();                      
                    }
                    growIndex = 0;
                }
                else growIndex++;
            }

            // Expand the current points to the new growth points.
            foreach (Branch subBranch in subBranches) Expand(subBranch, finalDirection, subBranch.growFactor * growRate);
        }

        // Plants that require all branches to grow out regardless of the growth of the plant.
        if (fillSubBranches)
        {
            // Grow all sub branches.
            foreach (Branch subBranch in subBranches) subBranch.Grow();      
        }      
    }

    public void Expand(Branch subBranch, Vector3 finalDirection, float growth)
    {

        //subBranch.transform.position += finalDirection * growth;
        //subBranch.transform.position += (Vector3) subBranch.paraToBranch * growth * 0.25f;
        //subBranch.transform.position += subBranch.startingDirection * growth * 0.25f;
        subBranch.transform.position += finalDirection * growth * 0.25f;
        //subBranch.transform.position += subBranch.finalDirection * growWidth[depth].x;

        //for (int p = 0; p < subBranch.lr.positionCount; p++)
        //{
        // TODO expand the branch to in the direction of the branches closest index minus the previous index. 
        //subBranch.lr.SetPosition(p, subBranch.lr.GetPosition(p) + finalDirection * growth);            
        //}

        // Move all leaves.
        //foreach (GameObject leaf in leaves) leaf.transform.position += finalDirection * growth;
        //foreach (GameObject leaf in subBranch.leaves)  leaf.transform.position += finalDirection * growth;

        // Expand all sub branches.
        //foreach (Branch br in subBranch.subBranches) subBranch.Expand(br, finalDirection, growth);      
    }

    public void Split()
    {
        // End condition.
        if (depth + 1 >= size.Length) return;

        // Generate new sub branch.
        GameObject subBranch = new GameObject();
        subBranch.transform.parent = transform;
        Branch br = subBranch.AddComponent<Branch>();

        // Get the current normalized parallel and perpendicular direction. 
        Vector2 branchDirection = lr.GetPosition(lr.positionCount - 1) - lr.GetPosition(lr.positionCount - 2);
        br.perpToBranch = Vector2.Perpendicular(branchDirection).normalized;
        br.paraToBranch = branchDirection.normalized;

        // Calculate the initial direction of the sub branch.
        Vector2 xPos = Vector2.Perpendicular(branchDirection).normalized * subBranchDirection * direction[depth + 1].y;
        Vector2 yPos = branchDirection.normalized * direction[depth + 1].x;

        br.growDirection = (xPos + yPos).normalized;
        br.constraint = constraint;

        // Branch.
        br.branch = branch;
        br.tile = tile;
        br.roundEnds = roundEnds;
        br.gravFactor = gravFactor;

        // Leaf.
        br.leaf = leaf;
        br.leafAngle = leafAngle;
        br.leafSpace = leafSpace;
        br.leafDepth = leafDepth;
        br.hasEndLeaf = hasEndLeaf;
        br.isEndFruit = isEndFruit;
        br.expandLeaves = expandLeaves;

        // Flower.
        br.flowerFreq = flowerFreq;
        br.fruitIsHanging = fruitIsHanging;

        // Size
        br.size = size;
        br.variation = variation;
        br.startWidth = startWidth;
        br.growWidth = growWidth;

        // Direction.
        br.direction = direction;
        br.branchStart = branchStart;
        br.branchSpace = branchSpace;
        br.subBranchDirection = subBranchDirection;

        // Orientation.
        br.sway = sway;
        br.jitter = jitter;
        br.symmetrical = symmetrical;
        br.origin = origin + lr.GetPosition(lr.positionCount - 1);
        
        // Global. 
        br.depth = depth + 1;

        // Growth. 
        br.growFactor = 1f - (1f / lr.positionCount);
        br.growRate = growRate;
        br.growSpeed = growSpeed;
        br.fillSubBranches = fillSubBranches;
        br.climbable = climbable;

        br.Create();

        subBranches.Add(br);
        subBranchDirection *= -1;
    }
}
