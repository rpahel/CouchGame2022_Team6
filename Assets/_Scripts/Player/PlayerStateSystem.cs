using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class PlayerStateSystem : StateMachine
{
    public PlayerManager PlayerManager { get; private set; }
    public CooldownManager CooldownManager { get; private set; }
    public FaceManager FaceManager { get; private set; }
    //public PLAYER_STATE currentState;
    public State PlayerState => State;
    private AudioManager audioManager;

    [FormerlySerializedAs("listVFXeffect")]
    [Header("VFX")]
    [SerializeField] private List<VisualEffect> listVfX_effect = new List<VisualEffect>();

    public List<VisualEffect> ListVfX_effect => listVfX_effect;

    private void Awake()
    {
        PlayerManager = GetComponent<PlayerManager>();
        CooldownManager = GetComponent<CooldownManager>();
        FaceManager = GetComponent<FaceManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Start()
    {
        SetState((new Moving(this)));
        InvokeRepeating("CustomUpdate", 0f, 0.05f);
    }

    private void LateUpdate()
    {
        //currentState = (PLAYER_STATE)System.Enum.Parse(typeof(PLAYER_STATE), this.State.GetType().ToString());
    }

    private void CustomUpdate()
    {
        State?.CustomUpdate();
    }

    public void SetKnockback(Vector2 knockBackForce)
    {
        SetState((new Knockback(this)));
        State?.OnKnockback(knockBackForce);
    }

    //public void SetStun<T>(T damageDealer, int damage, Vector2 knockBackForce)
    //{
    //    if (State is Special) return;
    //
    //    SetState((new Stun(this)));
    //    State?.OnStun<T>(damageDealer, damage, knockBackForce);
    //}

    public void PlaySound(string name)
    {
        audioManager.Play(name);
    }
    public void StopSound(string name)
    {
        audioManager.Stop(name);
    }

    public void PlayEffect(int index)
    {
        listVfX_effect[index].Play();
    }

    public void StopEffect(int index)
    {
        listVfX_effect[index].Stop();
    }

    public void StopAllEffects()
    {
        foreach (VisualEffect vfx in listVfX_effect)
        {
            vfx.Stop();
        }
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
    
    public void OnHoldSpecial()
    {
        State?.OnHoldSpecial();
    }

    public void OnSpecial()
    {
        State?.OnSpecial();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        State?.OnCollisionEnter(collision);
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        State?.OnCollisionStay(collision);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        State?.OnTriggerEnter(col);
    }
}

#if UNITY_EDITOR
public enum PLAYER_STATE
{
    AimShoot,
    AimSpecial,
    Dead,
    Knockback,
    Moving,
    Special,
    State,
    Stun
}
#endif