using System.Collections;
using UnityEngine;
using System;
using CustomMaths;
using Data;
using DG.Tweening;

public class Shooting : State 
{
    public Shooting(PlayerSystem playerSystem) : base(playerSystem)
    {
    }
    
    public override void Start()
    {
        playerSystem.PlayerSystemManager.AimPivot.gameObject.SetActive(false);
        Shoot();
    }
    
    private void Shoot()
    {
        var aimDirection = playerSystem.PlayerSystemManager.inputVectorDirection;

        if (playerSystem.PlayerSystemManager.cdTimer > 0)
        {
            Debug.Log($"Attendez le cooldown du tir ({playerSystem.PlayerSystemManager.cdTimer:0.000}s)");
            return;
        }

        if(playerSystem.PlayerSystemManager.fullness < playerSystem.PlayerSystemManager.NecessaryFood)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }

        if(aimDirection == Vector2.zero)
            aimDirection = playerSystem.PlayerSystemManager.LookDirection;

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
            return;
        }

        ShootProjectile(aimDirection);

        playerSystem.PlayerSystemManager.fullness = Mathf.Clamp(playerSystem.PlayerSystemManager.fullness - playerSystem.PlayerSystemManager.NecessaryFood, 0, 100);
        playerSystem.PlayerSystemManager.UpdatePlayerScale();

        playerSystem.PlayerSystemManager.cdTimer = playerSystem.PlayerSystemManager.CooldownShoot;
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.CooldownShoot());
        
        playerSystem.SetState(new Moving(playerSystem));
    }
    
    private void ShootProjectile(Vector2 aimDirection)
    {
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = playerSystem.PlayerSystemManager;
        projectile.color = playerSystem.PlayerSystemManager.color;
        projectile.transform.position = playerSystem.transform.position;
        projectile.gravity = playerSystem.PlayerSystemManager.Gravity;
        projectile.bounceForce = playerSystem.PlayerSystemManager.BounceForce;
        projectile.percentageDealt = playerSystem.PlayerSystemManager.InflictedFoodDamage;
        projectile.knockBackForce = playerSystem.PlayerSystemManager.KnockBackForce;
        projectile.ownerVelocityAtLaunch = playerSystem.PlayerSystemManager.Rb2D.velocity;

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, playerSystem.PlayerSystemManager.InitialSpeed);
    }
    public override void OnCollision(Collision2D col)
    {
        Debug.Log("collision");
    }
    
    private bool IsThereEnoughSpace(Vector2 aimDirection)
    {
        Vector2 rayOrigin = (Vector2)playerSystem.transform.position + (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin, aimDirection, playerSystem.PlayerSystemManager.raycastRange);

        #if UNITY_EDITOR
            if (!hit1)
                Debug.DrawRay(rayOrigin, aimDirection * playerSystem.PlayerSystemManager.raycastRange, Color.red, 1f);
            else
                Debug.DrawLine(rayOrigin, hit1.point, Color.red, 1f);
        #endif

        rayOrigin = (Vector2)playerSystem.transform.position - (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, aimDirection, playerSystem.PlayerSystemManager.raycastRange);

        #if UNITY_EDITOR
            if(!hit2)
                Debug.DrawRay(rayOrigin, aimDirection * playerSystem.PlayerSystemManager.raycastRange, Color.red, 1f);
            else
                Debug.DrawLine(rayOrigin, hit2.point, Color.red, 1f);
        #endif

        RaycastHit2D closestHit;

        if (hit1 && hit2)
        {
            closestHit = hit1.distance < hit2.distance ? hit1 : hit2;
        }
        
        else if (hit1)
            closestHit = hit1;
        else
            closestHit = hit2;

        if (!closestHit) return true;
        
        if (closestHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            return true;
        
        RaycastHit2D[] hits = CustomPhysics.SquareCast((Vector2)closestHit.transform.position + GameManager.Instance.LevelGenerator.Scale * closestHit.normal, GameManager.Instance.LevelGenerator.Scale * .9f);
        
        foreach(RaycastHit2D hit2D in hits)
        {
            if (hit2D)
                return false;
        }

        return true;
    }
}


