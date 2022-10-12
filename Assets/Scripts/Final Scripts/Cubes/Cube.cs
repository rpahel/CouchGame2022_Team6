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

    // V�rifie si ce cube a �t� mang� ou pas
    public bool isManged()
    {
        return !cube.activeSelf;
    }
}