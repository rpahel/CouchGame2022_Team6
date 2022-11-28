using System.Collections;
using UnityEngine;
using System;
using Data;
using DG.Tweening;
public class Moving : State 
{
    public Moving(PlayerSystem playerSystem) : base(playerSystem)
    {
    }

    public override IEnumerator Awake()
    {
        playerSystem.PlayerSystemManager.stopDuration = playerSystem.PlayerSystemManager.stopDuration < 0.01f ? 0.01f : playerSystem.PlayerSystemManager.stopDuration;
        playerSystem.PlayerSystemManager.LookDirection = Vector2.right;
        playerSystem.PlayerSystemManager.castRadius = playerSystem.transform.localScale.x * .5f - .05f;
        yield break;
    }
    public override IEnumerator Start()
    {
        playerSystem.PlayerSystemManager.Rb2D.gravityScale = playerSystem.PlayerSystemManager.gravityScale != 0 ? playerSystem.PlayerSystemManager.gravityScale : playerSystem.PlayerSystemManager.Rb2D.gravityScale;

        if (playerSystem.PlayerSystemManager.PCollider is CapsuleCollider2D)
            playerSystem.PlayerSystemManager.castDistance = (playerSystem.PlayerSystemManager.PCollider as CapsuleCollider2D).size.y * playerSystem.transform.localScale.y * .25f + .3f;
        else
            throw new Exception("PManager.PCollider is not a CapsuleCollider2D. Please update the code.");
        yield break;
    }

    public override IEnumerator Update()
    {
          
        #if UNITY_EDITOR
        {
            if(playerSystem.PlayerSystemManager.stopDuration < 0.01f)
                playerSystem.PlayerSystemManager.stopDuration = 0.01f;
        }
        #endif

        yield break;
    }

    private void FixedUpdate()
    {
        playerSystem.PlayerSystemManager.Rb2D.gravityScale = playerSystem.PlayerSystemManager.gravityScale;


        playerSystem.PlayerSystemManager.castRadius = playerSystem.transform.localScale.x * .5f - .05f;
        playerSystem.PlayerSystemManager.castDistance = (playerSystem.PlayerSystemManager.PCollider as CapsuleCollider2D).size.y * playerSystem.transform.localScale.y * .25f + .3f;

        playerSystem.PlayerSystemManager.groundCheck = Physics2D.CircleCast(playerSystem.transform.position, playerSystem.PlayerSystemManager.castRadius, Vector2.down, playerSystem.PlayerSystemManager.castDistance);
            // Si le joueur n'est pas knockback, il peut bouger.

        if (playerSystem.PlayerSystemManager.groundCheck)
            playerSystem.PlayerSystemManager.PlayerState = PLAYER_STATE.WALKING;
        else
            playerSystem.PlayerSystemManager.PlayerState = PLAYER_STATE.FALLING;
                

        Movement();

        if (playerSystem.PlayerSystemManager.holdJump && playerSystem.PlayerSystemManager.groundCheck)
            OnJump();

        // On limite la vitesse du joueur.
        playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(Mathf.Clamp(playerSystem.PlayerSystemManager.Rb2D.velocity.x, -playerSystem.PlayerSystemManager.maxSpeed, playerSystem.PlayerSystemManager.maxSpeed), playerSystem.PlayerSystemManager.Rb2D.velocity.y);
        
    }

    public void OnMove(Vector2 input)
    {
        if (Mathf.Abs(input.x) <= playerSystem.PlayerSystemManager.deadZone || playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.SHOOTING)
        {
            playerSystem.PlayerSystemManager.inputVectorMove = Vector2.zero;
            return;
        }

        playerSystem.PlayerSystemManager.inputVectorMove = input;

        // Redondance (x est pas censé valoir 0 mais on sait jamais)
        if (playerSystem.PlayerSystemManager.inputVectorMove.x > 0)
            playerSystem.PlayerSystemManager.LookDirection = Vector2.right;
        else if (playerSystem.PlayerSystemManager.inputVectorMove.x < 0)
            playerSystem.PlayerSystemManager.LookDirection = Vector2.left;

        if (playerSystem.PlayerSystemManager.brakingCoroutine != null)
        {
            playerSystem.PlayerSystemManager.StopCoroutine(playerSystem.PlayerSystemManager.brakingCoroutine);
            playerSystem.PlayerSystemManager.brakingCoroutine = null;
        }
    }

    public void OnJump()
    {
        if (playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.STUNNED
            || playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.KNOCKBACKED
            || playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.SHOOTING)
            return;

        int testlayer = 0;
        if (playerSystem.PlayerSystemManager.groundCheck)
            testlayer = playerSystem.PlayerSystemManager.groundCheck.collider.gameObject.layer;

        if (playerSystem.PlayerSystemManager.groundCheck && (testlayer == LayerMask.NameToLayer("Destructible")
            || testlayer == LayerMask.NameToLayer("Indestructible")
            || testlayer == LayerMask.NameToLayer("Trap")))
        {
            Jump();
        }
        else
        {
            #if UNITY_EDITOR
            {
                Debug.Log("You can't jump, you're not on solid ground.");
            }
            #endif
        }
    }

    IEnumerator Braking()
    {
        float iniVelocityX = playerSystem.PlayerSystemManager.Rb2D.velocity.x;
        float t = 0;
        while (t < 1f)
        {
            if (playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.KNOCKBACKED || playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.DASHING)
                break;

            playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), playerSystem.PlayerSystemManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / playerSystem.PlayerSystemManager.stopDuration;
            yield return new WaitForFixedUpdate();
        }

        if (playerSystem.PlayerSystemManager.PlayerState != PLAYER_STATE.KNOCKBACKED && playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.DASHING)
            playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(0, playerSystem.PlayerSystemManager.Rb2D.velocity.y);

        playerSystem.PlayerSystemManager.brakingCoroutine = null;
    }

    private void Movement()
    {
        if (playerSystem.PlayerSystemManager.inputVectorMove == Vector2.zero)
        {
            if (playerSystem.PlayerSystemManager.brakingCoroutine == null && playerSystem.PlayerSystemManager.Rb2D.velocity.x != 0)
            {
                playerSystem.PlayerSystemManager.brakingCoroutine = playerSystem.PlayerSystemManager.StartCoroutine(Braking());
            }
        }

        // Permet de se retourner rapidement sans perdre sa vitesse
        if ((playerSystem.PlayerSystemManager.inputVectorMove.x / Mathf.Abs(playerSystem.PlayerSystemManager.inputVectorMove.x)) + (playerSystem.PlayerSystemManager.Rb2D.velocity.x / Mathf.Abs(playerSystem.PlayerSystemManager.Rb2D.velocity.x)) == 0)
            playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(-playerSystem.PlayerSystemManager.Rb2D.velocity.x, playerSystem.PlayerSystemManager.Rb2D.velocity.y);

        playerSystem.PlayerSystemManager.Rb2D.velocity += Time.fixedDeltaTime * 100f * new Vector2(playerSystem.PlayerSystemManager.inputVectorMove.x, 0);
    }

    private void Jump()
    {
        playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(playerSystem.PlayerSystemManager.Rb2D.velocity.x, playerSystem.PlayerSystemManager.jumpForce * Time.fixedDeltaTime * 100f);
    }

}


