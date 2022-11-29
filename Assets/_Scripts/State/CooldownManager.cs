using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private PlayerSystemManager playerSystemManager;
    private PlayerSystem playerSystem;
    private void Awake()
    {
        playerSystemManager = GetComponent<PlayerSystemManager>();
        playerSystem = GetComponent<PlayerSystem>();
    }
    
    public IEnumerator CooldownShoot()
    {
        while (playerSystemManager.cdTimer > 0)
        {
            playerSystemManager.cdTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        playerSystemManager.cdTimer = 0;
    }
    
    public IEnumerator Braking()
    {
        float iniVelocityX = playerSystem.PlayerSystemManager.Rb2D.velocity.x;
        float t = 0;
        while (t < 1f)
        {

            playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), playerSystem.PlayerSystemManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / playerSystem.PlayerSystemManager.StopDuration;
            yield return new WaitForFixedUpdate();
        }

        //if (playerSystem.PlayerSystemManager.PlayerState != PLAYER_STATE.KNOCKBACKED && playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.DASHING) //A quoi sert ca
           // playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(0, playerSystem.PlayerSystemManager.Rb2D.velocity.y);
        
        playerSystem.PlayerSystemManager.brakingCoroutine = null;
    }
}
