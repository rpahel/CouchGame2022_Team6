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

    //==========================================
    protected bool isManged;
    public bool IsManged => isManged;

    //==========================================
    [HideInInspector] public Vector2 unscaledPosition;
    #endregion
}