using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Data;
 
public class PlayerMovement : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private PlayerManager _playerManager;
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
    private TrailRenderer _trailRenderer;
    [SerializeField] private Collider2D playerCollider2D;
    [SerializeField] private float looseEatForce = 1f;
    private ScaleEat _scaleEat;
    private IEnumerator _dashCoroutine;
    private bool _isDashing;
    public bool _canDash = false;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private float _normalGravity;
    public bool _canHit = false;
    #endregion
 
    #region Unity_Functions

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        _playerManager.SetPlayerState(PlayerState.Moving);
        dureeAvantArret = dureeAvantArret < 0.01f ? 0.01f : dureeAvantArret;

        echelleDeGravité = echelleDeGravité != 0 ? echelleDeGravité : _rb.gravityScale;
    }
 
    private void Update()
    {
        if(dureeAvantArret < 0.01f)
            dureeAvantArret = 0.01f;

        if (_playerManager.InputVector == Vector2.zero)
            pointeurTransform.rotation = Quaternion.Euler(0, 0, lookAtRight ? 0 : 180);
    }
 
    private void FixedUpdate()
    {
        #if UNITY_EDITOR
            _rb.gravityScale = echelleDeGravité;
        #endif

        if (_isDashing) 
        {
            _rb.AddForce(_playerManager.InputVector * dashForce, ForceMode2D.Impulse);
        }

        //_playerManager.SetPlayerState(groundCheck ? PlayerState.Moving : PlayerState.Falling);

        _isGrounded = IsGrounded();
 
        if (_playerManager.State == PlayerState.Moving)
            Deplacement();
        
        OnMove();
 
        if (holdJump && groundCheck)
            OnJump();
    }
    
    #endregion
 
    #region Custom_Functions
    public void OnMove()
    {
        if (Mathf.Abs(_playerManager.InputVector.x) <= deadZone)
        {
            _inputVectorWithDeadZone = Vector2.zero;
            return;
        }
        else
            _inputVectorWithDeadZone = _playerManager.InputVector;

        lookAtRight = _playerManager.InputVector.x switch
        {
            < 0 => false,
            > 0 => true,
            _ => lookAtRight
        };
 
        if(freinage != null)
        {
            StopCoroutine(freinage);
            freinage = null;
        }
    }
 
    public void OnJump()
    {
        if (IsGrounded())
        {
            Jump();
        }
        else
        {
            Debug.Log("You can't jump, you're not on solid ground.");
        }
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
            _rb.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), _rb.velocity.x);
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
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y);

        var velocity = _rb.velocity;
        velocity += new Vector2(_inputVectorWithDeadZone.x, 0) * Time.fixedDeltaTime * 100f;
        
        _rb.velocity = velocity;
        _rb.velocity = new Vector2(Mathf.Clamp(velocity.x, -vitesseMax, vitesseMax), _rb.velocity.y);
    }
 
    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _rb.velocity += new Vector2(0, forceDeSaut);
    }
    #endregion
    
    public void Dash()
    {
        if (_canDash)
        {
            if (_dashCoroutine != null) 
            {
                StopCoroutine(_dashCoroutine);
            }

            _playerManager.SetPlayerState(PlayerState.Dashing);
            _dashCoroutine = Dash(dashTime, dashCooldown);
            StartCoroutine(_dashCoroutine);
        }
    }
    
    private IEnumerator Dash(float dashDuration, float dashCooldown)
    {
        _canHit = true;
        gameObject.layer = LayerMask.NameToLayer("Dash");
        var originalVelocity = _rb.velocity;
        var originalGravityScale = _rb.gravityScale;
        _isDashing = true;
        _trailRenderer.emitting = true;
        _canDash = false;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashDuration);
        _isDashing = false;
        _rb.gravityScale = _normalGravity;
        _rb.velocity = originalVelocity;
        _rb.gravityScale = originalGravityScale;
        _canHit = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        _playerManager.SetPlayerState(PlayerState.Moving);
        _trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;

    }
}