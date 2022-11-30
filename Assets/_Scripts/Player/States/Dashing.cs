using UnityEngine;

public class Dashing : State
{
    public Dashing(PlayerStateSystem playerSystem) : base(playerSystem)
    {
    }

    public override void Start()
    {
    }

    public override void FixedUpdate()
    {
    }



    public override void OnCollision(Collision2D col)
    {
        Debug.Log("collision");
    }
}