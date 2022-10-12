using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShootProjectile : MonoBehaviour
{
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool canCreatePlatform;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float pressPower;
    [SerializeField] private float holdPower;
    [SerializeField] private float lifetime;
    [SerializeField] private float explosionRadius;
    
    //private PlayerInputAction _playerInput;
    private bool _canShoot;
    private Vector2 _direction;
    private Rigidbody2D _rb;
    private float _cooldown;

    [Header("Slider")]
    [SerializeField] private Slider slider;
    
    
    public AnimationCurve shootDirection;
    private float curveTime;
    

    private void Awake()
    {
        /*_playerInput = new PlayerInputAction();
        _playerInput.Gameplay.ShootDirection.performed += ctx => _direction = ctx.ReadValue<Vector2>();
        _playerInput.Gameplay.ShootDirection.canceled += ctx => _direction = Vector2.zero;
        _playerInput.Gameplay.ShootPress.performed += ctx => Shoot(false);
        _playerInput.Gameplay.ShootHold.performed += ctx => Shoot(true);*/
        _rb = GetComponent<Rigidbody2D>();
        ResetCooldown();
    }

   /* void Shoot(bool wasHolding)
    {
        
        
        
        if (_cooldown < 0) //&& slider.value > 0
        {
            var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            _rb = projectile.GetComponent<Rigidbody2D>();
            if (!wasHolding)
            {
                slider.value -= 0.1f;
                _rb.gravityScale = gravityScale;
                _rb.AddForce(_direction * pressPower, ForceMode2D.Impulse);
            }
            else
            {
                slider.value -= 0.4f;
                _rb.gravityScale = 0;
                _rb.AddForce(_direction * holdPower, ForceMode2D.Impulse);
            }

            projectile.GetComponent<AutoDestroy>().DestroyObj((lifetime));
            ResetCooldown();
        }
    }
*/
    void Update()
    {
        _cooldown -= Time.deltaTime;

        

    }

    private void FixedUpdate()
    {
        curveTime += Time.fixedDeltaTime;
      //  _rb.MovePosition(new Vector2(_rb.position.x,_rb.position.y + shootDirection.Evaluate(curveTime)));
    }

    void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.DrawLine(position, (_direction - (Vector2)position).normalized);
    }

    private void ResetCooldown()
    {
        _cooldown = shootCooldown;
    }

    /*
    private void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,explosionRadius);

        foreach (Collider2D collider in hits) {
            switch (collider.tag) {
                case "CubeEdible":
                    Cube_Edible cube;
                    if (collider.gameObject.TryGetComponent<Cube_Edible>(out cube)) {
                        if (!cube.isManged()) {
                            cube.GetManged();
                        }
                    }
                    break;
                
                case "Player":
                    Mover mover;
                    if (collider.gameObject.TryGetComponent<Mover>(out mover)) {
                        mover.satiety -= mover.satiety * (mover.shootImpactSatietyPercent / 100);
                        Mathf.Clamp(mover.satiety, 0f, 1f);
                        mover._rb.AddForce(-col.contacts[0].normal * mover.shootForce,ForceMode2D.Impulse);
                    }
                    break;
            }
        }
        
        Destroy(transform.gameObject);
    }*/

    private void OnEnable()
    {
        //_playerInput.Gameplay.Enable();
    }

    private void OnDisable()
    {
        //_playerInput.Gameplay.Disable();
    }
}
