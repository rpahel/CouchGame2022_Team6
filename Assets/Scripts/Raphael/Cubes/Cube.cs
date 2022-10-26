using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

[ExecuteInEditMode]
public abstract class Cube : MonoBehaviour
{
    #region Variables
    //==========================================
    [SerializeField] private CUBE_TYPE cubeType;
    public CUBE_TYPE CubeType => cubeType;

    //==========================================
    [SerializeField] protected GameObject cube;
    #endregion


    #region Custom_Functions
    // Vérifie si ce cube a été mangé ou pas
    public bool isManged()
    {
        return !cube.activeSelf;
    }
    #endregion
}