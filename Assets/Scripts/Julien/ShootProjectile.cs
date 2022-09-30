using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShootProjectile : MonoBehaviour
{
    [Header("Options Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool canCreatePlatform;
    [SerializeField, Range(0, 1)] private float gravityScale;
    [SerializeField, Range(1, 100)] private float mass;
    [SerializeField] private float lifetime;
    
    private bool _canShoot;
    private Vector2 mousePos;
    
    private Rigidbody2D _rb;

    void OnShoot()
    {
        Debug.Log("shoot");
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        _rb = projectile.GetComponent<Rigidbody2D>();
        _rb.mass = mass;
        _rb.gravityScale = gravityScale;
        projectile.GetComponent<AutoDestroy>().Destroy((lifetime));
    }

    void Update()
    {
        mousePos = Input.mousePosition;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, (mousePos - (Vector2)transform.position).normalized);
    }
}
