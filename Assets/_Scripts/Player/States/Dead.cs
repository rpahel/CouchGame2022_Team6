using UnityEngine;

public class Dead : State
{
    public Dead(PlayerStateSystem playerSystem) : base(playerSystem){}

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
        Debug.Log("Je meurs");
        waitTime = 0;
        GameManager.Instance.CameraManager.UpdatePlayers(playerSystem.transform); // Retire le transform du joueur dans le TargetGroup
        playerSystem.transform.position = new Vector3(-100, -100, 0);
    }

    // Mettre cette fonction direct dans PlayerSystemManager
    private void Respawn()
    {
        Debug.Log("Je revis");
        GameManager.Instance.RespawnPlayer(playerSystem.PlayerSystemManager.playerInput);
        playerSystem.PlayerSystemManager.fullness = 50;
        playerSystem.SetState(new Moving(playerSystem));
    }
}