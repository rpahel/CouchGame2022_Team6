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
    [SerializeField, Range(1, 3), Tooltip("Dur�e en secondes avant d'atteindre le niveau de charge max.")]
    private float dureeAvantChargeMax;
    [SerializeField, Range(0, 40), Tooltip("Distance en m�tres du special � son maximum.")]
    private float distanceMin;
    [SerializeField, Range(0, 40), Tooltip("Distance en m�tres du special � son minimum (simple press du bouton).")]
    private float distanceMax;

    //===================================================
    private float charge; // 0 � 100
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
    #endregion

    #region Custom_Functions
    //===================================================
    public void OnCharge()
    {
        isHolding = true;
    }

    public void UseSpecial()
    {
        // TODO REVEILLE TOI
    }
    #endregion
}
