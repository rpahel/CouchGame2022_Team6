using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public  GAME_STATE GameState { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of GameManager");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    public void SetGameState(GAME_STATE state)
    {
        GameState = state;
    }
}
