using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region Variables
    //============================
    public static GameManager Instance { get; private set; }

    //============================
    [Header("DEBUGGING")]
    public bool DEBUGGING;
    [Range(1, 4)] public int DEBUG_playersToSpawn;

    //============================
    [Header("Données")]
    public int minimumNbOfPlayers = 1;
    private int nbOfPlayers;
    public GameObject playerPrefab;
    private PlayerInputManager playerInputManager;
    private bool allPlayerHaveSpawned;

    //============================
    private GAME_STATE  gameState;
    public  GAME_STATE GameState { get; }

    //============================
    private Transform[] playerTransforms = new Transform[4];
    public  Transform[] PlayerTransforms { get; }

    //============================
    private Transform[] spawnPositions = new Transform[4];
    public  Transform[] SpawnPositions { get { return spawnPositions; } set { spawnPositions = value; } }
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(Instance);
        }

        gameState = GAME_STATE.NONE;

        //if (!TryGetComponent<PlayerInputManager>(out playerInputManager))
        //{
        //    #if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //    #endif
        //    throw new Exception("No PlayerInputManager component found in GameManager game object !");
        //}

        //playerPrefab = playerInputManager.playerPrefab;
    }

    private void Update()
    {
        if (DEBUGGING && !allPlayerHaveSpawned && gameState == GAME_STATE.PLAYING)
        {
            for(int i = 0; i < DEBUG_playersToSpawn; i++)
            {
                OnPlayerJoin();
            }

            allPlayerHaveSpawned = true;
        }
    }
    #endregion

    #region Custom_Functions
    public void ChangeGameState(GAME_STATE newState)
    {
        gameState = newState;
    }

    public void OnPlayerJoin()
    {
        if (spawnPositions[nbOfPlayers] == null)
            throw new System.Exception("Not enough Spawn positions in level for all players.");

        playerTransforms[nbOfPlayers] = Instantiate(playerPrefab, spawnPositions[nbOfPlayers].position, Quaternion.identity).transform;
        nbOfPlayers++;
    }
    #endregion
}
