using CustomMaths;
using Data;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


public class PlayerSystemManager : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private Rigidbody2D rb2D;
    private Collider2D coll;
    private PlayerInputs pInputs;


    // Getter
    public Rigidbody2D Rb2D => rb2D;
    public Collider2D PCollider => coll;
    public PlayerInputs PInputs => pInputs;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;

    //============================
    private PLAYER_STATE playerState;
    public PLAYER_STATE PlayerState
    {
        get { return playerState; }
        set
        {
            if (PlayerState != value)
            {
                PreviousPlayerState = playerState;
                playerState = value;
            }
        }
    }

    public PLAYER_STATE PreviousPlayerState { get; private set; }
    public Vector2 AimDirection { get; set; }
    public Vector2 LookDirection { get; set; }

    //============================
    [SerializeField] private float maxSize = 2.857f;
    [SerializeField] private float minSize = 1f;

    //============================ TODO : Supprimer
    public SpriteRenderer insideSprite;
    #endregion

    #region Unity_Functions


    #region Variables
    //==========================================================================
    [SerializeField, Range(0, 40f)]
    public float maxSpeed;
    [SerializeField, Range(.01f, .2f), Tooltip("Durée de freinage en seconde.")]
    public float stopDuration;
    [SerializeField]
    public int jumpForce;
    [SerializeField, Range(0, 100), Tooltip("Multiplicateur de la gravité, 1 = gravité de base d'Unity.")]
    public float gravityScale;
    [SerializeField, Range(0.01f, 1), Tooltip("Valeur à dépasser avec le joystick pour initier le déplacement.")]
    public float deadZone;

    //==========================================================================
    public Coroutine brakingCoroutine;

    //========================================================================== POUR LE SAUT (DETECTION DE SOL)
    public RaycastHit2D groundCheck;
    public RaycastHit2D GroundCheck => groundCheck;
    public float castRadius;
    public float castDistance;

    //==========================================================================
    public Vector2 inputVectorMove = Vector2.zero;
    public Vector2 InputVectorMove => inputVectorMove;

    //==========================================================================
    public bool holdJump;
    public bool HoldJump { set => holdJump = value; }
    #endregion

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        pInputs = GetComponent<PlayerInputs>();
    }

    private void Update()
    {

#if UNITY_EDITOR
        {
            Debug.DrawRay(transform.position - Vector3.forward, AimDirection * 5f, Color.cyan, Time.deltaTime);
        }
#endif

        // On change LookDirection ici
        if (AimDirection.x > 0)
            LookDirection = Vector2.right;
        else if (AimDirection.x < 0)
            LookDirection = Vector2.left;
    }

    private void FixedUpdate()
    {
       // UpdatePlayerScale();

        // On change la couleur du joueur en fonction de son état
        if (PlayerState == PLAYER_STATE.KNOCKBACKED)
            insideSprite.color = Color.red;
        else if (PlayerState == PLAYER_STATE.SHOOTING)
            insideSprite.color = Color.blue;
        else
            insideSprite.color = Color.white;
    }
    #endregion

    /// <summary>
    /// Réduit la jauge de bouffe et knockback le joueur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="damageDealer">L'objet responsable des dégats (un joueur, un piège, etc).</param>
    /// <param name="damage">La quantité de bouffe à retirer.</param>
   /* public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        PEat.fullness = Mathf.Clamp(PEat.fullness - damage, 0, 100);
        UpdatePlayerScale();

        //rb2D.AddForce(knockBackForce, ForceMode2D.Impulse);
        rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
        PlayerState = PLAYER_STATE.KNOCKBACKED;
    }

    public void UpdatePlayerScale()
    {
        transform.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, pEat.fullness * .01f);
    }*/

    /// <summary>
    /// Pousse de façon smooth le joueur vers un endroit donné.
    /// </summary>
    public void MoveOverTo(Vector2 endPosition)
    {

#if UNITY_EDITOR
        {
            CustomDebugs.DrawWireSphere(endPosition, .5f, Color.magenta, 5f, 1);
        }
#endif

        StartCoroutine(MoverOverAnimation(endPosition));
    }

    IEnumerator MoverOverAnimation(Vector2 endPosition)
    {
        Vector2 iniPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            if (PlayerState == PLAYER_STATE.KNOCKBACKED)
                break;

            rb2D.velocity = Vector2.zero;
            transform.position = DOVirtual.EasedValue(iniPos, endPosition, t, Ease.OutBack, 2f);
            t += Time.deltaTime * 4f;
            yield return null;
        }

        if (PlayerState != PLAYER_STATE.KNOCKBACKED)
        {
            transform.position = endPosition;
        }
    }
}
