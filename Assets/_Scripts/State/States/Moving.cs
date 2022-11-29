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

    public void Awake()
    {
        
    }
    public override IEnumerator Start()
    {
        //PLAYERMOVEMENT AWAKE
        playerSystem.PlayerSystemManager.stopDuration = playerSystem.PlayerSystemManager.stopDuration < 0.01f ? 0.01f : playerSystem.PlayerSystemManager.stopDuration;
        playerSystem.PlayerSystemManager.LookDirection = Vector2.right;
        playerSystem.PlayerSystemManager.castRadius = playerSystem.transform.localScale.x * .5f - .05f;
        
        //EAT AWAKE
        playerSystem.PlayerSystemManager.cooldown = playerSystem.PlayerSystemManager.eatCooldown;
        playerSystem.PlayerSystemManager.tickHoldEat = 1f;
        
        
        playerSystem.PlayerSystemManager.Rb2D.gravityScale = playerSystem.PlayerSystemManager.gravityScale != 0 ? playerSystem.PlayerSystemManager.gravityScale : playerSystem.PlayerSystemManager.Rb2D.gravityScale;

        if (playerSystem.PlayerSystemManager.PCollider is CapsuleCollider2D collider2D)
            playerSystem.PlayerSystemManager.castDistance = collider2D.size.y * playerSystem.transform.localScale.y * .25f + .3f;
        else
            throw new Exception("PManager.PCollider is not a CapsuleCollider2D. Please update the code.");
        
        yield break;
    }

    public override void Update()
    {
        #if UNITY_EDITOR
        {
            if(playerSystem.PlayerSystemManager.stopDuration < 0.01f)
                playerSystem.PlayerSystemManager.stopDuration = 0.01f;
        }
        #endif
        
    }

    public override void FixedUpdate()
    {
        playerSystem.PlayerSystemManager.Rb2D.gravityScale = playerSystem.PlayerSystemManager.gravityScale;
        
        playerSystem.PlayerSystemManager.castRadius = playerSystem.transform.localScale.x * .5f - .05f;
        playerSystem.PlayerSystemManager.castDistance = (playerSystem.PlayerSystemManager.PCollider as CapsuleCollider2D).size.y * playerSystem.transform.localScale.y * .25f + .3f;

        playerSystem.PlayerSystemManager.groundCheck = Physics2D.CircleCast(playerSystem.transform.position, playerSystem.PlayerSystemManager.castRadius, Vector2.down, playerSystem.PlayerSystemManager.castDistance);
            // Si le joueur n'est pas knockback, il peut bouger.

        playerSystem.PlayerSystemManager.PlayerState = playerSystem.PlayerSystemManager.groundCheck ? PLAYER_STATE.WALKING : PLAYER_STATE.FALLING;

        Movement();

        if (playerSystem.PlayerSystemManager.holdJump && playerSystem.PlayerSystemManager.groundCheck)
            OnJump();

        // On limite la vitesse du joueur.
        playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(Mathf.Clamp(playerSystem.PlayerSystemManager.Rb2D.velocity.x, -playerSystem.PlayerSystemManager.maxSpeed, playerSystem.PlayerSystemManager.maxSpeed), playerSystem.PlayerSystemManager.Rb2D.velocity.y);
        
        //EAT FIXED UPDATE
        playerSystem.PlayerSystemManager.cooldown = Mathf.Clamp(playerSystem.PlayerSystemManager.cooldown + Time.deltaTime, 0, playerSystem.PlayerSystemManager.eatCooldown);

        if (!playerSystem.PlayerSystemManager.holdEat) return;
        if (playerSystem.PlayerSystemManager.fullness > 100) return;
        
        if (playerSystem.PlayerSystemManager.tickHoldEat >= 1f)
        {
            OnEat();
            playerSystem.PlayerSystemManager.tickHoldEat = 0f;
        }
        else
        {
            playerSystem.PlayerSystemManager.tickHoldEat += Time.deltaTime * playerSystem.PlayerSystemManager.eatTickrate;
        }
    }

    public override void OnMove()
    {
        if (Mathf.Abs(playerSystem.PlayerSystemManager.inputVectorDirection.x) <= playerSystem.PlayerSystemManager.deadZone)
        {
            playerSystem.PlayerSystemManager.inputVectorMove = Vector2.zero;
            return;
        }

        playerSystem.PlayerSystemManager.inputVectorMove = playerSystem.PlayerSystemManager.inputVectorDirection;

        playerSystem.PlayerSystemManager.LookDirection = playerSystem.PlayerSystemManager.inputVectorMove.x switch
        {
            // Redondance (x est pas cens� valoir 0 mais on sait jamais)
            > 0 => Vector2.right,
            < 0 => Vector2.left,
            _ => playerSystem.PlayerSystemManager.LookDirection
        };

        if (playerSystem.PlayerSystemManager.brakingCoroutine == null) return;
        
        playerSystem.PlayerSystemManager.StopCoroutine(playerSystem.PlayerSystemManager.brakingCoroutine);
        playerSystem.PlayerSystemManager.brakingCoroutine = null;
    }

    public override void OnJump()
    {
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

            playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), playerSystem.PlayerSystemManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / playerSystem.PlayerSystemManager.stopDuration;
            yield return new WaitForFixedUpdate();
        }

        if (playerSystem.PlayerSystemManager.PlayerState != PLAYER_STATE.KNOCKBACKED && playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.DASHING) //A quoi sert ca
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
    
    /// <summary>
    /// Essaie de manger ce qui se trouve dans direction.
    /// </summary>
    // TODO : Supprimer les Debug
    public override void OnEat()
    {
        var direction = playerSystem.PlayerSystemManager.inputVectorDirection;
        
        Debug.Log(direction);
        
        if (playerSystem.PlayerSystemManager.cooldown < playerSystem.PlayerSystemManager.eatCooldown)
        {
            Debug.Log($"Attendez un peu avant de manger ({playerSystem.PlayerSystemManager.cooldown:0.00} / {playerSystem.PlayerSystemManager.eatCooldown:0.00})");
            return;
        }

        if(playerSystem.PlayerSystemManager.fullness >= 100)
        {
            Debug.Log("Tu es plein et ne peut donc plus manger! Vomis.");
            return;
        }

        if (direction == Vector2.zero)
        {
            direction = !(playerSystem.PlayerSystemManager.GroundCheck) ? Vector2.up : playerSystem.PlayerSystemManager.LookDirection;
        }

        #if UNITY_EDITOR
        Debug.DrawRay(playerSystem.transform.position - Vector3.forward, direction.normalized * playerSystem.PlayerSystemManager.eatDistance, Color.red, playerSystem.PlayerSystemManager.eatCooldown);
        #endif

        RaycastHit2D hit = Physics2D.Raycast(playerSystem.transform.position, direction.normalized, playerSystem.PlayerSystemManager.eatDistance, 1 << LayerMask.NameToLayer("Destructible"));
        if (hit)
        {
            hit.transform.parent.GetComponent<Cube_Edible>().GetManged(playerSystem.transform);
            playerSystem.PlayerSystemManager.fullness = Mathf.Clamp(playerSystem.PlayerSystemManager.fullness + playerSystem.PlayerSystemManager.filling, 0, 100);
            playerSystem.PlayerSystemManager.UpdatePlayerScale();
        }
        else if(!(playerSystem.PlayerSystemManager.GroundCheck)) // S'il touche rien et qu'il n'est pas au sol on ressaie de manger dans le sens du regard cette fois
        {
            direction = playerSystem.PlayerSystemManager.LookDirection;
            hit = Physics2D.Raycast(playerSystem.transform.position, direction.normalized, playerSystem.PlayerSystemManager.eatDistance, 1 << LayerMask.NameToLayer("Destructible"));

            if (hit)
            {
                hit.transform.parent.GetComponent<Cube_Edible>().GetManged(playerSystem.transform);
                playerSystem.PlayerSystemManager.fullness = Mathf.Clamp(playerSystem.PlayerSystemManager.fullness + playerSystem.PlayerSystemManager.filling, 0, 100);
                playerSystem.PlayerSystemManager.UpdatePlayerScale();
            }
        }

        playerSystem.PlayerSystemManager.cooldown = 0;
    }

    public override void OnCollision(Collision2D col)
    {
    }

}


