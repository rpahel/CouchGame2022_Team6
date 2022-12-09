using CustomMaths;
using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class PlayerManager : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private Rigidbody2D _rb2D;
    private Collider2D _coll;

    private PlayerStateSystem _playerSystem;
    private CooldownManager _cooldownManager;
    private FaceManager faceManager;
    private StatisticsManager statsManager;

    // Getter
    public Rigidbody2D Rb2D => _rb2D;
    public Collider2D PCollider => _coll;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;
    [HideInInspector] public PlayerInputsHandler playerInput;
    public SpriteRenderer insideSprite; // TODO : A supprimer quand on aura les assets des GA
    public SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer face;
    public SpriteRenderer Face => face;
    [HideInInspector] public int playerIndex;
    private bool _isInvincible = false;
    public bool IsInvincible
    { 
        set {   
                _isInvincible = value;
                if (value) { StartCoroutine(InvincibilityCoroutine()); }
            }
    }

    private float _invincibleCd;

    // PLAYER MOVEMENT Variables
    public Vector2 LookDirection { get; set; }
    [SerializeField] private float _maxSize = 2.857f;
    [SerializeField] private float _minSize = 1f;

    [HideInInspector] public Image imageUI;
    [HideInInspector] public TextMeshProUGUI textUI;

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
    private int _groundChekLayerMaskWallJump;


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

    //==========================================================================
    [HideInInspector] public bool holdJump;

    // PLAYER EAT VAR
    [Header("EAT Variables")]
    [Range(0, 100), Tooltip("La quantite de nourriture dans le corps.")]
    public int fullness;
    [SerializeField, Tooltip("L'angle en degres du cone de manger."), Range(0, 90)]
    private float _eatAngle;
    [SerializeField, Tooltip("Décalage de la rotation du cone de manger par rapport à la visee et/ou le vecteur droit."), Range(-45, 45)]
    private float _eatAngleOffset;
    [SerializeField, Tooltip("L'angle de l'arc de cercle que va faire le mouvement du manger."), Range(0, 180)]
    private int _maxImmobileEatAngle;
    [SerializeField, Tooltip("Le nombre de cubes manges par seconde."), Range(2, 10)]
    private int _eatTickrate;
    [SerializeField, Tooltip("Distance max pour pouvoir manger le cube qu'on vise."), Range(1f, 20f)]
    private float _eatDistance;
    [SerializeField, Tooltip("Combien de nourriture tu recois en mangeant un cube. 100 = Un cube suffit a te remplir."), Range(0f, 100f)]
    private int _filling;
    [HideInInspector]
    public float tickHoldEat;
    [HideInInspector]
    public bool holdEat;
    [HideInInspector]
    public int currentImmobileEatAngle;
    private bool _hasEaten;


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

    //============================Wall JUMP============================//
    [Header("WALL JUMP Variables")]
    [SerializeField] private float wallJumpTimer = 0f;
    private float dirPlayerToWall;   
    public Transform wallCheckRight;  
    public bool isPlayerTouchingWall;
    public bool isSliding;
    public float wallSlidingSpeedMax;
    public float wallJumpDuration;  
    public float wallDetectionRadius;
    public bool wallJumping;
    public float wallJumpAngle;
    public float wallJumpForce;
    public bool canWallJump;
    public bool cantDoubleJump;

    //==========================================================================

    [Header("SPECIAL Variables")]
    [SerializeField, Range(0, 100)]
    private int _specialDamage;
    [SerializeField, Range(0, 100), Tooltip("Force du knockback que subit l'ennemi quand il se fait toucher par un special.")]
    private float _specialInflictedKnockbackForce;
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
    [HideInInspector] public float charge; // 0 a 1
    [HideInInspector] public bool isHolding;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public Vector2 inputDirectionDash;

    [Header("SPECIAL TEST Variables")]
    public bool specialStopsOnPlayerContact;

    public int SpecialDamage => _specialDamage;
    public float SpecialInflictedKnockbackForce => _specialInflictedKnockbackForce;
    public float TimeToMaxCharge => timeToMaxCharge;
    public float MaxDistance => maxDistance;
    public float MinDistance => minDistance;
    public int NecessaryFoodSpecial => necessaryFoodSpecial;
    public float DashCooldown => dashCooldown;
    public float DashForce => dashForce;
    public GameObject SpecialTrigger => specialTrigger;

    [Header("TEST STRESH AND SQUASH")]
    private Vector3 normalSpriteScale;
    public float durationEffectJump;
    public float multiplierXJump;
    public float multiplierYJump;

    [Header("SCORING VALUES")] 
    [SerializeField] private int damageScore;
    [SerializeField] private int killScore;
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>();
        _playerSystem = GetComponent<PlayerStateSystem>();
        _cooldownManager = GetComponent<CooldownManager>();
        _groundChekLayerMask = LayerMask.GetMask("Destructible", "Indestructible", "Limite", "Player", "TNT");

        playerInput = GetComponent<PlayerInputsHandler>();
        faceManager = GetComponent<FaceManager>();
        statsManager = GameManager.Instance.GetComponent<StatisticsManager>();
        _groundChekLayerMaskWallJump = LayerMask.GetMask("Destructible", "Indestructible", "Limite");
        LookDirection = Vector2.right;
        
        if (StopDuration < 0.01f) SetStopDuration(0.01f);
        tickHoldEat = 1f;
    }

    private void Start()
    {
        _raycastRange = GameManager.Instance.LevelGenerator.Scale * 4;
        _rb2D.gravityScale = _gravityScale != 0 ? _gravityScale : _rb2D.gravityScale;
        _invincibleCd = GameManager.Instance.invincibilityCooldown;
        normalSpriteScale = sprite.transform.localScale;

        UpdatePlayerScale();
        UpdateScoring();
    }

    private void Update()
    {
#if UNITY_EDITOR
        {
            Debug.DrawRay(transform.position - Vector3.forward, inputVectorDirection * 5f, Color.cyan, Time.deltaTime);
        }
#endif

        if (_coll is CapsuleCollider2D)
            castDistance = (_coll.bounds.extents.y - _coll.bounds.extents.x) + .1f;
        else
            throw new Exception("PManager.PCollider is not a CapsuleCollider2D. Please update the code.");
        castRadius = _coll.bounds.extents.x - .05f;
    }

    private void FixedUpdate()
    {
        if (!isJumping)
            _rb2D.gravityScale = _gravityScale;

        if (_isInvincible)
        {
            sprite.color = new Color(1, 1, 1, Mathf.Sin(Time.time * 20));
            insideSprite.color = sprite.color;
        }
        else
        {
            sprite.color = Color.white;
            insideSprite.color = sprite.color;
        }

        switch (_playerSystem.PlayerState)
        {
            case Knockback:
                insideSprite.color = Color.red;
                insideSprite.sortingOrder = 85;
                break;
            default:
                insideSprite.sortingOrder = 70;
                break;
        }

        if (holdEat)
        {
            if (tickHoldEat >= 1f)
            {
                if(inputVectorDirection == Vector2.zero)
                {
                    OnEat(Quaternion.Euler(0, 0, Mathf.Sign(LookDirection.x) * ((float)currentImmobileEatAngle / _maxImmobileEatAngle) * _maxImmobileEatAngle) * LookDirection);
                    currentImmobileEatAngle += 30;
                    if (currentImmobileEatAngle > _maxImmobileEatAngle)
                        currentImmobileEatAngle = 0;
                }
                else
                    OnEat(inputVectorDirection);

                tickHoldEat = 0f;
            }
            else
            {
                tickHoldEat += Time.deltaTime * _eatTickrate;
            }
        }
        else
        {
            currentImmobileEatAngle = 0;
        }
    }
    #endregion

    #region Eat
    /// <summary>
    /// Essaie de manger ce qui se trouve dans direction.
    /// </summary>
    public void OnEat(Vector2 direction)
    {
        _hasEaten = false;

        if (_playerSystem.PlayerState is not Moving)
            return;

        if (direction == Vector2.zero)
        {
            direction = !GroundCheck() ? Vector2.up : LookDirection;
        }

        Vector2 offsetDirection = Quaternion.Euler(0, 0, _eatAngleOffset) * direction;
#if UNITY_EDITOR
        for(int i = 0; i < 5; i++)
        {
            Debug.DrawRay(transform.position - Vector3.forward, _eatDistance * (Quaternion.Euler(0, 0, (-_eatAngle * .5f) + (_eatAngle * ((float)i / 4))) * offsetDirection.normalized), Color.magenta, 0.2f);
        }
#endif

        for (int i = 0; i < 5; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (Quaternion.Euler(0, 0, (-_eatAngle * .5f) + (_eatAngle * ((float)i / 4))) * offsetDirection.normalized), _eatDistance, LayerMask.GetMask("Destructible", "Indestructible", "Limite", "Trap"));
            if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Destructible"))
            {
                _cooldownManager.SetupCoroutine(faceManager.FaceEat);
                hit.transform.parent.GetComponent<Cube_Edible>().GetEaten(transform);
                fullness = Mathf.Clamp(fullness + _filling, 0, 100);
                _hasEaten = true;
                UpdatePlayerScale();
            }
            else if (!holdEat && !GroundCheck()) // S'il touche rien et qu'il n'est pas au sol on ressaie de manger dans le sens du regard cette fois
            {
                direction = LookDirection;
                hit = Physics2D.Raycast(transform.position, direction.normalized, _eatDistance, LayerMask.GetMask("Destructible", "Indestructible", "Limite", "Trap"));

                if (hit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Destructible"))
                {
                    _cooldownManager.SetupCoroutine(faceManager.FaceEat);
                    hit.transform.parent.GetComponent<Cube_Edible>().GetEaten(transform);
                    fullness = Mathf.Clamp(fullness + _filling, 0, 100);
                    _hasEaten = true;
                    UpdatePlayerScale();
                }
            }
        }

        if(_hasEaten) _playerSystem.PlaySound("Player_Eat");
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 _eatAngleOffsetVector = Quaternion.Euler(0, 0, _eatAngleOffset) * Vector2.right;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _eatDistance * (Quaternion.Euler(0, 0, -_eatAngle / 2) * _eatAngleOffsetVector));
        Gizmos.DrawRay(transform.position, _eatDistance * (Quaternion.Euler(0, 0, _eatAngle / 2) * _eatAngleOffsetVector));

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _eatDistance * Vector2.right);
        Gizmos.DrawRay(transform.position, _eatDistance * (Quaternion.Euler(0, 0, _maxImmobileEatAngle) * Vector2.right));
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

    #region WallJump
    public void WallJump()
    {
        playerInput.SetEnableInput(false);
        wallJumping = true;
        wallJumpTimer = 0;
        isSliding = false;
    }

     public void UpdateWallSliding()
    {
        if (!isSliding) return;


        float wallSlidingSpeed = _rb2D.velocity.y;

        if (wallSlidingSpeed < -wallSlidingSpeedMax)
        {
            wallSlidingSpeed = -wallSlidingSpeedMax;
        }

        //TODO: change wall Detach (using timer to detect input)
        _rb2D.velocity = new Vector2(_rb2D.velocity.x, wallSlidingSpeed);


    }

   public void UpdateWallJump()
    {
        if (!wallJumping) return;

        wallJumpTimer += Time.deltaTime;
        if (wallJumpTimer >= wallJumpDuration)
        {
            wallJumping = false;
            return;
        }

        if (isSliding) return;

        _playerSystem.PlaySound("Player_Jump");
        Vector2 wallJumpdir;

        wallJumpdir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * wallJumpAngle), Mathf.Sin(Mathf.Deg2Rad * wallJumpAngle));
        _rb2D.velocity = new Vector2(wallJumpdir.x * dirPlayerToWall * wallJumpForce, wallJumpdir.y * wallJumpForce);
        playerInput.SetEnableInput(true);
        // _rb.velocity = new Vector2(wallJumpdir.x * wallOrientXLeft * wallJumpForce, wallJumpdir.y * wallJumpForce);
    }


    public void UpdateDetectWall()
    {

        Collider2D hit = Physics2D.OverlapCircle(wallCheckRight.position, wallDetectionRadius , _groundChekLayerMaskWallJump);
        
        isPlayerTouchingWall = (hit != null);

        if (isPlayerTouchingWall && !GroundCheck() && !wallJumping && inputVectorDirection.x >= 0.90f || isPlayerTouchingWall && !GroundCheck() && !wallJumping && inputVectorDirection.x <= -0.90f )
        {

            dirPlayerToWall = Mathf.Sign(_rb2D.position.x - hit.transform.position.x);


            isSliding = true;
            
        }
        else
        {

            isSliding = false;
            
        }
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
        if (_playerSystem.PlayerState is Special || _isInvincible) return;
        
        _playerSystem.StopAllEffects();
        _cooldownManager.SetupCoroutine(faceManager.FaceDamage);
        var damageDealerIsAPlayer = false;
        PlayerManager damager = null;
        
        switch(damageDealer)
        {
            case PlayerManager playerManager:
                damager = playerManager; 
                damageDealerIsAPlayer = true;
                _playerSystem.PlaySound("Player_HitShoot");
                break;
            case Cube_Trap:
                damageDealerIsAPlayer = false;
                _playerSystem.PlaySound("Game_Trap");
                break;
            default:
                Debug.Log("Dont know this damage dealer type");
                break;
        }

        fullness = Mathf.Clamp(fullness - damage, 0, 100);

        if (fullness <= 0 && _playerSystem.PlayerState is not Dead)
        {
            _playerSystem.SetState(new Dead(_playerSystem));
            _playerSystem.PlaySound("Player_Kill");
            _playerSystem.PlaySound("Player_Death");
            if(damageDealerIsAPlayer)
                UpdateStats(damager);
            return;
        }
        
        if(damageDealerIsAPlayer)
            UpdateStats(damager, damageScore);
        
        UpdatePlayerScale();

        _playerSystem.SetKnockback(knockBackForce);
    }
    #endregion

    #region Shoot
    public void Shoot()
    {
        var aimDirection = inputVectorDirection;

        _isInvincible = false;

        if (!canShoot)
        {
            Debug.Log($"Attendez le cooldown du tir");
            return;
        }

        if (fullness < _necessaryFood)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            _playerSystem.PlaySound("Menu_ChoosePlayer1");
            return;
        }

        if (aimDirection == Vector2.zero)
            aimDirection = LookDirection;

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
            return;
        }

        ShootProjectile(aimDirection);
        _playerSystem.PlaySound("Player_Shoot");

        fullness = Mathf.Clamp(fullness - _necessaryFood, 0, 100);
        UpdatePlayerScale();

        _cooldownManager.StartCoroutine(_cooldownManager.CooldownShoot());

        _playerSystem.SetState(new Moving(_playerSystem));
    }

    private void ShootProjectile(Vector2 aimDirection)
    {
        _cooldownManager.SetupCoroutine(faceManager.FaceShoot);
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = _playerSystem.PlayerManager;
        projectile.color = color;
        projectile.transform.position = transform.position;
        projectile.gravity = _gravity;
        projectile.bounceForce = _bounceForce;
        projectile.percentageDealt = _inflictedFoodDamage;
        projectile.knockBackForce = _knockBackForce;
        //projectile.ownerVelocityAtLaunch = Rb2D.velocity;

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, _initialSpeed);
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

        if (closestHit.collider.gameObject.layer == LayerMask.NameToLayer("Player") || closestHit.collider.gameObject.layer == LayerMask.NameToLayer("TNT"))
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
        //UpdateTextUI();
    }
    
    /*private void UpdateTextUI()
    {
        if (textUI == null) return;

        var text = fullness;

        switch (text)
        {
            case 100:
                textUI.text = text + "%";
                break;
            case > 0 and < 10:
            case < 1:
                textUI.text = (text.ToString()).Substring(0, 1) + "%";
                break;
            default:
                textUI.text = (text.ToString()).Substring(0, 2) + "%";
                break;
        }
    }*/

    public void UpdateScoring()
    {
        foreach (Stats stats in statsManager.ArrayStats)
        {
            if (stats._playerIndex == playerIndex)
                textUI.text = stats._damageDeal.ToString(CultureInfo.CurrentCulture);
        }
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

    public void SetEnableInputs(bool result)
    {
        playerInput.SetEnableInput(result);
    }

    public void SetFace(Sprite sprite)
    {
        face.sprite = sprite;
    }
    
    private void UpdateStats(PlayerManager damageDealer, int score)
    {
        foreach (var playerStat in statsManager.ArrayStats)
        {
            if (playerStat._playerIndex == damageDealer.playerIndex)
            {
                playerStat._damageDeal += score;
            }
        }
        
        damageDealer.UpdateScoring();
    }
    private void UpdateStats(PlayerManager damageDealer)
    {
        foreach (var playerStat in statsManager.ArrayStats)
        {
            if (playerStat._playerIndex == damageDealer.playerIndex)
            {
                playerStat._damageDeal += killScore;
                playerStat._kill++;
            }
            else if (playerStat._playerIndex == playerIndex)
            {
                playerStat._death++;
            }
        }
        
        damageDealer.UpdateScoring();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        float t = 0;
        while(t < _invincibleCd)
        {
            if (!_isInvincible)
                break;

            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
        }

        _isInvincible = false;
    }
    
    public void SetupJumpEffect()
    {
        StartCoroutine(StreshAndSquash(durationEffectJump, multiplierXJump, multiplierYJump));
    }
    
    
    private IEnumerator StreshAndSquashJump(float duration, float multiplierX, float multiplierY)
    {
        var dur2 = duration / 2;
        var dur4 = duration / 4;
        sprite.transform.DOScale(new Vector3(normalSpriteScale.x * multiplierX, normalSpriteScale.y * multiplierY, normalSpriteScale.z), dur4);
        yield return new WaitForSeconds(dur4);
        sprite.transform.DOScale(new Vector3(normalSpriteScale.x * multiplierY, normalSpriteScale.y * multiplierX, normalSpriteScale.z), dur4 * 3);
    }
    
    private IEnumerator StreshAndSquash(float duration, float multiplierX, float multiplierY)
    {
        var dur = duration / 2;
        sprite.transform.DOScale(new Vector3(normalSpriteScale.x * multiplierX, normalSpriteScale.y * multiplierY, normalSpriteScale.z), dur);
        yield return new WaitForSeconds(dur);
        sprite.transform.DOScale(normalSpriteScale, dur);
    }
#endregion
}