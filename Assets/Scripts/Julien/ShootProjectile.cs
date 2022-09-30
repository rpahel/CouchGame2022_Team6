using System;
using System.Collections;
using System.Collections.Generic;
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

    private PlayerInput _playerInput;
    private bool _canShoot;
    private Vector2 _direction;
    private Rigidbody2D _rb;
    private float _cooldown;

    [Header("Slider")]
    [SerializeField] private Slider slider;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Gameplay.ShootDirection.performed += ctx => _direction = ctx.ReadValue<Vector2>();
        _playerInput.Gameplay.ShootDirection.canceled += ctx => _direction = Vector2.zero;
        _playerInput.Gameplay.ShootPress.performed += ctx => Shoot(false);
        _playerInput.Gameplay.ShootHold.performed += ctx => Shoot(true);

        ResetCooldown();
    }

    void Shoot(bool wasHolding)
    {
        if (_cooldown < 0 && slider.value > 0)
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

    void Update()
    {
        _cooldown -= Time.deltaTime;
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

    private void OnEnable()
    {
        _playerInput.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Gameplay.Disable();
    }
}
