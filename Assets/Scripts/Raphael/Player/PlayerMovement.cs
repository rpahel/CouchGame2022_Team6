using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System;
using DG.Tweening;
using UnityEditor.Timeline;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //============================
    private PlayerManager playerManager;

    //============================
    [Header("Données publiques")]
    [SerializeField, Range(0, 40f)] private float vitesseMax;
    [SerializeField, Range(.1f, 2f)] private float dureeAvantArret;
    [SerializeField, Range(0, 70)] private int forceDeSaut;
    [SerializeField, Range(0, 30), Tooltip("Multiplicateur de la gravité, 1 = gravité de base d'Unity.")]
    private float echelleDeGravité;

    //============================
    private Vector2 inputVector = Vector2.zero;
    public Vector2 InputVector => inputVector;

    //============================
    private Rigidbody2D rb2d;
    private Collider2D coll;
    private Coroutine freinage;

    //============================ POUR LE SAUT (DETECTION DE SOL)
    private RaycastHit2D groundCheck;
    private float castRadius;
    private float castDistance;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        dureeAvantArret = dureeAvantArret < 0.1f ? 0.1f : dureeAvantArret;

        if (!TryGetComponent<PlayerManager>(out playerManager)) // ça c'est obligé pcq sinon playerManager == null;
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerManager component found in Player game object !");
        }

        playerManager.TryGetPlayerComponent<Rigidbody2D>(out rb2d);
        playerManager.TryGetPlayerComponent<Collider2D>(out coll);
        
        echelleDeGravité = echelleDeGravité != 0 ? echelleDeGravité : rb2d.gravityScale;

        castRadius = transform.localScale.x * .5f - .05f;
        castDistance = (coll as CapsuleCollider2D).size.y * transform.localScale.y * .25f + .3f;
    }

    private void Update()
    {
        if(dureeAvantArret < 0.1f)
            dureeAvantArret = 0.1f;
    }

    private void FixedUpdate()
    {
        rb2d.gravityScale = echelleDeGravité;

        groundCheck = Physics2D.CircleCast(transform.position, castRadius, Vector2.down, castDistance);

        if (groundCheck)
            playerManager.PlayerState = PLAYER_STATE.WALKING;
        else
            playerManager.PlayerState = PLAYER_STATE.FALLING;

        if (playerManager.PlayerState == PLAYER_STATE.WALKING) // Le player est en walking lorsqu'il est sur quelquechose
        {
            Deplacement();
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = groundCheck ? Color.cyan : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * castDistance, castRadius);
        }
    }
    #endif
    #endregion

    #region Custom_Functions
    public void OnMove(Vector2 input)
    {
        inputVector = input;

        if (inputVector == Vector2.zero)
            return;

        if(freinage != null)
        {
            StopCoroutine(freinage);
            freinage = null;
        }

        playerManager.PlayerState = PLAYER_STATE.WALKING;
    }

    public void OnJump()
    {
        if(groundCheck && (groundCheck.collider.gameObject.layer == 6 || groundCheck.collider.gameObject.layer == 7))
        {
            Jump();
        }
        else
        {
            Debug.Log("You can't jump, you're not on solid ground.");
        }
    }

    IEnumerator Freinage()
    {
        float iniVelocityX = rb2d.velocity.x;
        float t = 0;
        while(t < 1f)
        {
            rb2d.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), rb2d.velocity.y);
            t += Time.fixedDeltaTime / dureeAvantArret;
            yield return new WaitForFixedUpdate();
        }
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        freinage = null;
    }

    private void Deplacement()
    {
        if (inputVector == Vector2.zero)
        {
            if (freinage == null && rb2d.velocity.x != 0)
            {
                freinage = StartCoroutine(Freinage());
            }
        }

        if ((inputVector.x / Mathf.Abs(inputVector.x)) + (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x)) == 0)
            rb2d.velocity = new Vector2(-rb2d.velocity.x, rb2d.velocity.y);

        rb2d.velocity += new Vector2(inputVector.x, 0) * Time.fixedDeltaTime * 100f;
        rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -vitesseMax, vitesseMax), rb2d.velocity.y);
    }

    private void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.velocity += new Vector2(0, forceDeSaut);
    }
    #endregion
}
