using UnityEngine;

public class Knockback : State
{
    public Knockback(PlayerStateSystem playerSystem) : base(playerSystem) { }

    public override void Start()
    {
        playerSystem.PlayerManager.inputVectorDirection = Vector2.zero;
        playerSystem.PlayerManager.inputVectorMove = Vector2.zero;
    }

    /// <summary>
    /// Knockback le joueur
    /// </summary>
    public override void OnKnockback(Vector2 knockBackForce)
    {
        playerSystem.PlayerManager.Rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
    }

    public override void FixedUpdate()
    {
        if (playerSystem.PlayerManager.GroundCheck() && playerSystem.PlayerManager.Rb2D.velocity.y == 0 && playerSystem.PlayerManager.fullness > 0)
            playerSystem.SetState((new Moving(playerSystem)));
        else if(playerSystem.PlayerManager.fullness <= 0)
            playerSystem.SetState(new Dead(playerSystem));
    }
}