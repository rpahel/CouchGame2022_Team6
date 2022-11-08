using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{

    [SerializeField] private LevelGenerator levelGenerator;
    
    private Transform _bestSpawnPoint;

    public void Respawn(GameObject playerGo)
    {
        playerGo.SetActive(false);


        _bestSpawnPoint.position = new Vector3(50f, 50f, 50f);
        
        for (var x = 0; x < levelGenerator.ImageRef.width; x++)
        {
            for (var y = 0; y < levelGenerator.ImageRef.height; y++)
            {
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
    }
}