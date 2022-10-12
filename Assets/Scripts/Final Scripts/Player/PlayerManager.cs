using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [field: Header("Player State")]
    public PlayerState State { get; private set; }

    public Vector2 InputVector { get; private set; }

    public void SetInputVector(Vector2 direction)
    {
        InputVector = direction;
    }

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }
}
