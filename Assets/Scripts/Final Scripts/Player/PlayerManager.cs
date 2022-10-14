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
    [field: Header("Player State")]
    public PlayerState State { get; private set; }

    public Vector2 InputVector { get; private set; }

    public float eatAmount;

    private float _maxEatValue = 2.857143f;
    public float maxEatValue => _maxEatValue;
    
    public Image imageUI;
    public TextMeshProUGUI textUI;
    

    private void Awake()
    {
        eatAmount = (maxEatValue+1)/2;
    }

    public void SetInputVector(Vector2 direction)
    {
        InputVector = direction;
    }

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }
}
