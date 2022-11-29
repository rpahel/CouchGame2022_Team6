using CustomMaths;
using Data;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


public class PlayerSystemManager : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private Rigidbody2D rb2D;
    private Collider2D coll;
    private PlayerInputs pInputs;
    private PlayerSystem playerSystem;

    // Getter
    public Rigidbody2D Rb2D => rb2D; 
    public Collider2D PCollider => coll;
    public PlayerInputs PInputs => pInputs;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;
    [HideInInspector] public PlayerInput playerInput;

    // PLAYER MOVEMENT Variables
    public Vector2 LookDirection { get; set; }
    [SerializeField] private float maxSize = 2.857f;
    [SerializeField] private float minSize = 1f;
    
    //==========================================================================
    [Header("PLAYER MOVEMENT Variables")]
    [SerializeField, Range(0, 40f)] private float maxSpeed;
    [SerializeField, Range(.01f, .2f), Tooltip("Dur�e de freinage en seconde.")] private float stopDuration;
    [SerializeField] private int jumpForce;
    [SerializeField, Range(0, 100), Tooltip("Multiplicateur de la gravit�, 1 = gravit� de base d'Unity.")]  private float gravityScale;
    [SerializeField, Range(0.01f, 1), Tooltip("Valeur � d�passer avec le joystick pour initier le d�placement.")]  private float deadZone;

    //GETTER
    public float MaxSpeed => maxSpeed;
    public float StopDuration => stopDuration;
    public int JumpForce => jumpForce;
    public float GravityScale => gravityScale;
    public float DeadZone => deadZone;

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
    
    // PLAYER EAT VAR
    [Header("EAT Variables")]
    [Range(0, 100), Tooltip("La quantit� de nourriture dans le corps.")] public int fullness;
    [SerializeField, Tooltip("La dur�e d'attente entre deux inputs de manger."), Range(0f, 1f)] private float eatCooldown;
    [SerializeField, Tooltip("Le nombre de cubes mang�s par seconde."), Range(2, 10)] private int eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 5f)] private float eatDistance;
    [SerializeField, Tooltip("Combien de nourriture tu re�ois en mangeant un cube. 100 = Un cube suffit � te remplir."), Range(0f, 100f)] private int filling;
    public float cooldown;
    public float tickHoldEat;
    public bool holdEat;
    public bool HoldEat { set => holdEat = value; }
    public float EatCooldown => eatCooldown;
    public int EatTickrate => eatTickrate;
    public float EatDistance => eatDistance;
    public int Filling => filling;
    
    // PLAYER SHOOT VAR
    [Header("PLAYER SHOOT Variables")]
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilisé pour tirer.")]
    private int necessaryFood;
    [SerializeField, Range(0, 2), Tooltip("Laps de temps entre chaque tir.")]
    private float cooldownShoot;
    public float cdTimer;
    [SerializeField, Tooltip("Le gameObject AimPivot de ce Prefab.")]
    private Transform aimPivot;

    public int NecessaryFood => necessaryFood;
    public float CooldownShoot => cooldownShoot;
    public Transform AimPivot => aimPivot;
    //============================
    [Header("Projectile")]
    [SerializeField] private float initialSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float bounceForce;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retiré au joueur ennemi touché.")]
    private int inflictedFoodDamage;
    [SerializeField, Tooltip("Force du knockback infligé au joueur ennemi.")]
    private float knockBackForce;

    public float InitialSpeed => initialSpeed;
    public float Gravity => gravity;
    public float BounceForce => gravity;
    public int InflictedFoodDamage => inflictedFoodDamage;
    public float KnockBackForce => knockBackForce;
    
    //============================
    public float raycastRange; // Utilisé pour voir si y'a assez de place pour tirer
    #endregion

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        pInputs = GetComponent<PlayerInputs>();
        playerSystem = GetComponent<PlayerSystem>();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        {
            Debug.DrawRay(transform.position - Vector3.forward, inputVectorDirection * 5f, Color.cyan, Time.deltaTime);
        }
        #endif

        LookDirection = inputVectorDirection.x switch
        {
            // On change LookDirection ici
            > 0 => Vector2.right,
            < 0 => Vector2.left,
            _ => LookDirection
        };
    }

    private void FixedUpdate()
    {
        UpdatePlayerScale();

       /* insideSprite.color = PlayerState switch
        {
            // On change la couleur du joueur en fonction de son �tat
            PLAYER_STATE.KNOCKBACKED => Color.red,
            PLAYER_STATE.SHOOTING => Color.blue,
            _ => Color.white
        };*/
    }

    public void SetStopDuration(float amount)
    {
        stopDuration = amount;
    }

    /// <summary>
    /// R�duit la jauge de bouffe et knockback le joueur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="damageDealer">L'objet responsable des d�gats (un joueur, un pi�ge, etc).</param>
    /// <param name="damage">La quantit� de bouffe � retirer.</param>
    public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        fullness = Mathf.Clamp(fullness - damage, 0, 100);
        rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
        UpdatePlayerScale();
        //PlayerState = PLAYER_STATE.KNOCKBACKED;
    }

    public void UpdatePlayerScale()
    {
        if (fullness <= 0 && playerSystem.PlayerState is not Dead)
        {
            playerSystem.SetState(new Dead(playerSystem));
            return;
        }

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

        //StartCoroutine(MoverOverAnimation(endPosition));
    }

    /*IEnumerator MoverOverAnimation(Vector2 endPosition)
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
    }*/
}
