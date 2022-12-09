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
        _playerManager.fullness = Mathf.Clamp(_playerManager.fullness - _playerManager.NecessaryFoodSpecial, 0, 100);
        _playerManager.UpdatePlayerScale();
        playerSystem.CooldownManager.SetDashCoroutine();
    }

    public override void FixedUpdate()
    {
        if(_dashing)
            _playerManager.Rb2D.AddForce(_playerManager.inputDirectionDash * _playerManager.DashForce, ForceMode2D.Impulse);
    }

    public override void OnCollisionEnter(Collision2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Limite") || col.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            _dashing = false;
            playerSystem.PlaySound("Player_Special_Bonk");

            if (playerSystem.PlayerManager.fullness > 0)
                playerSystem.SetState(new Moving(playerSystem));
            else
                playerSystem.PlayerManager.OnDamage(col.gameObject.GetComponentInParent<Cube>(), 100, Vector2.zero);
        }
    }
}