using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

[ExecuteInEditMode]
public abstract class Cube : MonoBehaviour
{
    [SerializeField] private CUBETYPE cubeType;
    public CUBETYPE CubeType => cubeType;

    [SerializeField] protected GameObject cube;

    [HideInInspector] public Vector2 unscaledPosition;
    // Vérifie si ce cube a été mangé ou pas
    protected bool isManged;
    public bool IsManged => isManged;
}