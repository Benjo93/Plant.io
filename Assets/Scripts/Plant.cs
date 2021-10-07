using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{ 
    public Material[] branchMaterialsAssign;
    public static Material[] branchMaterials;

    public void Start()
    {
        branchMaterials = branchMaterialsAssign;
    }
    
}
