using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;

public class ApplicationManager : MonoBehaviour
{
    private List<PlayerConfiguration> _playerConfigs;

    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 4;

    public static ApplicationManager Instance { get; private set; }
    
    public  GAME_STATE GameState { get; private set; }
    
    //======================================= UI LOADING
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject playersLayout;
    [SerializeField] private Slider loadingSlider;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of PlayerConfigurationManager");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            _playerConfigs = new List<PlayerConfiguration>();
        }
        
        GameState = GAME_STATE.MENU;
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return _playerConfigs;
    }

    public void SetPlayerColor(int index, Material color)
    {
        _playerConfigs[index].PlayerMaterial = color;
    }

    public void ReadyPlayer(int index)
    {
        _playerConfigs[index].IsReady = true;

        if (_playerConfigs.Count >= minPlayers && _playerConfigs.Count <= maxPlayers && _playerConfigs.All(p => p.IsReady == true ))
        {
            GameState = GAME_STATE.LOADING;
            playersLayout.SetActive(false);
            loadingScreen.SetActive(true);
            StartCoroutine(LoadAsynchronously(1));
        }
    }

    private IEnumerator LoadAsynchronously(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
        
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("player joined " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if(!_playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            _playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }
    
    public void SetGameState(GAME_STATE state)
    {
        GameState = state;
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public Material PlayerMaterial { get; set; }
}