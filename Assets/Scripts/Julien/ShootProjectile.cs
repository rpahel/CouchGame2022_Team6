using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShootProjectile : MonoBehaviour
{
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private bool canCreatePlatform;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField, Range(1, 100)] private float mass;
    
    private bool _canShoot;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _rb.mass = mass;
        _rb.gravityScale = gravityScale;
    }

    void Shoot()
    {
        
    }
}
