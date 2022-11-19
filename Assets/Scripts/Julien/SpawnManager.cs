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
    //============================ Spawn/Respawn
    [Header("Spawn")]
    [SerializeField] float cooldownRespawn;
    [SerializeField] float cooldownInvincible;

    private Transform _bestSpawnPoint;

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
            GameManager.Instance.AddPlayer(player);
            manager.PlayersUI[i].SetActive(true);
            player.GetComponent<PlayerManager>().imageUI = manager.PlayersUI[i].transform.GetChild(0).GetComponent<Image>();
            player.GetComponent<PlayerManager>().textUI = manager.PlayersUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            manager.CinemachineTargetGroup.m_Targets[i].target = player.transform;
            manager.CinemachineTargetGroup.m_Targets[i].weight = 1;
        }
        
        GameManager.Instance.StatsManager.InitializeStats();
    }

    /*
    public void Respawn(GameObject playerGo)
    {
        playerGo.SetActive(false);


        //_bestSpawnPoint.position = new Vector3(50f, 50f, 50f);
        
        /*for (var x = 0; x < levelGenerator.ImageRef.width; x++)
        {
            for (var y = 0; y < levelGenerator.ImageRef.height; y++)
            {
                if(y == 0) continue;
                if (levelGenerator.CubesArray[x, y] == null || levelGenerator.CubesArray[x, y - 1] == null) continue;
                
                FindClosestPlayer(levelGenerator.CubesArray[x, y]);
            }
        }
        
        //Respawn, reset all values
        playerGo.SetActive(true);
        playerGo.GetComponent<PlayerManager>().ResetPlayer();
        playerGo.transform.position = _bestSpawnPoint.position;

    }

    private void FindClosestPlayer(Transform spawnPoint)
    {
        var listAlivePlayers = GameManager.Instance.ListPlayersGo.Where(playerGo => playerGo.GetComponent<PlayerManager>().State == PlayerState.Dead).ToList();

        foreach (var t in listAlivePlayers)
        {
            if (Vector3.Distance(t.transform.position, spawnPoint.position) <
                Vector3.Distance(_bestSpawnPoint.position, spawnPoint.position))
                _bestSpawnPoint = t.transform;
        }
    }*/

    public void Respawn2(GameObject playerGo)
    {
        //Play anim
        playerGo.SetActive(false);

        StartCoroutine(deathCoroutine(playerGo));
    }

    private void RespawnPlayerInstantiate(int index)
    {
        var manager = GameManager.Instance;
        var playerConfigs = ApplicationManager.Instance.GetPlayerConfigs().ToArray();
        var player = Instantiate(manager.PlayerPrefab, manager.LevelGenerator.IniSpawns[index].position, Quaternion.identity, manager.LevelGenerator.gameObject.transform); ;
        player.gameObject.name = "Player " + playerConfigs[index].PlayerIndex;
        GameManager.Instance.AddPlayer(player);

        manager.PlayersUI[index].SetActive(true);
        player.GetComponent<PlayerManager>().imageUI = manager.PlayersUI[index].transform.GetChild(0).GetComponent<Image>();
        player.GetComponent<PlayerManager>().textUI = manager.PlayersUI[index].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[index]);
        manager.CinemachineTargetGroup.m_Targets[index].target = player.transform;
        manager.CinemachineTargetGroup.m_Targets[index].weight = 1;
    }

    private IEnumerator deathCoroutine(GameObject playerGo)
    {
        yield return new WaitForSeconds(cooldownRespawn);
        GameManager.Instance.RemovePlayer(playerGo);
        var index= UsefullMethods.GetPlayerIndex(playerGo);

        RespawnPlayerInstantiate(index);
        Destroy(playerGo);
    }
}