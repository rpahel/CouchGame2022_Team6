using System.Collections;
using UnityEngine;
using Data;
using System;
using DG.Tweening;

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
    [SerializeField, Range(0, 70)]
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
    public Vector2 SensDuRegard { get; set; } // à ne pas confondre avec aimDirection
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        dureeAvantArret = dureeAvantArret < 0.01f ? 0.01f : dureeAvantArret;

        //if (!TryGetComponent<PlayerManager>(out PManager)) // ça c'est obligé pcq sinon playerManager == null;
        //{
        //    #if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //    #endif
        //    throw new Exception("No PlayerManager component found in Player game object !");
        //}
        //
        //playerManager.TryGetPlayerComponent<Rigidbody2D>(out rb2d);
        //playerManager.TryGetPlayerComponent<Collider2D>(out coll);
        
        echelleDeGravité = echelleDeGravité != 0 ? echelleDeGravité : PManager.Rb2D.gravityScale;

        castRadius = transform.localScale.x * .5f - .05f;
        castDistance = (PManager.PCollider as CapsuleCollider2D).size.y * transform.localScale.y * .25f + .3f;
    }

    private void Update()
    {
        if(dureeAvantArret < 0.01f)
            dureeAvantArret = 0.01f;
    }

    private void FixedUpdate()
    {
        PManager.Rb2D.gravityScale = echelleDeGravité;

        groundCheck = Physics2D.CircleCast(transform.position, castRadius, Vector2.down, castDistance);

        if (groundCheck)
            PManager.PlayerState = PLAYER_STATE.WALKING;
        else
            PManager.PlayerState = PLAYER_STATE.FALLING;

        Deplacement();

        if (holdJump && groundCheck)
            OnJump();
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
        if (Mathf.Abs(input.x) <= deadZone)
        {
            inputVector_move = Vector2.zero;
            return;
        }
        
        inputVector_move = input;

        SensDuRegard = inputVector_move.x > 0 ? Vector2.right : Vector2.left;

        if(freinage != null)
        {
            StopCoroutine(freinage);
            freinage = null;
        }
    }

    public void OnJump()
    {
        int testlayer = 0;
        if (groundCheck)
            testlayer = groundCheck.collider.gameObject.layer;

        if (groundCheck && (testlayer == 6 || testlayer == 7))
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
        PManager.Rb2D.velocity = new Vector2(Mathf.Clamp(PManager.Rb2D.velocity.x, -vitesseMax, vitesseMax), PManager.Rb2D.velocity.y);
    }

    private void Jump()
    {
        PManager.Rb2D.velocity = new Vector2(PManager.Rb2D.velocity.x, 0);
        PManager.Rb2D.velocity += new Vector2(0, forceDeSaut);
    }
    #endregion
}
