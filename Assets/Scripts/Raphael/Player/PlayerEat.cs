using System;
using UnityEngine;

public class PlayerEat : MonoBehaviour
{
    #region Autres Scripts
    //============================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Variables
    //============================
    [SerializeField, Range(0, 100), Tooltip("La quantité de nourriture dans le corps.")]
    private int remplissage;
    public int Remplissage { get => remplissage; set => remplissage = value; }

    //============================
    [SerializeField, Tooltip("La durée d'attente entre deux inputs de manger."), Range(0f, 1f)]
    private float eatCooldown;
    [SerializeField, Tooltip("Le nombre de cubes mangés par seconde."), Range(2, 10)]
    private int eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 5f)]
    private float eatDistance;
    [SerializeField, Tooltip("Combien de nourriture tu reçois en mangeant un cube. 100 = Un cube suffit à te remplir."), Range(0f, 100f)]
    private int tauxRemplissage;

    //============================
    private float cooldown;
    private float tickHoldEat;

    //============================
    private bool holdEat;
    public bool HoldEat { set => holdEat = value; }
    #endregion

    #region Unity_Function
    private void Awake()
    {
        cooldown = eatCooldown;
        tickHoldEat = 1f;
    }

    private void Update()
    {
        cooldown += Time.deltaTime;
        cooldown = Mathf.Clamp(cooldown, 0, eatCooldown);

        if (holdEat)
        {
            if (remplissage <= 100)
            {
                if (tickHoldEat >= 1f)
                {
                    OnEat(PManager.AimDirection);
                    tickHoldEat = 0f;
                }
                else
                {
                    tickHoldEat += Time.deltaTime * eatTickrate;
                }
            }
        }
    }
    #endregion

    #region Custom_Functions
    public void OnEat(Vector2 direction)
    {
        if (cooldown < eatCooldown)
        {
            Debug.Log($"Attendez un peu avant de manger ({cooldown.ToString("0.00")} / {eatCooldown.ToString("0.00")})");
            return;
        }

        if(remplissage >= 100)
        {
            Debug.Log("Tu es plein et ne peut donc plus manger! Vomis.");
            return;
        }

        if (direction == Vector2.zero)
        {
            if(!(PManager.PMovement.GroundCheck))
                direction = Vector2.up;
            else
                direction = PManager.PMovement.SensDuRegard;
        }

        #if UNITY_EDITOR
            Debug.DrawRay(transform.position - Vector3.forward, direction.normalized * eatDistance, Color.red, eatCooldown);
        #endif

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, eatDistance, 1 << LayerMask.NameToLayer("Destructible"));
        if (hit)
        {
            hit.transform.parent.GetComponent<Cube_Edible>().GetManged(transform);
            remplissage += tauxRemplissage;
            remplissage = Mathf.Clamp(remplissage, 0, 100);
        }
        else if(!(PManager.PMovement.GroundCheck))
        {
            direction = PManager.PMovement.SensDuRegard;
            hit = Physics2D.Raycast(transform.position, direction.normalized, eatDistance, 1 << LayerMask.NameToLayer("Destructible"));

            if (hit)
            {
                hit.transform.parent.GetComponent<Cube_Edible>().GetManged(transform);
                remplissage += tauxRemplissage;
                remplissage = Mathf.Clamp(remplissage, 0, 100);
            }
        }

        cooldown = 0;
    }
    #endregion
}
