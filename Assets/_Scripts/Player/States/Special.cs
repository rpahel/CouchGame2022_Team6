using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special : State
{
    public Special(PlayerSystem playerSystem) : base(playerSystem)
    {
    }

    public override void Start()
    {
        playerSystem.PlayerSystemManager.inputDirectionDash = playerSystem.PlayerSystemManager.inputVectorDirection;
        playerSystem.CooldownManager.SetDashCoroutine();
    }

    public override void FixedUpdate()
    {
        playerSystem.PlayerSystemManager.Rb2D.AddForce(playerSystem.PlayerSystemManager.inputDirectionDash * playerSystem.PlayerSystemManager.DashForce, ForceMode2D.Impulse);
    }

}
