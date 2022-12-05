using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class SpawnManager : MonoBehaviour
{
    public void SpawnPlayers()
    {
        var manager = GameManager.Instance;
        var playerConfigs = ApplicationManager.Instance?.GetPlayerConfigs().ToArray();

        if (!ApplicationManager.Instance)
            return;

        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(manager.PlayerPrefab, manager.LevelGenerator.IniSpawns[i].position, manager.LevelGenerator.IniSpawns[i].rotation,
                manager.LevelGenerator.transform);
            player.gameObject.name = "Player " + playerConfigs[i].PlayerIndex;
            manager.AddPlayer(player);
            manager.PlayersUI[i].SetActive(true);
            player.GetComponent<PlayerManager>().imageUI = manager.PlayersUI[i].transform.GetChild(0).GetComponent<Image>();
            player.GetComponent<PlayerManager>().textUI = manager.PlayersUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            player.GetComponent<PlayerInputsHandler>().InitializePlayer(playerConfigs[i]);
            player.GetComponent<PlayerInputsHandler>().SetEnableInput(false);
            manager.CameraManager.UpdatePlayers(player.transform);

        }
        
        GameManager.Instance.StatsManager.InitializeStats();
    }
}