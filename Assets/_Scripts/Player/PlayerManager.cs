using CustomMaths;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private Rigidbody2D _rb2D;
    private Collider2D _coll;

    private PlayerStateSystem _playerSystem;
    private CooldownManager _cooldownManager;

    // Getter
    public Rigidbody2D Rb2D => _rb2D;
    public Collider2D PCollider => _coll;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;
    [HideInInspector] public PlayerInput playerInput;
    public SpriteRenderer insideSprite; // TODO : A supprimer quand on aura les assets des GA

    // PLAYER MOVEMENT Variables
    public Vector2 LookDirection { get; set; }
    [SerializeField] private float _maxSize = 2.857f;
    [SerializeField] private float _minSize = 1f;

    //==========================================================================
    [Header("PLAYER MOVEMENT Variables")]
    [SerializeField, Range(0, 40f)]
    private float _maxSpeed;
    [SerializeField, Range(.01f, .2f), Tooltip("Duree de freinage en seconde.")]
    private float _stopDuration;
    [SerializeField, Tooltip("La vitesse initial du joueur quand il saute.")]
    private int _jumpForce;
    [SerializeField, Tooltip("Combien de temps en seconde le saut doit il durer ?"), Range(0.1f, 5f)]
    private float _jumpDuration;
    [SerializeField]
    private AnimationCurve _jumpCurve;
    [SerializeField, Range(0, 100), Tooltip("Multiplicateur de la gravite, 1 = gravite de base d'Unity.")]
    private float _gravityScale;
    [SerializeField, Range(0.01f, 1), Tooltip("Valeur a depasser avec le joystick pour initier le deplacement.")]
    private float _deadZone;
    private Coroutine _jumpCoroutine;
    private int _groundChekLayerMask;


    //GETTER
    public float MaxSpeed => _maxSpeed;
    public float StopDuration => _stopDuration;
    public int JumpForce => _jumpForce;
    public float JumpDuration => _jumpDuration;
    public AnimationCurve JumpCurve => _jumpCurve;
    public Coroutine JumpCoroutine { set => _jumpCoroutine = value; }
    public float GravityScale => _gravityScale;
    public float DeadZone => _deadZone;


    //==========================================================================
    [HideInInspector] public Coroutine brakingCoroutine;

    //========================================================================== POUR LE SAUT (DETECTION DE SOL)
    [HideInInspector] public float castRadius;
    [HideInInspector] public float castDistance;
    [HideInInspector] public bool isJumping;

    //==========================================================================
    [HideInInspector] public Vector2 inputVectorDirection = Vector2.zero;
    [HideInInspector] public Vector2 inputVectorMove = Vector2.zero;
    public Vector2 InputVectorMove => inputVectorMove;

    //==========================================================================
    [HideInInspector] public bool holdJump;

    // PLAYER EAT VAR
    [Header("EAT Variables")]
    [Range(0, 100), Tooltip("La quantite de nourriture dans le corps.")]
    public int fullness;
    [SerializeField, Tooltip("Le nombre de cubes manges par seconde."), Range(2, 10)]
    private int _eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 5f)]
    private float _eatDistance;
    [SerializeField, Tooltip("Combien de nourriture tu recois en mangeant un cube. 100 = Un cube suffit a te remplir."), Range(0f, 100f)]
    private int _filling;
    [HideInInspector]
    public float tickHoldEat;
    [HideInInspector]
    public bool holdEat;

    // PLAYER SHOOT VAR
    [Header("PLAYER SHOOT Variables")]
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilise pour tirer.")]
    private int _necessaryFood;
    [SerializeField, Range(0, 2), Tooltip("Laps de temps entre chaque tir.")]
    private float _cooldownShoot;
    [HideInInspector]
    public bool canShoot = true;
    [SerializeField, Tooltip("Le gameObject AimPivot de ce Prefab.")]
    private Transform _aimPivot;
    private float _raycastRange; // Utilisé pour voir si y'a assez de place pour tirer

    public int NecessaryFood => _necessaryFood;
    public float CooldownShoot => _cooldownShoot;
    public Transform AimPivot => _aimPivot;

    //============================
    [Header("PROJECTILE Variables")]
    [SerializeField] private float _initialSpeed;
    [SerializeField] private float _gravity;
    [SerializeField, Tooltip("Force du rebond de la boule.")]
    private float _bounceForce;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retiré au joueur ennemi touché.")]
    private int _inflictedFoodDamage;
    [SerializeField, Tooltip("Force du knockback infligé au joueur ennemi.")]
    private float _knockBackForce;

    //==========================================================================

    //[Header("STUN Variables")]
    //[SerializeField] private float cooldownStun;
    //public float CooldownStun => cooldownStun;
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>();
        _playerSystem = GetComponent<PlayerStateSystem>();
        _cooldownManager = GetComponent<CooldownManager>();
        _groundChekLayerMask = LayerMask.GetMask("Destructible", "Indestructible", "Limite", "Player");

        LookDirection = Vector2.right;
        if (StopDuration < 0.01f) SetStopDuration(0.01f);
        tickHoldEat = 1f;
    }

    private void Start()
    {
        _raycastRange = GameManager.Instance.LevelGenerator.Scale * 4;

        insideSprite.color = color;
        castRadius = _coll.bounds.extents.x - .05f;
        _rb2D.gravityScale = _gravityScale != 0 ? _gravityScale : _rb2D.gravityScale;

        if (_coll is CapsuleCollider2D)
            castDistance = (_coll.bounds.extents.y - _coll.bounds.extents.x) + .1f;
        else
            throw new Exception("PManager.PCollider is not a CapsuleCollider2D. Please update the code.");
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
        if (!isJumping)
            _rb2D.gravityScale = _gravityScale;

        insideSprite.color = _playerSystem.PlayerState switch
        {
            // On change la couleur du joueur en fonction de son etat
            Knockback => Color.red,
            _ => color
        };

        if (holdEat)
        {
            if (tickHoldEat >= 1f)
            {
                OnEat();
                tickHoldEat = 0f;
            }
            else
            {
                tickHoldEat += Time.deltaTime * _eatTickrate;
            }
        }
    }
    #endregion

    #region Eat
    /// <summary>
    /// Essaie de manger ce qui se trouve dans direction.
    /// </summary>
    public void OnEat()
    {
        if (_playerSystem.PlayerState is not Moving)
            return;

        var direction = inputVectorDirection;

        //if (fullness >= 100)
        //{
        //    Debug.Log("Tu es plein et ne peut donc plus manger! Vomis.");
        //    return;
        //}

        if (direction == Vector2.zero)
        {
            direction = !GroundCheck() ? Vector2.up : LookDirection;
        }

#if UNITY_EDITOR
        Debug.DrawRay(transform.position - Vector3.forward, direction.normalized * _eatDistance, Color.red, 0.2f);
#endif

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, _eatDistance, 1 << LayerMask.NameToLayer("Destructible"));
        if (hit)
        {
            hit.transform.parent.GetComponent<Cube_Edible>().GetEaten(transform);
            fullness = Mathf.Clamp(fullness + _filling, 0, 100);
            UpdatePlayerScale();
        }
        else if (!GroundCheck()) // S'il touche rien et qu'il n'est pas au sol on ressaie de manger dans le sens du regard cette fois
        {
            direction = LookDirection;
            hit = Physics2D.Raycast(transform.position, direction.normalized, _eatDistance, 1 << LayerMask.NameToLayer("Destructible"));

            if (hit)
            {
                hit.transform.parent.GetComponent<Cube_Edible>().GetEaten(transform);
                fullness = Mathf.Clamp(fullness + _filling, 0, 100);
                UpdatePlayerScale();
            }
        }
    }
    #endregion

    #region Jump

    public bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.CircleCast(_coll.bounds.center, castRadius, Vector2.down, castDistance, _groundChekLayerMask);
        return hit;
    }

    public void Jump()
    {
        if (_jumpCoroutine != null)
            return;

        _jumpCoroutine = StartCoroutine(_cooldownManager.JumpCoroutine());
    }

    public bool CheckHeadBonk()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            origin: PCollider.bounds.center,
            radius: PCollider.bounds.extents.x - 0.01f,
            direction: Vector2.up,
            distance: (PCollider.bounds.extents.y - PCollider.bounds.extents.x) + 0.11f, // 0.10f + 0.01f
            layerMask: LayerMask.GetMask("Player", "Destructible", "Indestructible", "Trap", "Limite")); ;

        if (hit)
            return true;
        else
            return false;
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
        if (_playerSystem.PlayerState is Dashing) return;

        fullness = Mathf.Clamp(fullness - damage, 0, 100);

        if (fullness <= 0 && _playerSystem.PlayerState is not Dead)
        {
            _playerSystem.SetState(new Dead(_playerSystem));
            return;
        }

        UpdatePlayerScale();
        //Stats

        _playerSystem.SetKnockback(knockBackForce);
    }
    #endregion

    #region Shoot
    public void Shoot()
    {
        var aimDirection = _playerSystem.PlayerSystemManager.inputVectorDirection;

        if (!_playerSystem.PlayerSystemManager.canShoot)
        {
            Debug.Log($"Attendez le cooldown du tir");
            return;
        }

        if (_playerSystem.PlayerSystemManager.fullness < _playerSystem.PlayerSystemManager.NecessaryFood)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }

        if (aimDirection == Vector2.zero)
            aimDirection = _playerSystem.PlayerSystemManager.LookDirection;

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
            return;
        }

        ShootProjectile(aimDirection);

        _playerSystem.PlayerSystemManager.fullness = Mathf.Clamp(_playerSystem.PlayerSystemManager.fullness - _playerSystem.PlayerSystemManager.NecessaryFood, 0, 100);
        _playerSystem.PlayerSystemManager.UpdatePlayerScale();

        _playerSystem.CooldownManager.StartCoroutine(_playerSystem.CooldownManager.CooldownShoot());

        _playerSystem.SetState(new Moving(_playerSystem));
    }

    private void ShootProjectile(Vector2 aimDirection)
    {
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = _playerSystem.PlayerSystemManager;
        projectile.color = _playerSystem.PlayerSystemManager.color;
        projectile.transform.position = _playerSystem.transform.position;
        projectile.gravity = _playerSystem.PlayerSystemManager._gravity;
        projectile.bounceForce = _playerSystem.PlayerSystemManager._bounceForce;
        projectile.percentageDealt = _playerSystem.PlayerSystemManager._inflictedFoodDamage;
        projectile.knockBackForce = _playerSystem.PlayerSystemManager._knockBackForce;
        projectile.ownerVelocityAtLaunch = _playerSystem.PlayerSystemManager.Rb2D.velocity;

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, _playerSystem.PlayerSystemManager._initialSpeed);
    }

    /// <summary>
    /// Check si y'a assez d'espace en la direction du tir pour faire pop un cube
    /// </summary>
    private bool IsThereEnoughSpace(Vector2 aimDirection)
    {
        Vector2 rayOrigin = (Vector2)transform.position + (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin, aimDirection, _raycastRange);

#if UNITY_EDITOR
        if (!hit1)
            Debug.DrawRay(rayOrigin, aimDirection * _raycastRange, Color.red, 1f);
        else
            Debug.DrawLine(rayOrigin, hit1.point, Color.red, 1f);
#endif

        rayOrigin = (Vector2)transform.position - (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, aimDirection, _raycastRange);

#if UNITY_EDITOR
        if (!hit2)
            Debug.DrawRay(rayOrigin, aimDirection * _raycastRange, Color.red, 1f);
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
        transform.localScale = Vector3.one * Mathf.Lerp(_minSize, _maxSize, fullness * .01f);
    }

    /// <summary>
    /// Pousse de façon smooth le joueur vers un endroit donne.
    /// </summary>
    public void MoveOverTo(Vector2 endPosition)
    {

#if UNITY_EDITOR
        {
            CustomDebugs.DrawWireSphere(endPosition, .5f, Color.magenta, 5f, 1);
        }
#endif

        _cooldownManager.StartCoroutine(_cooldownManager.MoverOverAnimation(endPosition));
    }

    public void SetStopDuration(float amount)
    {
        _stopDuration = amount;
    }
    #endregion
}