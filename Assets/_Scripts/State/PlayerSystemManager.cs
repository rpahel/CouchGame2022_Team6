using CustomMaths;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSystemManager : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private Rigidbody2D rb2D;
    private Collider2D coll;

    private PlayerSystem playerSystem;
    private CooldownManager cooldownManager;


    // Getter
    public Rigidbody2D Rb2D => rb2D; 
    public Collider2D PCollider => coll;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;
    [HideInInspector] public PlayerInput playerInput;
    public SpriteRenderer insideSprite;

    // PLAYER MOVEMENT Variables
    [HideInInspector] public Vector2 LookDirection { get; set; }
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
    [HideInInspector] public Coroutine brakingCoroutine;

    //========================================================================== POUR LE SAUT (DETECTION DE SOL)
    [HideInInspector] public RaycastHit2D groundCheck;
    public RaycastHit2D GroundCheck => groundCheck;
    [HideInInspector] public float castRadius;
    [HideInInspector] public float castDistance;

    //==========================================================================
    [HideInInspector] public Vector2 inputVectorMove = Vector2.zero;
    public Vector2 InputVectorMove => inputVectorMove;

    [HideInInspector] public Vector2 inputVectorDirection = Vector2.zero;

    //==========================================================================
    [HideInInspector] public bool holdJump;
    public bool HoldJump { set => holdJump = value; }
    
    // PLAYER EAT VAR
    [Header("EAT Variables")]
    [Range(0, 100), Tooltip("La quantit� de nourriture dans le corps.")] public int fullness;
    [SerializeField, Tooltip("La dur�e d'attente entre deux inputs de manger."), Range(0f, 1f)] private float eatCooldown;
    [SerializeField, Tooltip("Le nombre de cubes mang�s par seconde."), Range(2, 10)] private int eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 5f)] private float eatDistance;
    [SerializeField, Tooltip("Combien de nourriture tu re�ois en mangeant un cube. 100 = Un cube suffit � te remplir."), Range(0f, 100f)] private int filling;
    [HideInInspector] public float cooldown;
    [HideInInspector] public float tickHoldEat;
    [HideInInspector] public bool holdEat;
    public bool HoldEat { set => holdEat = value; }
    public float EatCooldown => eatCooldown;
    public int EatTickrate => eatTickrate;
    public float EatDistance => eatDistance;
    public int Filling => filling;
    
    // PLAYER SHOOT VAR
    [Header("PLAYER SHOOT Variables")]
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilisé pour tirer.")]
    private int necessaryFoodShoot;
    [SerializeField, Range(0, 2), Tooltip("Laps de temps entre chaque tir.")]
    private float cooldownShoot;
    //[HideInInspector] public float cdTimer;
    [HideInInspector] public bool canShoot = true;
    [SerializeField, Tooltip("Le gameObject AimPivot de ce Prefab.")]
    private Transform aimPivot;

    public int NecessaryFoodShoot => necessaryFoodShoot;
    public float CooldownShoot => cooldownShoot;
    public Transform AimPivot => aimPivot;
    //============================
    [Header("PROJECTILE Variables")]
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

    //==========================================================================
    [SerializeField] public float raycastRange; // Utilisé pour voir si y'a assez de place pour tirer

    //[Header("STUN Variables")]
    //[SerializeField] private float cooldownStun;
    //public float CooldownStun => cooldownStun;
    
    [Header("SPECIAL Variables")]
    [SerializeField, Range(1, 3), Tooltip("Dur�e en secondes avant d'atteindre le niveau de charge max.")]
    private float timeToMaxCharge;
    [SerializeField, Range(0, 40), Tooltip("Distance en m�tres du special � son maximum.")]
    public float maxDistance;
    [SerializeField, Range(0, 40), Tooltip("Distance en m�tres du special � son minimum (simple press du bouton).")]
    private float minDistance;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilisé pour special.")]
    private int necessaryFoodSpecial;
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private float dashForce;
    [SerializeField]
    private GameObject specialTrigger;

    public Vector2 inputDirectionDash;
    public float TimeToMaxCharge => timeToMaxCharge;
    public float MaxDistance => maxDistance;
    public float MinDistance => minDistance;
    public int NecessaryFoodSpecial => necessaryFoodSpecial;
    public float DashCooldown => dashCooldown; 
    public float DashForce => dashForce;

    public GameObject SpecialTrigger => specialTrigger;
    //===================================================
    public float charge; // 0 � 1
    public bool isHolding;
    public bool canDash = true;
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        playerSystem = GetComponent<PlayerSystem>();
        cooldownManager = GetComponent<CooldownManager>();
    }

    private void Start()
    {
        insideSprite.color = color;
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
       insideSprite.color = playerSystem.PlayerState switch
        {
            // On change la couleur du joueur en fonction de son �tat
            Knockback => Color.red,
            _ => color
        };
    }
    #endregion

    #region Damage
    /// <summary>
    /// R�duit la jauge de bouffe et knockback le joueur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="damageDealer">L'objet responsable des d�gats (un joueur, un pi�ge, etc).</param>
    /// <param name="damage">Lra quantit� de bouffe � retirer.</param>
    public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        if (playerSystem.PlayerState is Special) return;

        fullness = Mathf.Clamp(fullness - damage, 0, 100);

        if (fullness <= 0 && playerSystem.PlayerState is not Dead)
        {
            playerSystem.SetState(new Dead(playerSystem));
            return;
        }

        UpdatePlayerScale();
        //Stats

        //rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
        playerSystem.SetKnockback(knockBackForce);
    }
    #endregion

    #region Shoot
    public void Shoot()
    {
        var aimDirection = playerSystem.PlayerSystemManager.inputVectorDirection;

        if (!playerSystem.PlayerSystemManager.canShoot)
        {
            Debug.Log($"Attendez le cooldown du tir");
            return;
        }

        if (playerSystem.PlayerSystemManager.fullness < playerSystem.PlayerSystemManager.NecessaryFoodShoot)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }

        if (aimDirection == Vector2.zero)
            aimDirection = playerSystem.PlayerSystemManager.LookDirection;

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
            return;
        }

        ShootProjectile(aimDirection);

        playerSystem.PlayerSystemManager.fullness = Mathf.Clamp(playerSystem.PlayerSystemManager.fullness - playerSystem.PlayerSystemManager.NecessaryFoodShoot, 0, 100);
        playerSystem.PlayerSystemManager.UpdatePlayerScale();

        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.CooldownShoot());

        playerSystem.SetState(new Moving(playerSystem));
    }

    private void ShootProjectile(Vector2 aimDirection)
    {
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = playerSystem.PlayerSystemManager;
        projectile.color = playerSystem.PlayerSystemManager.color;
        projectile.transform.position = playerSystem.transform.position;
        projectile.gravity = playerSystem.PlayerSystemManager.Gravity;
        projectile.bounceForce = playerSystem.PlayerSystemManager.BounceForce;
        projectile.percentageDealt = playerSystem.PlayerSystemManager.InflictedFoodDamage;
        projectile.knockBackForce = playerSystem.PlayerSystemManager.KnockBackForce;
        projectile.ownerVelocityAtLaunch = playerSystem.PlayerSystemManager.Rb2D.velocity;

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, playerSystem.PlayerSystemManager.InitialSpeed);
    }

    /// <summary>
    /// Check si y'a assez d'espace en la direction du tir pour faire pop un cube
    /// </summary>
    private bool IsThereEnoughSpace(Vector2 aimDirection)
    {
        Vector2 rayOrigin = (Vector2)playerSystem.transform.position + (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin, aimDirection, playerSystem.PlayerSystemManager.raycastRange);

#if UNITY_EDITOR
        if (!hit1)
            Debug.DrawRay(rayOrigin, aimDirection * playerSystem.PlayerSystemManager.raycastRange, Color.red, 1f);
        else
            Debug.DrawLine(rayOrigin, hit1.point, Color.red, 1f);
#endif

        rayOrigin = (Vector2)playerSystem.transform.position - (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, aimDirection, playerSystem.PlayerSystemManager.raycastRange);

#if UNITY_EDITOR
        if (!hit2)
            Debug.DrawRay(rayOrigin, aimDirection * playerSystem.PlayerSystemManager.raycastRange, Color.red, 1f);
        else
            Debug.DrawLine(rayOrigin, hit2.point, Color.red, 1f);
#endif

        RaycastHit2D closestHit;

        if (hit1 && hit2)
        {
            closestHit = hit1.distance < hit2.distance ? hit1 : hit2;
        }

        else if (hit1)
            closestHit = hit1;
        else
            closestHit = hit2;

        if (!closestHit) return true;

        if (closestHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            return true;

        RaycastHit2D[] hits = CustomPhysics.SquareCast((Vector2)closestHit.transform.position + GameManager.Instance.LevelGenerator.Scale * closestHit.normal, GameManager.Instance.LevelGenerator.Scale * .9f);

        foreach (RaycastHit2D hit2D in hits)
        {
            if (hit2D)
                return false;
        }

        return true;
    }
    #endregion

    #region Other
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

        cooldownManager.StartCoroutine(cooldownManager.MoverOverAnimation(endPosition));
    }

    public void SetStopDuration(float amount)
    {
        stopDuration = amount;
    }
    #endregion
}
