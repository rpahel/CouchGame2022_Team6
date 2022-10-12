using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Mover : MonoBehaviour //Rename to playerController
{
    [Header("Player State")] private PlayerState _state;

    enum PlayerState
    {
        Aiming,
        Shooting,
        Dashing,
        WallJumping,
        Moving,
    }

    [Header("Movements")] [SerializeField] private float moveSpeed;
    private Rigidbody2D _rb;
    private Vector2 _inputVector = Vector2.zero;

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
    private bool _isWallJumping;
    [SerializeField] private float maxJumpCount;
    [SerializeField] private float wjForce;
    private bool _canWallJump;
    private Vector3 _normalVec;

    [Header("Shoot")]
    private float _cooldown;
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    private Rigidbody2D _rbProjectile;
    [SerializeField] private bool canCreatePlatform;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float shootPower;
    [SerializeField] private float lifetime;
    
    [Header("Eat")]
    public Transform pointeur;       
    public Transform pointeurBase;   
    public float reach = 1f;  
    [SerializeField, Range(0f, 1f)]
    private float filling = 0.12f;
    private bool canEat = true;
    private float satiety = 0f;   
    [SerializeField, Range(0f, .5f)]
    private float eatCooldown = 0.5f;
    private Coroutine cooldownCoroutine;
    private float  angle;    
    
    [Header("Dash")]
    private TrailRenderer _trailRenderer;
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
        _rb = gameObject.GetComponent<Rigidbody2D>();
        ResetShootCooldown();
        _state = PlayerState.Moving;
        _vecGravity = new Vector2(0, -Physics2D.gravity.y);
        
        pointeur.gameObject.SetActive(false);
        _scaleEat = GetComponent<ScaleEat>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }
    
    public void SetInputVector(Vector2 direction)
    {
        _inputVector = direction;
        pointeur.gameObject.SetActive(_inputVector.sqrMagnitude > 0.1f ? true : false);
        angle = Mathf.Atan2(_inputVector.y, _inputVector.x);
    }

    void Update()
    {
        _cooldown -= Time.deltaTime;
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }

    private void FixedUpdate()
    {
        _isGrounded = IsGrounded();
            
        if(_state == PlayerState.Moving)
            Move();
        
        if (_isDashing) 
        {
            //Perdre de la matière selon time.deltatime
            _scaleEat.NbEaten -= Time.deltaTime;
            Debug.Log(_scaleEat.NbEaten);
            _rb.AddForce(_inputVector * dashForce, ForceMode2D.Impulse);
        }

        if (_isWallJumping)
        {
            /*Vector3 wjForceVec = _normalVec * wjForce;
            wjForceVec.y = jumpForce;
            _rb.AddForce(wjForceVec, ForceMode2D.Impulse);*/
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.2f), CapsuleDirection2D.Horizontal, 0,groundLayer);
    }
    private void Move()
    {
        _rb.velocity = new Vector2(_inputVector.x * moveSpeed, _rb.velocity.y);
    }

    public void Jump()
    {
        if (_state == PlayerState.Moving)
        {
            if (_canWallJump)
            {
                if (!_isGrounded)
                {

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
    
    private void ResetShootCooldown()
    {
        _cooldown = shootCooldown;
    }
    
    public void Shoot()
    {
        if (_cooldown < 0) //&& slider.value > 0
        {
            _state = PlayerState.Shooting;
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            _rbProjectile = projectile.GetComponent<Rigidbody2D>();
            //slider.value -= 0.1f;
            _rbProjectile.gravityScale = gravityScale; 
            _rbProjectile.AddForce(_inputVector * shootPower, ForceMode2D.Impulse);

            projectile.GetComponent<AutoDestroy>().DestroyObj((lifetime));
            ResetShootCooldown();
        }
        _state = PlayerState.Moving;
    }
    
    void OnDrawGizmos()
    {
        if (_state != PlayerState.Aiming) return;
        var position = transform.position;
        Gizmos.DrawLine(position, (_inputVector - (Vector2)position).normalized);
    }

    public void Aim()
    {
        _state = PlayerState.Aiming;
    }
    private void Eat(Cube_Edible cubeMangeable)
    {
        cubeMangeable.GetManged();
        satiety += filling;
        satiety = Mathf.Clamp(satiety, 0f, 1f);
        canEat = false;
        cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    public void TryEat()
    {
        Debug.Log("try Eat");
        if (!canEat)
        {
            print("I'm on eating cooldown !");
            return;
        }

        if (satiety > 1f)
        {
            print("I'm full !!");
            return;
        }
        
        RaycastHit hit;
        if (Physics.Raycast(pointeurBase.position, _inputVector, out hit, reach))
        {
            if (hit.transform.parent.CompareTag("CubeEdible"))
            {
                Cube_Edible cubeMangeable;
                if (hit.transform.parent && hit.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
                {
                    Eat(cubeMangeable);
                }
                else
                    print("Pas de Raf_CubeMangeable dans le cube vis�.");
            }
        }
    }
    
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
        StopCoroutine(cooldownCoroutine);
    }
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

            _state = PlayerState.Dashing;
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
        _state = PlayerState.Moving;
        _trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;

    }
}