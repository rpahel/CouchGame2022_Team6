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
        var playerConfigs = ApplicationManager.Instance.GetPlayerConfigs().ToArray();
        manager.CinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[playerConfigs.Length];

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
            manager.CinemachineTargetGroup.m_Targets[i].target = player.transform;
            manager.CinemachineTargetGroup.m_Targets[i].weight = 1;
        }
        
        //GameManager2.Instance.StatsManager.InitializeStats();
    }
}