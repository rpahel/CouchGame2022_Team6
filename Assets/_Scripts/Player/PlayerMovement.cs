using Data;
using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Autres Scripts
    //==========================================================================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Variables
    //==========================================================================
    [SerializeField, Range(0, 40f)]
    private float maxSpeed;
    [SerializeField, Range(.01f, .2f), Tooltip("Durée de freinage en seconde.")]
    private float stopDuration;
    [SerializeField, Tooltip("La vitesse initial du joueur quand il saute.")]
    private int jumpForce;
    [SerializeField, Tooltip("Combien de temps en seconde le saut doit il durer ?"), Range(0.1f, 5f)]
    private float jumpDuration;
    [SerializeField]
    private AnimationCurve jumpCurve;
    [SerializeField, Range(0, 100), Tooltip("Multiplicateur de la gravité, 1 = gravité de base d'Unity.")]
    private float gravityScale;
    [SerializeField, Range(0.01f, 1), Tooltip("Valeur à dépasser avec le joystick pour initier le déplacement.")]
    private float deadZone;

    //==========================================================================
    private Coroutine brakingCoroutine;

    //========================================================================== POUR LE SAUT (DETECTION DE SOL)
    private RaycastHit2D groundCheck;
    public RaycastHit2D GroundCheck => groundCheck;
    private float castRadius;
    private float castDistance;
    private bool isJumping;

    //==========================================================================
    private Vector2 inputVectorMove = Vector2.zero;
    public Vector2 InputVectorMove => inputVectorMove;

    //==========================================================================
    private bool holdJump;
    public bool HoldJump { set => holdJump = value; }
    #endregion

    #region Unity_Functions
    //==========================================================================
    private void Awake()
    {
        stopDuration = stopDuration < 0.01f ? 0.01f : stopDuration;
        PManager.LookDirection = Vector2.right;
        castRadius = transform.localScale.x * .5f - .05f;
    }

    private void Start()
    {
        PManager.Rb2D.gravityScale = gravityScale != 0 ? gravityScale : PManager.Rb2D.gravityScale;

        if (PManager.PCollider is CapsuleCollider2D)
            castDistance = (PManager.PCollider as CapsuleCollider2D).size.y * transform.localScale.y * .25f + .3f;
        else
            throw new Exception("PManager.PCollider is not a CapsuleCollider2D. Please update the code.");
    }

    private void Update()
    {
        #if UNITY_EDITOR
        {
            if(stopDuration < 0.01f)
                stopDuration = 0.01f;
        }
        #endif
    }

    private void FixedUpdate()
    {
        #if UNITY_EDITOR
        {
            if (!isJumping && PManager.PlayerState != PLAYER_STATE.DASHING)
                PManager.Rb2D.gravityScale = gravityScale;
        }
        #endif

        if (PManager.PlayerState != PLAYER_STATE.STUNNED && PManager.PlayerState != PLAYER_STATE.DASHING) // Si le joueur est stun ou dashing, on fait rien.
        {
            castRadius = transform.localScale.x * .5f - .05f;
            castDistance = (PManager.PCollider as CapsuleCollider2D).size.y * transform.localScale.y * .25f + .3f;

            groundCheck = Physics2D.CircleCast(transform.position, castRadius, Vector2.down, castDistance);

            if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED) // Si le joueur n'est pas knockback, il peut bouger.
            {
                if(PManager.PlayerState != PLAYER_STATE.SHOOTING) // Si le joueur n'est pas entrain de viser, alors il marche ou il tombe.
                {
                    if (groundCheck)
                        PManager.PlayerState = PLAYER_STATE.WALKING;
                    else
                        PManager.PlayerState = PLAYER_STATE.FALLING;
                }

                Movement();

                if (holdJump && groundCheck)
                    OnJump();

                // On limite la vitesse du joueur.
                PManager.Rb2D.velocity = new Vector2(Mathf.Clamp(PManager.Rb2D.velocity.x, -maxSpeed, maxSpeed), PManager.Rb2D.velocity.y);
            }
            else
            {
                // Si le joueur est knockback, on regarde s'il est au sol.
                if (PManager.Rb2D.velocity.y <= 0 && groundCheck)
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
            inputVectorMove = Vector2.zero;
            return;
        }
        
        inputVectorMove = input;

        // Redondance (x est pas censé valoir 0 mais on sait jamais)
        if (inputVectorMove.x > 0)
            PManager.LookDirection = Vector2.right;
        else if (inputVectorMove.x < 0)
            PManager.LookDirection = Vector2.left;

        if(brakingCoroutine != null)
        {
            StopCoroutine(brakingCoroutine);
            brakingCoroutine = null;
        }
    }

    public void OnJump()
    {
        if (PManager.PlayerState == PLAYER_STATE.STUNNED
            || PManager.PlayerState == PLAYER_STATE.KNOCKBACKED
            || PManager.PlayerState == PLAYER_STATE.SHOOTING)
            return;

        int testlayer = 0;
        if (groundCheck)
            testlayer = groundCheck.collider.gameObject.layer;

        if (groundCheck && (testlayer == LayerMask.NameToLayer("Destructible")
            || testlayer == LayerMask.NameToLayer("Indestructible")
            || testlayer == LayerMask.NameToLayer("Trap")
            || testlayer == LayerMask.NameToLayer("Limite")))
        {
            StartCoroutine(Jump());
        }
        else
        {
            #if UNITY_EDITOR
            { 
                Debug.Log("You can't jump, you're not on solid ground.");
            }
            #endif
        }
    }

    IEnumerator Braking()
    {
        float iniVelocityX = PManager.Rb2D.velocity.x;
        float t = 0;
        while(t < 1f)
        {
            if (PManager.PlayerState == PLAYER_STATE.KNOCKBACKED || PManager.PlayerState == PLAYER_STATE.DASHING)
                break;

                PManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), PManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / stopDuration;
            yield return new WaitForFixedUpdate();
        }

        if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED && PManager.PlayerState == PLAYER_STATE.DASHING)
            PManager.Rb2D.velocity = new Vector2(0, PManager.Rb2D.velocity.y);

        brakingCoroutine = null;
    }

    private void Movement()
    {
        if (inputVectorMove == Vector2.zero)
        {
            if (brakingCoroutine == null && PManager.Rb2D.velocity.x != 0)
            {
                brakingCoroutine = StartCoroutine(Braking());
            }
        }

        // Permet de se retourner rapidement sans perdre sa vitesse
        if ((inputVectorMove.x / Mathf.Abs(inputVectorMove.x)) + (PManager.Rb2D.velocity.x / Mathf.Abs(PManager.Rb2D.velocity.x)) == 0)
            PManager.Rb2D.velocity = new Vector2(-PManager.Rb2D.velocity.x, PManager.Rb2D.velocity.y);

        PManager.Rb2D.velocity += Time.fixedDeltaTime * 100f * new Vector2(inputVectorMove.x, 0);
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        float t = 0;
        PManager.Rb2D.gravityScale = 0;
        while (t < 1)
        {
            if (CheckHeadBonk())
                break;

            if (PManager.PlayerState == PLAYER_STATE.KNOCKBACKED
                || PManager.PlayerState == PLAYER_STATE.DASHING
                || PManager.PlayerState == PLAYER_STATE.SHOOTING)
                break;

            PManager.Rb2D.velocity = new Vector2(PManager.Rb2D.velocity.x, Mathf.LerpUnclamped(jumpForce, 0, jumpCurve.Evaluate(t)));
            t += Time.fixedDeltaTime / jumpDuration;
            yield return new WaitForFixedUpdate();
        }
        isJumping = false;
        yield break;
    }

    private bool CheckHeadBonk()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            origin : PManager.PCollider.bounds.center,
            radius : PManager.PCollider.bounds.extents.x - 0.01f,
            direction : Vector2.up,
            distance : (PManager.PCollider.bounds.extents.y - PManager.PCollider.bounds.extents.x) + 0.11f, // 0.10f + 0.01f
            layerMask : LayerMask.GetMask("Player", "Destructible", "Indestructible", "Trap", "Limite"));;

        if (hit)
            return true;
        else
            return false;
    }
    #endregion
}