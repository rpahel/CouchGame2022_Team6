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
using TMPro;
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

    [Header("PlayerColors remaining"), SerializeField]
    private List<ColorPlayer> listColorRemainingInspector = new List<ColorPlayer>();
    
    private List<ColorPlayer> listColorRemaining = new List<ColorPlayer>();
    public List<ColorPlayer> ListColorRemaining => listColorRemaining;
    [HideInInspector] public List<PlayerSetupMenuController> listSetupMenuControllers = new List<PlayerSetupMenuController>();
    public static ApplicationManager Instance { get; private set; }
    
    public  GAME_STATE GameState { get; private set; }
    
    //======================================= UI LOADING
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject playersLayout;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI pressButtonText;
    [SerializeField] private TextMeshProUGUI indicativeText;

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
        listColorRemaining.AddRange(listColorRemainingInspector);
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
        _playerConfigs[index].PlayerHdrColor = playerGfx.hdrColor;
        _playerConfigs[index].playerGradient = playerGfx.gradientColor;
        _playerConfigs[index].PlayerArrowSprite = playerGfx.arrow;
        _playerConfigs[index].MenuSprite = playerGfx.menu;
        _playerConfigs[index].globalStats = new GlobalStats((byte)index);
    }

    public void SetPlayerSkin(int index, Sprite skin)
    {
        _playerConfigs[index].PlayerSprite = skin;
    }
    public void ReadyPlayer(int index)
    {
        _playerConfigs[index].IsReady = true;

        if (_playerConfigs.Count >= minPlayers && _playerConfigs.Count <= maxPlayers && _playerConfigs.All(p => p.IsReady == true ))
        {
            GameState = GAME_STATE.LOADING;
            playersLayout.SetActive(false);
            loadingScreen.SetActive(true);
            StartCoroutine(LoadAsynchronously(2));
            pressButtonText.text = null;
            indicativeText.text = null;

        }
        else if (_playerConfigs.Count < minPlayers)
        {
            indicativeText.text = "NEED AT LEAST " + minPlayers + " PLAYER TO START";
        }
    }

    public IEnumerator LoadAsynchronously(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if(loadingSlider != null)
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
        
        if (_playerConfigs.Any(p => p.IsReady != true) && _playerConfigs.Count >= minPlayers)
        {
            indicativeText.text = "GET READY TO START";
        }
    }
    
    public void SetGameState(GAME_STATE state)
    {
        GameState = state;
        if(state == GAME_STATE.PLAYING)
        {
            GameManager.Instance.AudioManager.Play("Clock_Start");
        }
    }
    
    public void DeleteColor(string name)
    {
        var index = listColorRemaining.TakeWhile(color => color.colorName != name).Count();
        listColorRemaining.Remove(listColorRemaining[index]);
        RefreshColors();
    }

    public void BackOnColorSelector(ColorPlayer color)
    {
        listColorRemaining.Insert(listColorRemaining.Count - 1, color);
        var listOredered =  listColorRemaining.OrderByDescending(x => -x.index);
        listColorRemaining = listOredered.ToList();
        RefreshColors();
    }

    private void RefreshColors()
    {
        foreach (PlayerSetupMenuController controller in listSetupMenuControllers)
        {
            controller.RefreshColors();
        }
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
    public Sprite PlayerArrowSprite { get; set; }
    public Sprite PlayerIcon { get; set; }
    
    public Color PlayerColor { get; set; }
    public Color PlayerHdrColor { get; set; }
    public Gradient playerGradient { get; set; }
    
    public Sprite MenuSprite { get; set; }

    public GlobalStats globalStats { get; set; }
}



[System.Serializable]
public class PlayerGfxUI
{
    public Sprite player;
    public Sprite face;
    public Sprite skin;
    public Sprite arrow;
    public Sprite menu;
    public Sprite button;
    public Sprite buttonSkin;
    public Sprite icon;
    public Color color;
    [ColorUsage(true, true)]
    public Color hdrColor;
    public Gradient gradientColor;
}

[System.Serializable]
public class ColorPlayer
{
    public string colorName;
    public GameObject buttonPrefab;
    public int index;
}