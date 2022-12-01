using UnityEngine;

public class AimSpecial : State
{
    private Transform _transformPos;
    private PlayerManager _playerManager;

    public AimSpecial(PlayerStateSystem playerSystem) : base(playerSystem)
    {
    }
    
    public override void Start()
    {
        _playerManager = playerSystem.PlayerManager;
        _playerManager.AimPivot.gameObject.SetActive(true);
        _playerManager.charge = 0;
            
        _transformPos = playerSystem.transform;
    }

    public override void Update()
    {
        _playerManager.charge = Mathf.Clamp01(_playerManager.charge + Time.deltaTime / _playerManager.TimeToMaxCharge);
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_playerManager.MaxDistance < _playerManager.MinDistance) _playerManager.maxDistance = _playerManager.MinDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_transformPos.position, _playerManager.MinDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_transformPos.position, _playerManager.MaxDistance);
    }
#endif
    

    public override void FixedUpdate()
    {
        _playerManager.AimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, _playerManager.inputVectorDirection != Vector2.zero ? _playerManager.inputVectorDirection : _playerManager.LookDirection) - 90f);
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking()); // TODO : Attention au probleme des coroutines
    }

    public override void OnSpecial()
    {
        _playerManager.AimPivot.gameObject.SetActive(false);
        playerSystem.SetState(new Special(playerSystem));
    }
}


