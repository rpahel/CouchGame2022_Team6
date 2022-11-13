using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Data;

public class PlayerMovement : MonoBehaviour
{
    #region Autres Scripts
    //============================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Variables
    //============================
    [Header("Données publiques")]
    [SerializeField, Range(0, 40f)]
    private float vitesseMax;
    [SerializeField, Range(.01f, .2f), Tooltip("Durée de freinage en seconde.")]
    private float dureeAvantArret;
    [SerializeField]
    private int forceDeSaut;
    [SerializeField, Range(0, 30), Tooltip("Multiplicateur de la gravité, 1 = gravité de base d'Unity.")]
    private float echelleDeGravité;
    [SerializeField, Range(0.01f, 1), Tooltip("Valeur à dépasser avec le joystick pour initier le déplacement.")]
    private float deadZone;

    //============================
    private Coroutine freinage;

    //============================ POUR LE SAUT (DETECTION DE SOL)
    private RaycastHit2D groundCheck;
    public RaycastHit2D GroundCheck => groundCheck;
    private float castRadius;
    private float castDistance;

    //============================
    private Vector2 inputVector_move = Vector2.zero;
    public Vector2 InputVector_move => inputVector_move;

    //============================
    private bool holdJump;
    public bool HoldJump { set => holdJump = value; }

    //============================
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        dureeAvantArret = dureeAvantArret < 0.01f ? 0.01f : dureeAvantArret;
        PManager.SensDuRegard = Vector2.right;
        castRadius = transform.localScale.x * .5f - .05f;
    }

    private void Start()
    {
        echelleDeGravité = echelleDeGravité != 0 ? echelleDeGravité : PManager.Rb2D.gravityScale;
        castDistance = (PManager.PCollider as CapsuleCollider2D).size.y * transform.localScale.y * .25f + .3f;
    }

    private void Update()
    {
        if(dureeAvantArret < 0.01f)
            dureeAvantArret = 0.01f;
    }

    private void FixedUpdate()
    {
        #if UNITY_EDITOR
            PManager.Rb2D.gravityScale = echelleDeGravité;
        #endif

        if(PManager.PlayerState != PLAYER_STATE.STUNNED)
        {
            castRadius = transform.localScale.x * .5f - .05f;
            castDistance = (PManager.PCollider as CapsuleCollider2D).size.y * transform.localScale.y * .25f + .3f;

            groundCheck = Physics2D.CircleCast(transform.position, castRadius, Vector2.down, castDistance);

            if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED)
            {
                if(PManager.PlayerState != PLAYER_STATE.SHOOTING)
                {
                    if (groundCheck)
                        PManager.PlayerState = PLAYER_STATE.WALKING;
                    else
                        PManager.PlayerState = PLAYER_STATE.FALLING;
                }

                Deplacement();

                if (holdJump && groundCheck)
                    OnJump();

                PManager.Rb2D.velocity = new Vector2(Mathf.Clamp(PManager.Rb2D.velocity.x, -vitesseMax, vitesseMax), PManager.Rb2D.velocity.y);
            }
            else
            {
                if (PManager.Rb2D.velocity.y <= 0 && groundCheck && PManager.PlayerState != PLAYER_STATE.SHOOTING)
                {
                    PManager.PlayerState = PLAYER_STATE.WALKING;
                }
            }
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
        if (Mathf.Abs(input.x) <= deadZone || PManager.PlayerState == PLAYER_STATE.SHOOTING)
        {
            inputVector_move = Vector2.zero;
            return;
        }
        
        inputVector_move = input;

        PManager.SensDuRegard = inputVector_move.x > 0 ? Vector2.right : Vector2.left;

        if(freinage != null)
        {
            StopCoroutine(freinage);
            freinage = null;
        }
    }

    public void OnJump()
    {
        if (PManager.PlayerState == PLAYER_STATE.KNOCKBACKED || PManager.PlayerState == PLAYER_STATE.SHOOTING)
            return;

        int testlayer = 0;
        if (groundCheck)
            testlayer = groundCheck.collider.gameObject.layer;

        if (groundCheck && (testlayer == LayerMask.NameToLayer("Destructible")
            || testlayer == LayerMask.NameToLayer("Indestructible")
            || testlayer == LayerMask.NameToLayer("Trap")))
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
        float iniVelocityX = PManager.Rb2D.velocity.x;
        float t = 0;
        while(t < 1f)
        {
            PManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), PManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / dureeAvantArret;
            yield return new WaitForFixedUpdate();
        }
        PManager.Rb2D.velocity = new Vector2(0, PManager.Rb2D.velocity.y);
        freinage = null;
    }

    private void Deplacement()
    {
        if (inputVector_move == Vector2.zero)
        {
            if (freinage == null && PManager.Rb2D.velocity.x != 0)
            {
                freinage = StartCoroutine(Freinage());
            }
        }

        if ((inputVector_move.x / Mathf.Abs(inputVector_move.x)) + (PManager.Rb2D.velocity.x / Mathf.Abs(PManager.Rb2D.velocity.x)) == 0)
            PManager.Rb2D.velocity = new Vector2(-PManager.Rb2D.velocity.x, PManager.Rb2D.velocity.y);

        PManager.Rb2D.velocity += new Vector2(inputVector_move.x, 0) * Time.fixedDeltaTime * 100f;
    }

    private void Jump()
    {
        PManager.Rb2D.velocity = new Vector2(PManager.Rb2D.velocity.x, 0);
        PManager.Rb2D.AddForce(Vector2.up * forceDeSaut, ForceMode2D.Impulse);
    }
    #endregion
}