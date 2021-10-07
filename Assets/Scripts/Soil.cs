using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour
{
    public static Soil soil;

    public int height;
    public int width;

    public static int uWidth;
    private Vector2 originPoint;

    public GameObject gridBlock;
    private GameObject[] grid;

    private Cube[] cubes;
    public Cube[] cubeTypes;
    public Cube[] rootTypes;

    private sbyte[] data;

    void Start()
    {

        soil = this;

        uWidth = width;

        // Layout the Grid Array.
        grid = new GameObject[height * width];
        originPoint = transform.position;

        // Layout Grid Blocks
        for (int g=0; g<grid.Length; g++)
        {
            grid[g] = Instantiate(gridBlock);
            grid[g].transform.parent = transform;
            grid[g].transform.position = originPoint + new Vector2(g % width * gridBlock.transform.localScale.x, (g - (g % width)) / width * gridBlock.transform.localScale.y);
            grid[g].GetComponent<GridBlock>().gridIndex = g;
        }

        // Create the Cube Array.
        cubes = new Cube[height * width];

        // Create the Data Array.
        data = new sbyte[height * width];

        // Create the borders. (-1)
        for (int d=0; d<data.Length; d++)
        {
            if (d % width == 0) data[d] = -1;
            if (d % width == width - 1) data[d] = -1;
            if (d <= width) data[d] = -1;
            if (d >= (height - 1) * width) data[d] = -1;
        }

        InvokeRepeating("UpdateSoil", 0.0f, 0.05f);
        InvokeRepeating("UpdateMoisture", 0.0f, 0.2f);

    }

    public Vector3 GetCubePosition(int index)
    {
        return grid[index].transform.position;
    }

    public void AddSolid (int index)
    {
        if (data[index] == 0)
        {
            data[index] = -1;
        }
    }

    public void RemoveSolid (int index)
    {
        if (data[index] == -1)
        {
            data[index] = 0;
        }
    }

    public void AddCube (int index)
    {
        if (index >= data.Length) return;

        if (data[index] == -1) index += width;

        if (index >= data.Length) return;

        if (data[index] == 0)
        {
            data[index] = (sbyte) SoilParticles.cubeType;

            cubes[index] = Instantiate(cubeTypes[SoilParticles.cubeType - 1]);
            cubes[index].transform.parent = transform;
            cubes[index].cubeType = SoilParticles.cubeType;
            cubes[index].transform.position = grid[index].transform.position;
            cubes[index].cubeIndex = index;
            cubes[index].moisture = 0;
            cubes[index].mineral = 1;
        }       
    }

    public void RemoveCube(int index)
    {
        if (index >= data.Length || index < 0) return;

        if (data[index] > 0)
        {
            data[index] = 0;
            Destroy(cubes[index].gameObject);
            cubes[index] = null;
        }
    }

    public void MixCubes(List<int> mixCubes)
    {

        for (int m=0; m<mixCubes.Count - 1; m++)
        {

            int mix = m + 1;
            //int mix = UnityEngine.Random.Range(0, mixCubes.Length);

            //if (m == mix) continue;

            //sbyte temp = data[mixCubes[m]];
            //data[mixCubes[m]] = data[mixCubes[mix]];
            //data[mixCubes[mix]] = temp;

            //Cube tempCube = cubes[mixCubes[m]];

            cubes[mixCubes[m]].cubeIndex = mixCubes[mix];
            //cubes[mixCubes[m]] = cubes[mixCubes[mix]];      

            //cubes[mixCubes[mix]] = tempCube;
            //cubes[mixCubes[mix]].cubeIndex = mixCubes[tempCube.cubeIndex];
            cubes[mixCubes[mix]].cubeIndex = mixCubes[m];

        }
    }

    public void AddWater(int index)
    {
        if (cubes[index] != null)
        {
            cubes[index].moisture += 1;
            if (cubes[index].moisture > 250) cubes[index].moisture = 250;
        }
    }

    public int[] absorbRoots(List<List<int>> roots)
    {
        int energy = 0;
        int water = 0; 

        foreach(List<int> root in roots)
        {
            foreach (int node in root)
            {
                if (cubes[node])
                {
                    if (cubes[node].moisture > 0)
                    {
                        water++;

                        if (cubes[node].mineral > 0)
                        {
                            cubes[node].moisture--;
                            cubes[node].mineral--;
                            energy++;
                        }
                    }                  
                }
            }
        }
        return new int[] { water, energy }; 
    }

    public int GrowRoot(int index)
    {
        int dir = UnityEngine.Random.Range(0, 3);

        switch (dir)
        {
            case 0:

                int below = index - uWidth;
                if (below >= 0)
                {
                    if (data[below] >= 0)
                    {
                        //data[below] = -2;
                        //Destroy(cubes[below]);
                        //cubes[below] = Instantiate(rootTypes[0]);
                        //cubes[below].transform.position = grid[below].transform.position;

                        return below;
                    }
                }
                break;

            case 1:

                int right = index + 1; 
                if (right >= 0)
                {
                    if (data[right] >= 0)
                    {
                        //data[right] = -2;
                        //Destroy(cubes[right]);
                        //cubes[right] = Instantiate(rootTypes[0]);
                        //cubes[right].transform.position = grid[right].transform.position;

                        return right;
                    }
                }
                break;

            case 2:

                int left = index - 1;
                if (left >= 0)
                {
                    if (data[left] >= 0)
                    {
                        //data[left] = -2;
                        //Destroy(cubes[left]);
                        //cubes[left] = Instantiate(rootTypes[0]);
                        //cubes[left].transform.position = grid[left].transform.position;

                        return left;
                    }
                }
                break;
        }
        return -1;
    }

    int multiplier = 1;
    private bool updateSoil = true;

    private void UpdateSoil()
    {
        if (!updateSoil) return;

        multiplier *= -1;

        // Could start checking only on the second row.
        for (int index = 0; index < data.Length; index++)
        {

            // Check if the cube is still active. If a cube is surrounded, deactivate the collider and set a switch that will 
            // cause this loop to ignore it.

            // Check if there is anything at that point.
            if (data[index] <= 0) continue;

            // Check if there is anything below the cube.
            int botIndex = index - width;
            if (botIndex >= 0)
            {
                // There is nothing below the cube.
                if (data[botIndex] == 0)
                {
                    // Swap the data elements.
                    data[botIndex] = data[index];
                    data[index] = 0;

                    // Swap the cube elements.
                    cubes[botIndex] = cubes[index];
                    cubes[index] = null;

                    cubes[botIndex].cubeIndex = botIndex;

                    continue;
                }
            }

            int sideIndex = index - width + multiplier;
            if (sideIndex >= 0)
            {
                if (data[sideIndex] == 0)
                {
                    // Swap the data elements.
                    data[sideIndex] = data[index];
                    data[index] = 0;

                    // Swap the cube elements.
                    cubes[sideIndex] = cubes[index];
                    cubes[index] = null;

                    cubes[sideIndex].cubeIndex = sideIndex;

                    continue;
                }
            }

            int otherSideIndex = sideIndex + (-2 * multiplier);
            if (otherSideIndex >= 0)
            {
                if (data[otherSideIndex] == 0)
                {
                    // Swap the data elements.
                    data[otherSideIndex] = data[index];
                    data[index] = 0;

                    // Swap the cube elements.
                    cubes[otherSideIndex] = cubes[index];
                    cubes[index] = null;

                    cubes[otherSideIndex].cubeIndex = otherSideIndex;

                    continue;
                }
            }
        }
    }

    public void UpdateMoisture()
    {
        for (int i=0; i<cubes.Length; i++)
        {
            if (cubes[i] != null)
            {

                int above = i + uWidth;
                if (above < cubes.Length)
                {
                    if (cubes[above] != null)
                    {
                        if (cubes[above].moisture > cubes[above].cubeType)
                        {
                            cubes[above].moisture -= 1;
                            cubes[i].moisture += 1;
                        }
                    }
                }
                cubes[i].UpdateColor();                      
            }          
        }
    }

    private void Update()
    {
        foreach (Cube cube in cubes)
        {
            if (cube)
            {
                cube.transform.position = Vector2.Lerp(cube.transform.position, grid[cube.cubeIndex].transform.position, Time.deltaTime * 10f);
            }
        }
    }
}
