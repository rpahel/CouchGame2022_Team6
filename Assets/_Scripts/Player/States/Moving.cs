using UnityEngine;

public class Moving : State
{
    public Moving(PlayerStateSystem playerSystem) : base(playerSystem) { }

    public override void Update()
    {
#if UNITY_EDITOR
        {
            if (playerSystem.PlayerManager.StopDuration < 0.01f) playerSystem.PlayerManager.SetStopDuration(0.01f);
        }
#endif

        if (playerSystem.PlayerManager.Rb2D.velocity.y > 0)
            playerSystem.PlayerManager.cantDoubleJump = true;

        else
            playerSystem.PlayerManager.cantDoubleJump = false;
    }

    public override void FixedUpdate()
    {
        Movement();

        if (playerSystem.PlayerManager.holdJump)
            OnJump();

        // On limite la vitesse du joueur.
        playerSystem.PlayerManager.Rb2D.velocity = new Vector2(Mathf.Clamp(playerSystem.PlayerManager.Rb2D.velocity.x, -playerSystem.PlayerManager.MaxSpeed, playerSystem.PlayerManager.MaxSpeed), playerSystem.PlayerManager.Rb2D.velocity.y);

        playerSystem.PlayerManager.UpdateDetectWall();
        playerSystem.PlayerManager.UpdateWallJump();
        playerSystem.PlayerManager.UpdateWallSliding();
    }

    public override void OnMove()
    {
        if (Mathf.Abs(playerSystem.PlayerManager.inputVectorDirection.x) <= playerSystem.PlayerManager.DeadZone)
        {
            playerSystem.PlayerManager.inputVectorMove = Vector2.zero;
            return;
        }

        playerSystem.PlayerManager.inputVectorMove = playerSystem.PlayerManager.inputVectorDirection;

        if (playerSystem.PlayerManager.brakingCoroutine != null)
            playerSystem.PlayerManager.brakingCoroutine = null;
    }

    public override void OnJump()
    {
        if(playerSystem.PlayerManager.GroundCheck())
            playerSystem.PlayerManager.Jump();
        else if(playerSystem.PlayerManager.isSliding && playerSystem.PlayerManager.GroundCheck() == false && playerSystem.PlayerManager.cantDoubleJump == false)
        {
            playerSystem.PlayerManager.inputVectorDirection.x = 0f;
            playerSystem.PlayerManager.WallJump();
            playerSystem.PlayerManager.inputVectorDirection.x = 1f;
           
        }
           
        
    }

    private void Movement()
    {
        if (playerSystem.PlayerManager.inputVectorMove == Vector2.zero)
        {
            if (playerSystem.PlayerManager.brakingCoroutine == null && playerSystem.PlayerManager.Rb2D.velocity.x != 0)
            {
                playerSystem.PlayerManager.brakingCoroutine = playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking());
            }
        }

        // Permet de se retourner rapidement sans perdre sa vitesse
        if ((playerSystem.PlayerManager.inputVectorMove.x / Mathf.Abs(playerSystem.PlayerManager.inputVectorMove.x)) + (playerSystem.PlayerManager.Rb2D.velocity.x / Mathf.Abs(playerSystem.PlayerManager.Rb2D.velocity.x)) == 0)
            playerSystem.PlayerManager.Rb2D.velocity = new Vector2(-playerSystem.PlayerManager.Rb2D.velocity.x, playerSystem.PlayerManager.Rb2D.velocity.y);

        playerSystem.PlayerManager.Rb2D.velocity += Time.fixedDeltaTime * 100f * new Vector2(playerSystem.PlayerManager.inputVectorMove.x, 0);
    }

    public override void OnHoldShoot()
    {
        playerSystem.SetState(new AimShoot(playerSystem));
    }

    public override void OnHoldSpecial()
    {
        playerSystem.SetState(new AimSpecial(playerSystem));
    }

}