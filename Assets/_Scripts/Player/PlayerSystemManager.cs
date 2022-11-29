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

    // PLAYER MOVEMENT Variables
    private PLAYER_STATE playerState;
    public PLAYER_STATE PlayerState
    {
        get => playerState;
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
    [SerializeField] private float maxSize = 2.857f;
    [SerializeField] private float minSize = 1f;

    //============================ TODO : Supprimer
    public SpriteRenderer insideSprite;
    #endregion

    #region Unity_Functions
    



    #region Variables
    //==========================================================================
    [Header("PLAYER MOVEMENT Variables")]
    [SerializeField, Range(0, 40f)]
    public float maxSpeed;
    [SerializeField, Range(.01f, .2f), Tooltip("Dur�e de freinage en seconde.")]
    public float stopDuration;
    [SerializeField]
    public int jumpForce;
    [SerializeField, Range(0, 100), Tooltip("Multiplicateur de la gravit�, 1 = gravit� de base d'Unity.")]
    public float gravityScale;
    [SerializeField, Range(0.01f, 1), Tooltip("Valeur � d�passer avec le joystick pour initier le d�placement.")]
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
    
    public Vector2 inputVectorDirection = Vector2.zero;

    //==========================================================================
    public bool holdJump;
    public bool HoldJump { set => holdJump = value; }
    
    // EAT EAT EAT EAT 
    [Header("EAT Variables")]
    [Range(0, 100), Tooltip("La quantit� de nourriture dans le corps.")]
    public int fullness;
    [SerializeField, Tooltip("La dur�e d'attente entre deux inputs de manger."), Range(0f, 1f)]
    public float eatCooldown;
    [SerializeField, Tooltip("Le nombre de cubes mang�s par seconde."), Range(2, 10)]
    public int eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 5f)]
    public float eatDistance;
    [SerializeField, Tooltip("Combien de nourriture tu re�ois en mangeant un cube. 100 = Un cube suffit � te remplir."), Range(0f, 100f)]
    public int filling;
    public float cooldown;
    public float tickHoldEat;
    public bool holdEat;
    public bool HoldEat { set => holdEat = value; }
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
        UpdatePlayerScale();

        insideSprite.color = PlayerState switch
        {
            // On change la couleur du joueur en fonction de son �tat
            PLAYER_STATE.KNOCKBACKED => Color.red,
            PLAYER_STATE.SHOOTING => Color.blue,
            _ => Color.white
        };
    }
    #endregion

    /// <summary>
    /// R�duit la jauge de bouffe et knockback le joueur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="damageDealer">L'objet responsable des d�gats (un joueur, un pi�ge, etc).</param>
    /// <param name="damage">La quantit� de bouffe � retirer.</param>
    /*public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        PEat.fullness = Mathf.Clamp(PEat.fullness - damage, 0, 100);
        UpdatePlayerScale();

        //rb2D.AddForce(knockBackForce, ForceMode2D.Impulse);
        rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
        PlayerState = PLAYER_STATE.KNOCKBACKED;
    }*/

    public void UpdatePlayerScale()
    {
        transform.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, fullness * .01f);
    }

    /// <summary>
    /// Pousse de fa�on smooth le joueur vers un endroit donn�.
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
