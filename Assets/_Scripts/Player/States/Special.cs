using UnityEngine;

public class Special : State
{
    private PlayerManager _playerManager;
    private bool _dashing;
    public Special(PlayerStateSystem playerSystem) : base(playerSystem)
    {
    }

    public override void Start()
    {
        _playerManager = playerSystem.PlayerManager;
        _playerManager.inputDirectionDash = _playerManager.inputVectorDirection;
        _dashing = true;
        playerSystem.CooldownManager.SetDashCoroutine();
    }

    public override void FixedUpdate()
    {
        if(_dashing)
            _playerManager.Rb2D.AddForce(_playerManager.inputDirectionDash * _playerManager.DashForce, ForceMode2D.Impulse);
    }

    public override void OnCollisionEnter(Collision2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Limite"))
        {
            _dashing = false;
            playerSystem.SetState(new Moving(playerSystem));
        }
    }
}