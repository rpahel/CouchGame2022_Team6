using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class PlayerSpecial : MonoBehaviour
{
    #region Autres Scripts
    //===================================================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Variables
    //===================================================
    [SerializeField, Range(1, 3), Tooltip("Durée en secondes avant d'atteindre le niveau de charge max.")]
    private float dureeAvantChargeMax;
    [SerializeField, Range(0, 40), Tooltip("Distance en mètres du special à son maximum.")]
    private float distanceMax;
    [SerializeField, Range(0, 40), Tooltip("Distance en mètres du special à son minimum (simple press du bouton).")]
    private float distanceMin;

    //===================================================
    private float charge; // 0 à 1
    private bool isHolding;
    #endregion

    #region Unity_Functions
    //===================================================
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(distanceMax < distanceMin) distanceMax = distanceMin;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceMax);
    }
#endif

    private void Update()
    {
        if(isHolding)
        {
            charge += Time.deltaTime / dureeAvantChargeMax;
            charge = Mathf.Clamp01(charge);
            #if UNITY_EDITOR
            Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.y * Vector2.down,     (distanceMin + charge * (distanceMax - distanceMin)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.SensDuRegard), Color.magenta);
            Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.x * Vector2.right,    (distanceMin + charge * (distanceMax - distanceMin)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.SensDuRegard), Color.magenta);
            Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.x * Vector2.left,     (distanceMin + charge * (distanceMax - distanceMin)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.SensDuRegard), Color.magenta);
            Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.y * Vector2.up,       (distanceMin + charge * (distanceMax - distanceMin)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.SensDuRegard), Color.magenta);
            #endif
        }
    }
    #endregion

    #region Custom_Functions
    //===================================================
    public void Charge(bool state = true)
    {
        if (state)
        {
            if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED)
                PManager.PlayerState = PLAYER_STATE.SHOOTING;
        }
        else
        {
            charge = 0;

            if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED)
                PManager.PlayerState = PLAYER_STATE.WALKING;
        }

        isHolding = state;
    }

    public void UseSpecial()
    {
        // TODO REVEILLE TOI
    }
    #endregion
}
