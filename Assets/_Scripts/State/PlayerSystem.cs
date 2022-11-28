using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSystem : StateMachine
{
    public PlayerSystemManager PlayerSystemManager { get; private set; }

    private void Awake()
    {
        PlayerSystemManager = GetComponent<PlayerSystemManager>();
    }
    public void Start()
    {
        SetState((new Moving(this))); //Start set in the player collision
    }

}
