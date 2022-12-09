//using UnityEngine;
//
//public class Stun : State
//{
//    public Stun(PlayerStateSystem playerSystem) : base(playerSystem)
//    {
//    }
//
//    /// <summary>
//    /// R?duit la jauge de bouffe et knockback le joueur.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="damageDealer">L'objet responsable des d?gats (un joueur, un pi?ge, etc).</param>
//    /// <param name="damage">Lra quantit? de bouffe ? retirer.</param>
//    public override void OnStun<T>(T damageDealer, int damage, Vector2 knockBackForce)
//    {
//        playerSystem.PlayerManager.fullness = Mathf.Clamp(playerSystem.PlayerManager.fullness - damage, 0, 100);
//        playerSystem.PlayerManager.UpdatePlayerScale();
//
//        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.ActivateInputDelay(playerSystem.PlayerManager.CooldownShoot));
//    }
//
//    public override void FixedUpdate()
//    {
//        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking());
//    }
//
//}