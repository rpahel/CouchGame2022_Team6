using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEat : MonoBehaviour
{
    #region Variables
    //============================
    [SerializeField, Tooltip("La durée d'attente entre deux inputs de manger."), Range(0f, 1f)]
    private float eatCooldown;
    [SerializeField, Tooltip("Le nombre de cubes mangés par seconde."), Range(1, 5)]
    private int eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 3f)]
    private float eatDistance;
    //============================
    #endregion

    #region Custom_Functions
    public void OnEat(Vector2 direction)
    {
        
    }
    #endregion
}
