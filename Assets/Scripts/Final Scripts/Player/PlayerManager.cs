using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputHandler _inputs;
    private Rigidbody2D _rb;

    [field: Header("Player State")]
    public PlayerState State { get; private set; }

    public Vector2 InputVector { get; private set; }

    public float eatAmount;

    private const float maxEatValue = 1;
    public float MaxEatValue => maxEatValue;

    private const float maxScale = 2.857143f;

    public float MaxScale => maxScale;
    
    public SwitchSizeSkin SwitchSkin  { get; private set; }
    
    public Image imageUI;
    public TextMeshProUGUI textUI;

    private bool _canEat;

    public bool CanEat => _canEat;

    private void Awake()
    {
        eatAmount = MaxEatValue/2;
        _inputs = GetComponent<PlayerInputHandler>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        State = PlayerState.Moving;
    }

    public void SetInputVector(Vector2 direction)
    {
        InputVector = direction;
    }

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }

    public void SetSkin(SwitchSizeSkin skin)
    {
        SwitchSkin = skin;
    }

    public void SetCanEat(bool result)
    {
        _canEat = result;
    }

    public void EnableInputs()
    {
        Debug.Log("Enable");
        _inputs.EnableInputs();
    }
    public void DisableInputs()
    {
        Debug.Log("Disable");
        _inputs.DisableInputs();
    }

    public void ResetPlayer()
    {
        eatAmount = MaxEatValue/2;
    }

    [ContextMenu("SetDead")]
    public void SetDead()
    {
        State = PlayerState.Dead;
        //GameManager.Instance.gameObject.GetComponent<RespawnPlayer>().Respawn(gameObject);
    }

    [ContextMenu("SetDead2")]
    public void SetDead2()
    {
        State = PlayerState.Dead;
        GameManager.Instance.RespawnPlayer(gameObject);
    }

    public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        eatAmount -= damage; //Div damage par 100

        _rb.AddForce(knockBackForce, ForceMode2D.Impulse);
        State = PlayerState.KNOCKBACKED;
    }

    public void OnDamage<T>(T damageDealer, int damage)
    {
        eatAmount -= damage; //Div damage par 100
    }
}
