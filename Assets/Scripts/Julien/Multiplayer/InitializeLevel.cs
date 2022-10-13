using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup cinemachine;
    [SerializeField] private LevelGenerator levelGen;
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        playerSpawns = levelGen.IniSpawns;

        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        cinemachine.m_Targets = new CinemachineTargetGroup.Target[playerConfigs.Length];

        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            cinemachine.m_Targets[i].target = player.transform;
            cinemachine.m_Targets[i].weight = 1;
        }

    }
}
