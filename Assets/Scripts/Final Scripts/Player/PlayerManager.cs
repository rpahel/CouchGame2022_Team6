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

    [field: Header("Player State")]
    public PlayerState State { get; private set; }

    public Vector2 InputVector { get; private set; }

    public float eatAmount;

    private float _maxEatValue = 1;
    public float maxEatValue => _maxEatValue;
    
    public SwitchSizeSkin SwitchSkin  { get; private set; }
    
    public Image imageUI;
    public TextMeshProUGUI textUI;

    private bool _canEat;

    public bool CanEat => _canEat;

    private void Awake()
    {
        eatAmount = maxEatValue/2;
        _inputs = GetComponent<PlayerInputHandler>();
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
        _inputs.EnableInputs();
    }
    public void DisableInputs()
    {
        _inputs.DisableInputs();
    }
}
