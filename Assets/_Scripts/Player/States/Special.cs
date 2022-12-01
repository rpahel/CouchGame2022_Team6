using UnityEngine;

public class Special : State
{
    private PlayerManager _playerManager;

    public Special(PlayerStateSystem playerSystem) : base(playerSystem)
    {
    }

    public override void Start()
    {
        _playerManager = playerSystem.PlayerManager;
        _playerManager.inputDirectionDash = _playerManager.inputVectorDirection;
        playerSystem.CooldownManager.SetDashCoroutine();
    }

    public override void FixedUpdate()
    {
        _playerManager.Rb2D.AddForce(_playerManager.inputDirectionDash * _playerManager.DashForce, ForceMode2D.Impulse);
    }
}