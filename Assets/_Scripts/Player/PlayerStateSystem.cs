using UnityEngine;

public class PlayerStateSystem : StateMachine
{
    public PlayerManager PlayerSystemManager { get; private set; }
    public CooldownManager CooldownManager { get; private set; }

    public PlayerInputManager PlayerInputsState { get; private set; }
    public State PlayerState => State;

    private void Awake()
    {
        PlayerSystemManager = GetComponent<PlayerManager>();
        CooldownManager = GetComponent<CooldownManager>();
        PlayerInputsState = GetComponent<PlayerInputManager>();
    }

    public void Start()
    {
        SetState((new Moving(this)));
    }

    public void SetKnockback(Vector2 knockBackForce)
    {
        SetState((new Knockback(this)));
        State?.OnKnockback(knockBackForce);
    }

    public void SetStun<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        if (State is Dashing) return;

        SetState((new Stun(this)));
        State?.OnStun<T>(damageDealer, damage, knockBackForce);
    }

    public void SetMoving()
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

    public void OnHoldShoot()
    {
        State?.OnHoldShoot();
    }

    public void OnShoot()
    {
        State?.OnShoot();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        State?.OnCollision(collision);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        State?.OnTrigger(col);
    }
}