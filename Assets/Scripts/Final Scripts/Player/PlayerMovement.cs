using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Data;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private PlayerManager _playerManager;
    private PlayerInputHandler _playerInputHandler;
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
    [SerializeField] private Transform pointeurTransform;
    
    //============================ // Pour le saut
    private bool _isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask groundLayer2;
    [SerializeField] private LayerMask groundLayer3;

    private Rigidbody2D _rb;
    private CapsuleCollider2D _capsuleCollider; //Serializefield car il y a deux collider sur le player
    //============================
    private Vector2 _inputVectorWithDeadZone;
 
    //============================
    private Coroutine freinage;
    
    //============================
    private bool holdJump;
    public bool HoldJump { set => holdJump = value; }
 
    //============================
    
    public bool lookAtRight; // à ne pas confondre avec aimDirection
    
    //============================
    [Header("Dash")]
    [SerializeField] private DashType dashType;
    [SerializeField] private DashLoading dashLoading;
    private TrailRenderer _trailRenderer;
    [SerializeField] private float looseEatForce = 1f;
    private ScaleEat _scaleEat;
    private IEnumerator _dashCoroutine;
    private bool _isDashing;
    public bool _canDash = false;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashTime;
    private float dashTimeMultiplier;
    [SerializeField] private float dashCooldown;
    private Vector2 originalVelocity;
    private float originalGravityScale;
    private float _normalGravity;
    public bool _canHit = false;
    [SerializeField] private float LooseEatValue = 0.7f;
    [SerializeField, Range(1f, 2f)] private float WidthMultiplier = 2f;
    [SerializeField, Range(1f, 2f)] private float LengthMultiplier = 2f;
    [SerializeField] private BoxCollider2D dashCollider;
    
    //============================
    [Header("Vfx")] 
    [SerializeField] private VisualEffect visualEffect;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f), SerializeField]
    private Color colorVFX1;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f), SerializeField]
    private Color colorVFX2;
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f), SerializeField]
    private Color colorVFX3;
    #endregion

    #region Unity_Functions

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        dureeAvantArret = dureeAvantArret < 0.01f ? 0.01f : dureeAvantArret;
        echelleDeGravité = echelleDeGravité != 0 ? echelleDeGravité : _rb.gravityScale;
        visualEffect.Stop();
    }
 
    private void Update()
    {
        if(dureeAvantArret < 0.01f)
            dureeAvantArret = 0.01f;

        if (_playerManager.InputVector == Vector2.zero)
            pointeurTransform.rotation = Quaternion.Euler(0, 0, lookAtRight ? 0 : 180);

        CheckHoldValue();
    }
 
    private void FixedUpdate()
    {
        #if UNITY_EDITOR
        if(_playerManager.State == PlayerState.Moving)
            _rb.gravityScale = echelleDeGravité;
        #endif

        if (_isDashing) 
        {
            _rb.AddForce(_playerManager.InputVector * dashForce, ForceMode2D.Impulse);
            return;
        }

        _isGrounded = IsGrounded();

        if (_playerManager.State == PlayerState.STUNNED) return;
        
        float castRadius = transform.localScale.x * .5f - .05f;
        float castDistance = _capsuleCollider.size.y * transform.localScale.y * .25f + .3f;

        if (_playerManager.State != PlayerState.KNOCKBACKED)
        {
            if (_playerManager.State != PlayerState.Aiming)
                _playerManager.SetPlayerState(groundCheck ? PlayerState.Moving : PlayerState.Falling);

            Deplacement();
            OnMove();

            if (holdJump && groundCheck)
                OnJump();

            _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -vitesseMax, vitesseMax), _rb.velocity.y);
        }
        /*else
        {
            if (_rb.velocity.y <= 0 && groundCheck && _playerManager.State != PlayerState.Aiming) TODO:: VOIR AVEC RAF CAR PB TRAP
            {
                _playerManager.SetPlayerState(PlayerState.Moving);
            }
        }*/
    }
    
    #endregion
 
    #region Custom_Functions

    private void OnMove()
    {
        if (Mathf.Abs(_playerManager.InputVector.x) <= deadZone || _playerManager.State == PlayerState.Aiming)
        {
            _inputVectorWithDeadZone = Vector2.zero;
            return;
        }

        _inputVectorWithDeadZone = _playerManager.InputVector;

        lookAtRight = _playerManager.InputVector.x switch
        {
            < 0 => false,
            > 0 => true,
            _ => lookAtRight
        };

        if (freinage == null) return;
        
        StopCoroutine(freinage);
        freinage = null;
    }
 
    public void OnJump()
    {
        if (_isGrounded)
            Jump();
        
        else
            Debug.Log("You can't jump, you're not on solid ground.");
        
    }
    
    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.2f), CapsuleDirection2D.Horizontal, 0,groundLayer | groundLayer2 | groundLayer3);
    }
 
    IEnumerator Freinage()
    {
        float iniVelocityX = _rb.velocity.x;
        float t = 0;
        while(t < 1f)
        {
            _rb.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), _rb.velocity.y);
            t += Time.fixedDeltaTime / dureeAvantArret;
            yield return new WaitForFixedUpdate();
        }
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        freinage = null;
    }
 
    private void Deplacement()
    {
        if (_inputVectorWithDeadZone == Vector2.zero)
        {
            if (freinage == null && _rb.velocity.x != 0)
            {
                freinage = StartCoroutine(Freinage());
            }
        }
 
        if ((_inputVectorWithDeadZone.x / Mathf.Abs(_inputVectorWithDeadZone.x)) + (_rb.velocity.x / Mathf.Abs(_rb.velocity.x)) == 0)
            _rb.velocity = new Vector2(-_rb.velocity.x, _rb.velocity.y);
        
        _rb.velocity += new Vector2(_inputVectorWithDeadZone.x, 0) * (Time.fixedDeltaTime * 100f);
        
        //_rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -vitesseMax, vitesseMax), _rb.velocity.y);
    }
 
    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _rb.velocity += new Vector2(0, forceDeSaut);
    }
    #endregion

    public void Dash(float holdValue)
    {
        SetDashWithHold(holdValue);

        if (_dashCoroutine != null) 
        {
            StopCoroutine(_dashCoroutine);
        }

        _playerManager.SetPlayerState(PlayerState.Dashing);
        visualEffect.SetVector4("Color", colorVFX1);
        visualEffect.Stop();
        _dashCoroutine = dashType == DashType.Normal ? DashCoroutine(dashTimeMultiplier) : InfiniteDashCoroutine();
        StartCoroutine(_dashCoroutine);
    }

    private void CheckHoldValue()
    {
        switch (_playerInputHandler.HoldCooldown)
        {
            case > 0 and <= 1:
                visualEffect.Play();
                break;
            case > 1 and <= 2:
                visualEffect.SetVector4("Color", colorVFX2);
                break;
            case > 2:
                visualEffect.SetVector4("Color", colorVFX3);
                break;
        }
    }

    private void SetDashWithHold(float hold)
    {
        switch (hold)
        {
            case <= 1f:
            case > 1f and < 2f:
                _playerManager.eatAmount -= LooseEatValue;
                break;
            default:
                _playerManager.eatAmount -= LooseEatValue;
                break;
        }


        switch (dashLoading)
        {
            //Increase Length
            case DashLoading.IncreaseLength:
            {
                var holdClamp = Mathf.Clamp(hold, 0f, 2f);
                var lengthValue = Mathf.Lerp(dashTime, LengthMultiplier * dashTime, holdClamp / 2);
                dashTimeMultiplier = lengthValue;
                Debug.Log("DashTime" + dashTimeMultiplier);
                break;
            }
            //Increase collider 
            case DashLoading.IncreaseWidth:
            {
                dashTimeMultiplier = dashTime;
                var holdClamp = Mathf.Clamp(hold, 0f, 2f);
                var sizeValue = Mathf.Lerp(1.5f, WidthMultiplier * 1.5f, holdClamp / 2);           
                dashCollider.size = new Vector2(sizeValue, sizeValue);
                Debug.Log("DashSize" + sizeValue);
                break;
            }
        }
    }
    
    private IEnumerator DashCoroutine(float dashDuration)
    {
        _playerManager.DisableInputs();
        _canHit = true;
        gameObject.layer = LayerMask.NameToLayer("Dash");
        originalVelocity = _rb.velocity;
        originalGravityScale = _rb.gravityScale;
        _isDashing = true;
        _trailRenderer.emitting = true;
        _canDash = false;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashDuration);
        EndDash();
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    private IEnumerator InfiniteDashCoroutine()
    {
        _playerManager.DisableInputs();
        _canHit = true;
        gameObject.layer = LayerMask.NameToLayer("Dash");
        originalVelocity = _rb.velocity;
        originalGravityScale = _rb.gravityScale;
        _isDashing = true;
        _trailRenderer.emitting = true;
        _canDash = false;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        yield break;
    }
    private IEnumerator EndInfiniteDash()
    {
        EndDash();
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_playerManager.State == PlayerState.Dashing && dashType == DashType.Infinite)
        {
            StartCoroutine(EndInfiniteDash());
        }
            
    }

    private void EndDash()
    {
        _isDashing = false;
        _rb.gravityScale = _normalGravity;
        _rb.velocity = originalVelocity;
        _rb.gravityScale = originalGravityScale;
        _canHit = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        _playerManager.SetPlayerState(PlayerState.Moving);
        _trailRenderer.emitting = false;
        _playerManager.EnableInputs();
    }
}