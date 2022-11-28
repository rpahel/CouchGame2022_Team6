using System;
using Data;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    private PlayerManager _playerManager;
    private PlayerMovement _movement;
    
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float Power;
    [SerializeField] private float cubeBounce;
    [SerializeField] private float explosionRadius;
    [Range(0, 100)] public float shootImpactSatietyPercent;
    [Range(0, 100)] public float shootLooseEat;

    [SerializeField] private Transform endOfAim;
    private bool _canShoot;
    private Rigidbody2D _rb;
    private float _cooldown;


    private void Awake()
    {
        _playerManager = gameObject.GetComponent<PlayerManager>();
        _movement = gameObject.GetComponent<PlayerMovement>();
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
            pr.InitializeValue( shootImpactSatietyPercent, Power, cubeBounce,this.gameObject);
            _playerManager.eatAmount -= shootLooseEat / 100;
            _rb.gravityScale = gravityScale;
            var dir = _playerManager.InputVector;
            if (dir == Vector2.zero)
                dir = _movement.lookAtRight ? Vector2.right : Vector2.left;

            _rb.AddForce(dir * Power, ForceMode2D.Impulse);
            
            ResetCooldown();
        }
        _playerManager.SetPlayerState(PlayerState.Moving);
    }
   void Update()
    {
        _cooldown -= Time.deltaTime;
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
