using UnityEngine;

public abstract class State
{
    protected readonly PlayerStateSystem playerSystem;
    
    protected State(PlayerStateSystem playerSystem)
    {
        this.playerSystem = playerSystem;
    }

    public virtual void Start()
    {
        return;
    }

    public virtual void Update()
    {
        return;
    }

    public virtual void FixedUpdate()
    {
        return;
    }
    public virtual void OnMove()
    {
        return;
    }

    public virtual void OnJump()
    {
        return;
    }

    public virtual void OnHoldShoot()
    {
        return;
    }

    public virtual void OnShoot()
    {
        return;
    }

    public virtual void OnEat()
    {
        return;
    }

    public virtual void OnCollisionEnter(Collision2D col)
    {
        return;
    }

    public virtual void OnCollisionStay(Collision2D col)
    {
        return;
    }
    
    public virtual void OnTriggerEnter(Collider2D col)
    {
        return;
    }

    public virtual void OnKnockback(Vector2 knockBackForce)
    {
        return;
    }

    public virtual void OnStun<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        return;
    }

    public virtual void OnSpecial()
    {
        return;
    }

    public virtual void OnHoldSpecial()
    {
        return;
    }
}

