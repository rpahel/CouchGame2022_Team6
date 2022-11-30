using UnityEngine;

public class Moving : State 
{
    public Moving(PlayerStateSystem playerSystem) : base(playerSystem) { }

    public override void Update()
    {
        #if UNITY_EDITOR
        {
            if(playerSystem.PlayerSystemManager.StopDuration < 0.01f)  playerSystem.PlayerSystemManager.SetStopDuration(0.01f);
        }
        #endif
    }

    public override void FixedUpdate()
    {
        Movement();

        if (playerSystem.PlayerSystemManager.holdJump && playerSystem.PlayerSystemManager.GroundCheck())
            OnJump();

        // On limite la vitesse du joueur.
        playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(Mathf.Clamp(playerSystem.PlayerSystemManager.Rb2D.velocity.x, -playerSystem.PlayerSystemManager.MaxSpeed, playerSystem.PlayerSystemManager.MaxSpeed), playerSystem.PlayerSystemManager.Rb2D.velocity.y);
    }

    public override void OnMove()
    {
        if (Mathf.Abs(playerSystem.PlayerSystemManager.inputVectorDirection.x) <= playerSystem.PlayerSystemManager.DeadZone)
        {
            playerSystem.PlayerSystemManager.inputVectorMove = Vector2.zero;
            return;
        }

        playerSystem.PlayerSystemManager.inputVectorMove = playerSystem.PlayerSystemManager.inputVectorDirection;

        playerSystem.PlayerSystemManager.LookDirection = playerSystem.PlayerSystemManager.inputVectorMove.x switch
        {
            // Redondance (x est pas cense valoir 0 mais on sait jamais)
            > 0 => Vector2.right,
            < 0 => Vector2.left,
            _ => playerSystem.PlayerSystemManager.LookDirection
        };

        if (playerSystem.PlayerSystemManager.brakingCoroutine != null)
            playerSystem.PlayerSystemManager.brakingCoroutine = null;
    }

    public override void OnJump()
    {
        playerSystem.PlayerSystemManager.Jump();
    }

    private void Movement()
    {
        if (playerSystem.PlayerSystemManager.inputVectorMove == Vector2.zero)
        {
            if (playerSystem.PlayerSystemManager.brakingCoroutine == null && playerSystem.PlayerSystemManager.Rb2D.velocity.x != 0)
            {
                playerSystem.PlayerSystemManager.brakingCoroutine = playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking());
            }
        }

        // Permet de se retourner rapidement sans perdre sa vitesse
        if ((playerSystem.PlayerSystemManager.inputVectorMove.x / Mathf.Abs(playerSystem.PlayerSystemManager.inputVectorMove.x)) + (playerSystem.PlayerSystemManager.Rb2D.velocity.x / Mathf.Abs(playerSystem.PlayerSystemManager.Rb2D.velocity.x)) == 0)
            playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(-playerSystem.PlayerSystemManager.Rb2D.velocity.x, playerSystem.PlayerSystemManager.Rb2D.velocity.y);

        playerSystem.PlayerSystemManager.Rb2D.velocity += Time.fixedDeltaTime * 100f * new Vector2(playerSystem.PlayerSystemManager.inputVectorMove.x, 0);
    }

    public override void OnHoldShoot()
    {
        playerSystem.SetState(new Aim(playerSystem));
    }

    public override void OnCollision(Collision2D col)
    {
    }

}