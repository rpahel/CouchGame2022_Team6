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
    }

    public override void FixedUpdate()
    {
        Movement();

        if (playerSystem.PlayerManager.holdJump)
            OnJump();

        // On limite la vitesse du joueur.
        playerSystem.PlayerManager.Rb2D.velocity = new Vector2(Mathf.Clamp(playerSystem.PlayerManager.Rb2D.velocity.x, -playerSystem.PlayerManager.MaxSpeed, playerSystem.PlayerManager.MaxSpeed), playerSystem.PlayerManager.Rb2D.velocity.y);
    }

    public override void OnMove()
    {
        if (Mathf.Abs(playerSystem.PlayerManager.inputVectorDirection.x) <= playerSystem.PlayerManager.DeadZone)
        {
            playerSystem.PlayerManager.inputVectorMove = Vector2.zero;
            return;
        }

        playerSystem.PlayerManager.inputVectorMove = playerSystem.PlayerManager.inputVectorDirection;

        playerSystem.PlayerManager.LookDirection = playerSystem.PlayerManager.inputVectorMove.x switch
        {
            // Redondance (x est pas cense valoir 0 mais on sait jamais)
            > 0 => Vector2.right,
            < 0 => Vector2.left,
            _ => playerSystem.PlayerManager.LookDirection
        };

        if (playerSystem.PlayerManager.brakingCoroutine != null)
            playerSystem.PlayerManager.brakingCoroutine = null;
    }

    public override void OnJump()
    {
        if(playerSystem.PlayerManager.GroundCheck())
            playerSystem.PlayerManager.Jump();
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