using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    
    [Header("Movements")] 
    [SerializeField] private float moveSpeed;
    private Rigidbody2D _rb;
    public float _brakeForce;

    [Header("Simple Jump")] [SerializeField]
    private float jumpForce;

    [SerializeField] private float jumpTime;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool _isGrounded;
    private bool _isJumping;
    private float _jumpCounter;
    private Vector2 _vecGravity;

    [Header("Wall Jump")] 
    [SerializeField] private float jumpCount;
    [SerializeField] private float maxJumpCount;
    [SerializeField] private float wjForce;
    private bool _canWallJump;
    private Vector3 _normalVec;

    /*[Header("Shoot")]
    private float _cooldown;
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    private Rigidbody2D _rbProjectile;
    [SerializeField] private bool canCreatePlatform;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float shootPower;
    [SerializeField] private float lifetime;*/

    [Header("Dash")]
    private TrailRenderer _trailRenderer;
    [SerializeField] private float looseEatForce = 1f;
    private ScaleEat _scaleEat;
    IEnumerator _dashCoroutine;
    private bool _isDashing;
    private bool _canDash = true;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private float _normalGravity;

    private void Awake()
    {
        _playerManager = gameObject.GetComponent<PlayerManager>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        //ResetShootCooldown();
        _playerManager.SetPlayerState(PlayerState.Moving);
        _vecGravity = new Vector2(0, -Physics2D.gravity.y);
        _scaleEat = GetComponent<ScaleEat>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    void Update()
    {
        //_cooldown -= Time.deltaTime; cooldown shoot
    }

    private void FixedUpdate()
    {
        _isGrounded = IsGrounded();

        if (_playerManager.State == PlayerState.Moving)
            Move();
        else 
            Brake();
        
        if (_isDashing) 
        {
             _playerManager.eatAmount -= Time.deltaTime * looseEatForce;
            _rb.AddForce(_playerManager.InputVector * dashForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.2f), CapsuleDirection2D.Horizontal, 0,groundLayer);
    }
    private void Move()
    {
        if (_playerManager.InputVector == Vector2.zero ) 
            Brake();

        float Vx = _playerManager.InputVector.x * moveSpeed + _rb.velocity.x;
        Vx = Mathf.Clamp(Vx, -moveSpeed, moveSpeed);
        _rb.velocity = new Vector2(Vx, _rb.velocity.y);
        
    }

    private void Brake()
    {
        if (Mathf.Abs(_rb.velocity.x) >= 0.1f && _isGrounded)
            _rb.velocity += new Vector2(-(_rb.velocity.x / Mathf.Abs(_rb.velocity.x)) * _brakeForce, 0);
        //_rb.AddForce(new Vector2(-(_rb.velocity.x / Mathf.Abs(_rb.velocity.x)) * _brakeForce, 0));
        else if (Mathf.Abs(_rb.velocity.x) < 0.1f && _isGrounded)
            _rb.velocity = new Vector2(0, _rb.velocity.y);
    }

    public void Jump()
    {
        if (_playerManager.State == PlayerState.Moving)
        {
            if (_canWallJump)
            {
                if (!_isGrounded)
                {
                    Vector3 wjForceVec = _normalVec * wjForce;
                    wjForceVec.y = jumpForce;
                    _rb.velocity = Vector2.zero;
                    _rb.AddForce(wjForceVec, ForceMode2D.Impulse);
                }

                return;
            }
        
            if (_isGrounded)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                _isJumping = true;
                _jumpCounter = 0;
            }

            if (_rb.velocity.y > 0 && _isJumping)
            {
                _jumpCounter += Time.deltaTime;
            
                if (_jumpCounter > jumpTime) _isJumping = false;

                var t = _jumpCounter / jumpTime;
                var currentJumpM = jumpMultiplier;

                if (t > 0.5f)
                    currentJumpM = jumpMultiplier * (1 - t);

                //_rb.velocity += _vecGravity * currentJumpM * Time.deltaTime;
            }
        
            if (_rb.velocity.y < 0)
            {
                _rb.velocity -= _vecGravity * (fallMultiplier * Time.deltaTime); 
            }
        }
    }

    public void StopJump()
    {
        _isJumping = false;
        _jumpCounter = 0;

        if (_rb.velocity.y > 0)
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.6f);
    }
    
    /*private void ResetShootCooldown()
    {
        _cooldown = shootCooldown;
    }
    
    public void Shoot()
    {
        if (_cooldown < 0) //&& slider.value > 0
        {
            PlayerManager.Instance.SetPlayerState(PlayerState.Shooting);
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            _rbProjectile = projectile.GetComponent<Rigidbody2D>();
            //slider.value -= 0.1f;
            _rbProjectile.gravityScale = gravityScale; 
            _rbProjectile.AddForce(_inputVector * shootPower, ForceMode2D.Impulse);

            projectile.GetComponent<AutoDestroy>().DestroyObj((lifetime));
            ResetShootCooldown();
        }
        PlayerManager.Instance.SetPlayerState(PlayerState.Moving);
    }
    

    public void Aim()
    {
        PlayerManager.Instance.SetPlayerState(PlayerState.Aiming);
    }*/

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag.Contains("Jumpable")) {
            _canWallJump = true;
           _normalVec = collision.contacts[0].normal;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag.Contains("Jumpable"))
            _canWallJump = false;
    }
    
    public void Dash()
    {
        if (_canDash) //&& _scaleEat.NbEaten >= 200f
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
        _playerManager.SetPlayerState(PlayerState.Moving);
        _trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;

    }
}