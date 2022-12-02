using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;
using Unity.VisualScripting;

public class ApplicationManager : MonoBehaviour
{
    [Header("Application Options")]
    [SerializeField] private SystemLanguage language;


    private List<PlayerConfiguration> _playerConfigs;
    [Header("Players Configuration Options")]
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 4;

    [Header("Players GFX ")] [SerializeField]
    private List<PlayerGfxUI> listPlayersGfx = new List<PlayerGfxUI>();

    public List<PlayerGfxUI> ListPlayersGfx => listPlayersGfx;
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
        //LocalizationManager.Language = language.ToString();
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return _playerConfigs;
    }

    public void SetPlayerGfx(int index, PlayerGfxUI playerGfx)
    {
        _playerConfigs[index].PlayerSprite = playerGfx.player;
        _playerConfigs[index].PlayerFaceSprite = playerGfx.face;
        _playerConfigs[index].PlayerIcon = playerGfx.icon;
        _playerConfigs[index].PlayerColor = playerGfx.color;
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
    public Sprite PlayerSprite { get; set; }
    public Sprite PlayerFaceSprite { get; set; }
    public Sprite PlayerIcon { get; set; }
    
    public Color PlayerColor { get; set; }
}

[System.Serializable]
public class PlayerGfxUI
{
    public Sprite player;
    public Sprite face;
    public Sprite menu;
    public Sprite button;
    public Sprite icon;
    public Color color;
}