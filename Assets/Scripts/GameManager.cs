using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool plantMode;
    public static bool PLANT_MODE; 
    public bool showRoots;
    public static bool SHOW_ROOTS;

    public float growthCycle;
    public static float GROWTH_CYCLE; 

    private void Start()
    {
        PLANT_MODE = plantMode;
        SHOW_ROOTS = showRoots;
        GROWTH_CYCLE = growthCycle; 
    }
}
