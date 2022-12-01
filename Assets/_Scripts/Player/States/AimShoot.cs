using UnityEngine;
public class AimShoot : State 
{
    public AimShoot(PlayerStateSystem playerSystem) : base(playerSystem)
    {
    }
    
    public override void Start()
    {
        playerSystem.PlayerManager.AimPivot.gameObject.SetActive(true);
    }

    public override void FixedUpdate()
    {
        playerSystem.PlayerManager.AimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, playerSystem.PlayerManager.inputVectorDirection != Vector2.zero ? playerSystem.PlayerManager.inputVectorDirection : playerSystem.PlayerManager.LookDirection) - 90f);
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking()); // TODO : Attention au probleme des coroutines
    }

    public override void OnShoot()
    {
        playerSystem.PlayerManager.AimPivot.gameObject.SetActive(false);
        playerSystem.PlayerManager.Shoot();
        playerSystem.SetState(new Moving(playerSystem));
    }
}