using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Data;

public class GameManager : MonoBehaviour
{
    #region Autres_Scripts
    //============================
    private PlayerInputManager playerInputManager;
    [SerializeField] private CameraManager cameraManager;
    #endregion

    #region Variables
    //============================
    public static GameManager Instance { get; private set; }

    //============================
    [Header("Données")]
    public int minimumNbOfPlayers = 1;
    public GameObject playerPrefab;

    //============================
    public  GAME_STATE GameState { get; private set; }

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

        GameState = GAME_STATE.NONE;

        if (!TryGetComponent(out playerInputManager))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerInputManager component found in GameManager game object !");
        }

        //playerPrefab = playerInputManager.playerPrefab;
    }

    private void Update()
    {
        //if (DEBUGGING && !allPlayerHaveSpawned && gameState == GAME_STATE.PLAYING)
        //{
        //    for(int i = 0; i < DEBUG_playersToSpawn; i++)
        //    {
        //        OnPlayerJoined();
        //    }
        //
        //    allPlayerHaveSpawned = true;
        //}
    }
    #endregion

    #region Custom_Functions
    public void ChangeGameState(GAME_STATE newState)
    {
        GameState = newState;
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        if (pi.playerIndex >= 3)
            return;

        if (spawnPositions[pi.playerIndex] == null)
            throw new System.Exception("Not enough Spawn positions in level for all players.");

        Debug.Log($"Player {pi.playerIndex} joined!");

        pi.transform.position = spawnPositions[pi.playerIndex].position;
        playerTransforms[pi.playerIndex] = pi.transform;
        cameraManager.PTransforms.Add(pi.transform);
    }
    #endregion
}
