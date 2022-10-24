using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShootProjectile : MonoBehaviour
{
    private PlayerManager_JULIEN _playerManager;
    
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float Power;
    [SerializeField] private float explosionRadius;
    [Range(0, 1)] public float shootImpactSatietyPercent = 0.25f;
    [Range(0, 1)] public float shootLooseEat = 0.08f;

    [SerializeField] private Transform endOfAim;
    private bool _canShoot;
    private Rigidbody2D _rb;
    private float _cooldown;
    
    //public AnimationCurve shootDirection;
    //private float curveTime;
    

    private void Awake()
    {
        _playerManager = gameObject.GetComponent<PlayerManager_JULIEN>();
        ResetCooldown();
    }

   public void Shoot()
    {
        Debug.Log("Shoot");
        if (_cooldown < 0 && _playerManager.eatAmount >= 0.08f) 
        {
            var projectile = Instantiate(projectilePrefab, endOfAim.position, Quaternion.identity);
            _rb = projectile.GetComponent<Rigidbody2D>();
            var pr = projectile.GetComponent<Projectile>();
            pr.InitializeValue(explosionRadius, shootImpactSatietyPercent, Power);
            _playerManager.eatAmount -= shootLooseEat;
            _rb.gravityScale = gravityScale;
            _rb.AddForce(_playerManager.InputVector * Power, ForceMode2D.Impulse);
            
            ResetCooldown();
        }
        _playerManager.SetPlayerState(PlayerState.Moving);
    }
   void Update()
    {
        _cooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
       // curveTime += Time.fixedDeltaTime;
      //  _rb.MovePosition(new Vector2(_rb.position.x,_rb.position.y + shootDirection.Evaluate(curveTime)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void ResetCooldown()
    {
        _cooldown = shootCooldown;
    }

    public void Aim()
    {
        Debug.Log("Aim");
        _playerManager.SetPlayerState(PlayerState.Aiming);
    }
}
