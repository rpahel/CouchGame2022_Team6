using Data;
using System;
using System.Collections;
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
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private float dashForce;
    [SerializeField]
    private GameObject specialTrigger;

    //===================================================
    private float charge; // 0 à 1
    private bool isHolding;
    private bool canDash = true;
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

    private void FixedUpdate()
    {
        if (PManager.PlayerState == PLAYER_STATE.DASHING)
            PManager.Rb2D.AddForce(PManager.AimDirection * dashForce, ForceMode2D.Impulse);
    }
    #endregion

    #region Custom_Functions
    //===================================================
    public void Charge(bool state = true)
    {
        if (state)
        {
            if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED && PManager.PlayerState != PLAYER_STATE.DASHING)
                PManager.PlayerState = PLAYER_STATE.SHOOTING;
        }
        else
        {
            charge = 0;

            if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED && PManager.PlayerState != PLAYER_STATE.DASHING)
                PManager.PlayerState = PLAYER_STATE.WALKING;
        }

        isHolding = state;
    }

    public void UseSpecial()
    {
        if (PManager.PlayerState == PLAYER_STATE.STUNNED || PManager.PlayerState == PLAYER_STATE.DASHING)
            return;

        if (!canDash)
        {
            Debug.Log("Wait for dash cooldown.");
            return;
        }

        StartCoroutine(DashCoroutine(charge + .1f));
    }
    private IEnumerator DashCoroutine(float dashDuration)
    {
        PManager.PlayerState = PLAYER_STATE.DASHING;
        canDash = false;
        gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
        specialTrigger.SetActive(true);
        var originalGravityScale = PManager.Rb2D.gravityScale;
        PManager.Rb2D.gravityScale = 0;
        PManager.Rb2D.velocity = Vector2.zero;
        //_trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDuration);
        PManager.PlayerState = PLAYER_STATE.WALKING;
        PManager.Rb2D.gravityScale = originalGravityScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        specialTrigger.SetActive(false);
        //_trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion
}
