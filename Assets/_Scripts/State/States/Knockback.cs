using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : State
{
    public Knockback(PlayerSystem playerSystem) : base(playerSystem)
    {
    }

    /// <summary>
    /// R?duit la jauge de bouffe et knockback le joueur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="damageDealer">L'objet responsable des d?gats (un joueur, un pi?ge, etc).</param>
    /// <param name="damage">Lra quantit? de bouffe ? retirer.</param>
    public override void OnKnockback(Vector2 knockBackForce)
    {
        Debug.Log("knock");
        //playerSystem.PlayerSystemManager.Rb2D.AddForce(knockBackForce, ForceMode2D.Impulse);
        playerSystem.PlayerSystemManager.Rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
    }

    public override void OnCollisionEnter(Collision2D col)
    {
        //playerSystem.PlayerSystemManager.Rb2D.velocity = Vector2.zero;
        playerSystem.SetState(new Moving(playerSystem));
    }
}
