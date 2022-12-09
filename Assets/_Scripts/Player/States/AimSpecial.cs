using UnityEngine;

public class AimSpecial : State
{
    private Transform _transformPos;
    private PlayerManager _playerManager;
    private int soundTracker = 0; // Very important do not delete

    public AimSpecial(PlayerStateSystem playerSystem) : base(playerSystem)
    {
    }
    
    public override void Start()
    {
        _playerManager = playerSystem.PlayerManager;
        _playerManager.AimPivot.gameObject.SetActive(true);
        _playerManager.charge = 0;
        playerSystem.PlaySound("Player_Special_Charge1");
        _transformPos = playerSystem.transform;
        playerSystem.PlayEffect(1);
        soundTracker = 0;
    }

    public override void Update()
    {
        _playerManager.charge = Mathf.Clamp01(_playerManager.charge + Time.deltaTime / _playerManager.TimeToMaxCharge);

        if(_playerManager.charge > _playerManager.TimeToMaxCharge * .33f && soundTracker == 0)
        {
            playerSystem.PlaySound("Player_Special_Charge2");
            soundTracker = 1;
        }
        else if(_playerManager.charge > _playerManager.TimeToMaxCharge * .66f && soundTracker == 1)
        {
            playerSystem.PlaySound("Player_Special_Charge3");
            soundTracker = 2;
        }

#if UNITY_EDITOR
        {
            Debug.DrawRay(
                start : (Vector2)_transformPos.position + _playerManager.PCollider.bounds.extents.y * Vector2.down,
                dir : (_playerManager.MinDistance + _playerManager.charge * _playerManager.MaxDistance - _playerManager.MinDistance) * (_playerManager.inputVectorDirection != Vector2.zero ? _playerManager.inputVectorDirection : _playerManager.LookDirection),
                color : Color.magenta);

            Debug.DrawRay(
                start : (Vector2)_transformPos.position + _playerManager.PCollider.bounds.extents.x * Vector2.right,
                dir : (_playerManager.MinDistance + _playerManager.charge * _playerManager.MaxDistance - _playerManager.MinDistance) * (_playerManager.inputVectorDirection != Vector2.zero ? _playerManager.inputVectorDirection : _playerManager.LookDirection),
                color : Color.magenta);

            Debug.DrawRay(
                start : (Vector2)_transformPos.position + _playerManager.PCollider.bounds.extents.x * Vector2.left,
                dir : (_playerManager.MinDistance + _playerManager.charge * _playerManager.MaxDistance - _playerManager.MinDistance) * (_playerManager.inputVectorDirection != Vector2.zero ? _playerManager.inputVectorDirection : _playerManager.LookDirection),
                color : Color.magenta);

            Debug.DrawRay(
                start : (Vector2)_transformPos.position + _playerManager.PCollider.bounds.extents.y * Vector2.up,
                dir : (_playerManager.MinDistance + _playerManager.charge * _playerManager.MaxDistance - _playerManager.MinDistance) * (_playerManager.inputVectorDirection != Vector2.zero ? _playerManager.inputVectorDirection : _playerManager.LookDirection),
                color : Color.magenta);
        }
#endif
    }

    public override void FixedUpdate()
    {
        _playerManager.AimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, _playerManager.inputVectorDirection != Vector2.zero ? _playerManager.inputVectorDirection : _playerManager.LookDirection) - 90f);
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking()); // TODO : Attention au probleme des coroutines
    }

    public override void OnSpecial()
    {
        _playerManager.AimPivot.gameObject.SetActive(false);
        playerSystem.StopEffect(1);
        playerSystem.SetState(new Special(playerSystem));

        if(_playerManager.charge < _playerManager.TimeToMaxCharge * .33f)
        {
            playerSystem.PlaySound("Player_Special_Short");
        }
        else if (_playerManager.charge >= _playerManager.TimeToMaxCharge * .33f && _playerManager.charge < _playerManager.TimeToMaxCharge * .66f)
        {
            playerSystem.PlaySound("Player_Special_Mid");
        }
        else if (_playerManager.charge >= _playerManager.TimeToMaxCharge * .66f)
        {
            playerSystem.PlaySound("Player_Special_Long");
        }
    }
}


