using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEat : MonoBehaviour
{
    #region Variables
    //============================
    private PlayerManager playerManager;

    //============================
    [SerializeField, Tooltip("La durée d'attente entre deux inputs de manger."), Range(0f, 1f)]
    private float eatCooldown;
    [SerializeField, Tooltip("Le nombre de cubes mangés par seconde."), Range(1, 5)]
    private int eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 5f)]
    private float eatDistance;
    //============================

    private float cooldown;
    #endregion

    #region Unity_Function
    private void Awake()
    {
        cooldown = eatCooldown;

        if (!TryGetComponent<PlayerManager>(out playerManager)) // ça c'est obligé pcq sinon playerManager == null;
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerManager component found in Player game object !");
        }
    }

    private void Update()
    {
        cooldown += Time.deltaTime;
        cooldown = Mathf.Clamp(cooldown, 0, eatCooldown);
    }
    #endregion

    #region Custom_Functions
    public void OnEat(Vector2 direction)
    {
        if (direction == Vector2.zero)
            direction = playerManager.SensDuRegard;

        if (cooldown < eatCooldown)
        {
            Debug.Log($"Attendez un peu avant de manger ({cooldown.ToString("0.00")} / {eatCooldown.ToString("0.00")})");
            return;
        }

        #if UNITY_EDITOR
            Debug.DrawRay(transform.position - Vector3.forward, direction.normalized * eatDistance, Color.red, eatCooldown);
        #endif

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, eatDistance, 1 << LayerMask.NameToLayer("Destructible"));
        if (hit)
        {
            hit.transform.parent.GetComponent<Cube_Edible>().GetManged(transform);
        }
        else
            Debug.Log("Rien n'a été touché");


        cooldown = 0;
    }
    #endregion
}
