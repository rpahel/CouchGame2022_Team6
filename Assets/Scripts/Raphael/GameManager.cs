using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Data;

public class GameManager : MonoBehaviour
{
    #region Autres_Scripts
    //============================
    private PlayerInputManager pInputManager;
    [SerializeField] private CameraManager cManager;
    [SerializeField] private LevelGenerator lGenerator;
    public LevelGenerator LGenerator => lGenerator;
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
        }

        GameState = GAME_STATE.NONE;

        if (!TryGetComponent(out pInputManager))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerInputManager component found in GameManager game object !");
        }
    }
    #endregion

    #region Custom_Functions
    public void ChangeGameState(GAME_STATE newState)
    {
        GameState = newState;
        switch (GameState)
        {
            case GAME_STATE.PLAYING:
                pInputManager.EnableJoining();
                break;
        }
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
        cManager.UpdatePlayers(pi.transform);
    }
    #endregion
}
