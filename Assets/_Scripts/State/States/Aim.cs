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
    
    public override IEnumerator Start()
    {
        if (playerSystem.PlayerSystemManager.fullness < playerSystem.PlayerSystemManager.NecessaryFood) yield break;
        
        //playerSystem.PlayerSystemManager.PlayerState = PLAYER_STATE.SHOOTING;
        playerSystem.PlayerSystemManager.AimPivot.gameObject.SetActive(true);

        yield break;
    }

    public override void FixedUpdate()
    {
        playerSystem.PlayerSystemManager.AimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, playerSystem.PlayerSystemManager.inputVectorDirection != Vector2.zero ? playerSystem.PlayerSystemManager.inputVectorDirection : playerSystem.PlayerSystemManager.LookDirection) - 90f);
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking());
    }
    

    public override void OnShoot()
    {
        playerSystem.SetState(new Shooting(playerSystem));
    }

    public override void OnCollision(Collision2D col)
    {
        Debug.Log("collision");
    }

}


