using Data;
using UnityEngine;

public class PlayerSpecial : MonoBehaviour
{
    #region Autres Scripts
    //===================================================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Variables
    //===================================================
    [SerializeField, Range(1, 3), Tooltip("Durée en secondes avant d'atteindre le niveau de charge max.")]
    private float timeToMaxCharge;
    [SerializeField, Range(0, 40), Tooltip("Distance en mètres du special à son maximum.")]
    private float maxDistance;
    [SerializeField, Range(0, 40), Tooltip("Distance en mètres du special à son minimum (simple press du bouton).")]
    private float minDistance;

    //===================================================
    private float charge; // 0 à 1
    private bool isHolding;
    #endregion

    #region Unity_Functions
    //===================================================
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(maxDistance < minDistance) maxDistance = minDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
    #endif

    private void Update()
    {
        if(isHolding)
        {
            charge = Mathf.Clamp01(charge + Time.deltaTime / timeToMaxCharge);
            #if UNITY_EDITOR
            { 
                Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.y * Vector2.down,     (minDistance + charge * (maxDistance - minDistance)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.LookDirection), Color.magenta);
                Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.x * Vector2.right,    (minDistance + charge * (maxDistance - minDistance)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.LookDirection), Color.magenta);
                Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.x * Vector2.left,     (minDistance + charge * (maxDistance - minDistance)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.LookDirection), Color.magenta);
                Debug.DrawRay((Vector2)transform.position + PManager.PCollider.bounds.extents.y * Vector2.up,       (minDistance + charge * (maxDistance - minDistance)) * (PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.LookDirection), Color.magenta);
            }
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
        // TODO : REVEILLE TOI RAPHAEL ON FAIT FONCTIONNER SES NEURONES LA UN PEU HOP HOP HOP
    }
    #endregion
}
