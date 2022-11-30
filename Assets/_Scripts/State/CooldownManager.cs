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
        playerSystemManager.canShoot = false;
        yield return new WaitForSeconds(playerSystemManager.CooldownShoot);
        playerSystemManager.canShoot = true;
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

    public IEnumerator MoverOverAnimation(Vector2 endPosition)
    {
        Vector2 iniPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            if (playerSystem.PlayerState is Knockback)
                break;

            playerSystem.PlayerSystemManager.Rb2D.velocity = Vector2.zero;
            transform.position = DOVirtual.EasedValue(iniPos, endPosition, t, Ease.OutBack, 2f);
            t += Time.deltaTime * 4f;
            yield return null;
        }

        if (playerSystem.PlayerState is not Knockback)
        {
            transform.position = endPosition;
        }
    }

    public IEnumerator ActiveInputDelay(float duration)
    {
        playerSystem.PlayerInputsState.SetEnableInput(false);
        yield return new WaitForSeconds(duration);
        playerSystem.PlayerInputsState.SetEnableInput(true);
        playerSystem.SetState(new Moving(playerSystem));
    }
}
