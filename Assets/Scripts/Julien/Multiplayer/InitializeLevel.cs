using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeLevel : MonoBehaviour
{
    [SerializeField] private CameraController cameraC;

    [SerializeField] private LevelGenerator levelGen;
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        playerSpawns = levelGen.IniSpawns;

        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();

        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]); 
            cameraC.Initialize(player.transform);
        }
        cameraC.EndOfInit();
        
    }
}
