using Data;
using UnityEngine;

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
    protected bool isEaten;
    public bool IsEaten => isEaten;

    //==========================================
    [HideInInspector] public Vector2Int unscaledPosition;
    #endregion
}