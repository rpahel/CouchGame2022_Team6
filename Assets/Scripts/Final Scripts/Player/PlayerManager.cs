using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [field: Header("Player State")]
    public PlayerState State { get; private set; }

    public Vector2 InputVector { get; private set; }

    public float eatAmount;

    public struct PlayerUI
    {
        public SpriteRenderer Sprite;
        public TextMeshPro Text;
    }

    private void Awake()
    {
        eatAmount = 0.5f;
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
