using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : State
{
    public Dashing(PlayerSystem playerSystem) : base(playerSystem)
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
