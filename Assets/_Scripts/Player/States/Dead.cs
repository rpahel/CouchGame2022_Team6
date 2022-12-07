using UnityEngine;

public class Dead : State
{
    public Dead(PlayerStateSystem playerSystem) : base(playerSystem) { }

    private float waitTime;
    private float timeToWait;

    public override void Start()
    {
        timeToWait = GameManager.Instance.RespawnTime;
        Die();
    }

    public override void FixedUpdate()
    {
        if (waitTime < timeToWait)
            waitTime += Time.fixedDeltaTime;
        else
            Respawn();
    }

    private void Die()
    {
        waitTime = 0;
        GameManager.Instance.CameraManager.UpdatePlayers(playerSystem.transform); // Retire le transform du joueur dans le TargetGroup
        playerSystem.transform.position = new Vector3(-100, -100, 0);
    }

    // Mettre cette fonction direct dans PlayerSystemManager
    private void Respawn()
    {
        GameManager.Instance.CameraManager.UpdatePlayers(playerSystem.transform);
        GameManager.Instance.RespawnPlayer(playerSystem.gameObject);
        playerSystem.PlayerManager.fullness = 50;
        playerSystem.PlayerManager.inputVectorDirection = Vector2.zero;
        playerSystem.PlayerManager.inputVectorMove = Vector2.zero;
        playerSystem.PlayerManager.UpdatePlayerScale();

        Vector2 respawnSpot = GameManager.Instance.LevelGenerator.FindSuitableRespawnSpot();
        if (respawnSpot != -Vector2.one)
            playerSystem.transform.position = respawnSpot;

        playerSystem.SetState(new Moving(playerSystem));
        playerSystem.gameObject.layer = LayerMask.NameToLayer("Player");
    }
}