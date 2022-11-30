using System.Collections;
using UnityEngine;
using System;
using Data;
using DG.Tweening;
public class Aim : State 
{
    public Aim(PlayerSystem playerSystem) : base(playerSystem)
    {
    }
    
    public override void Start()
    {
        playerSystem.PlayerSystemManager.AimPivot.gameObject.SetActive(true);
    }

    public override void FixedUpdate()
    {
        playerSystem.PlayerSystemManager.AimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, playerSystem.PlayerSystemManager.inputVectorDirection != Vector2.zero ? playerSystem.PlayerSystemManager.inputVectorDirection : playerSystem.PlayerSystemManager.LookDirection) - 90f);
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking()); // TODO : Attention au probleme des coroutines
    }

    public override void OnShoot()
    {
        playerSystem.PlayerSystemManager.AimPivot.gameObject.SetActive(false);
        playerSystem.PlayerSystemManager.Shoot();
        playerSystem.SetState(new Moving(playerSystem));
    }
}


