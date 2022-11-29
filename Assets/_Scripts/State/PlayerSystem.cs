using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSystem : StateMachine
{
    public PlayerSystemManager PlayerSystemManager { get; private set; }

    public State PlayerState => State;

    private void Awake()
    {
        PlayerSystemManager = GetComponent<PlayerSystemManager>();
    }
    public void Start()
    {
        SetState((new Moving(this))); 
    }

    public void Update()
    {
        State?.Update();
    }

    private void FixedUpdate()
    {
        State?.FixedUpdate();
    }

    public void OnMove()
    {
        State?.OnMove();
    }

    public void OnJump()
    {
        State?.OnJump();
    }

    public void OnEat()
    {
        State?.OnEat();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        State?.OnCollision(collision);
    }
}
