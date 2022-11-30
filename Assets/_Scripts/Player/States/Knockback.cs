using UnityEngine;

public class Knockback : State
{
    public Knockback(PlayerStateSystem playerSystem) : base(playerSystem) { }

    /// <summary>
    /// Knockback le joueur
    /// </summary>
    public override void OnKnockback(Vector2 knockBackForce)
    {
        playerSystem.PlayerManager.Rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
    }

    public override void FixedUpdate()
    {
        if (playerSystem.PlayerManager.GroundCheck() && playerSystem.PlayerManager.Rb2D.velocity.y == 0)
            playerSystem.SetState((new Moving(playerSystem)));
    }
}