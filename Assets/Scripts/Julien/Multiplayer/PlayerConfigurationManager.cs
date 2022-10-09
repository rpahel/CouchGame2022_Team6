using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerConfigurationManager : MonoBehaviour
{
    private List<PlayerConfiguration> _playerConfigs;

    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 4;

    public static PlayerConfigurationManager Instance { get; private set; }

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
            SceneManager.LoadScene("J_TestMultiplayer");
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