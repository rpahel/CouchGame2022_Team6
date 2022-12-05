using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using Image = UnityEngine.UI.Image;

public class StatisticsManager : MonoBehaviour
{
    public Stats[] _arrayStats;
    public Stats[] ArrayStats => _arrayStats;

    [SerializeField] private GameObject prefabStats;
    [SerializeField] private GameObject statsGoUI;
    [SerializeField] private List<StatsNameplate> listStatsNameplate = new List<StatsNameplate>();

    private ApplicationManager applicationManager;

    private void Awake()
    {
        applicationManager = ApplicationManager.Instance;
    }

    public void InitializeStats()
    {
        int count = GameManager.Instance.ListPlayersGo.Count;
        _arrayStats = new Stats[count];

        for (byte x = 0; x < count; x++)
        {
            _arrayStats[x] = new Stats(x);
        }
    }

    [ContextMenu("ShowStats")]
    public void ShowStats()
    {
        SetupStatsMenu();
        
        var sortedArray = _arrayStats.OrderByDescending(x => x._kill);

        var playerConfigs = applicationManager.GetPlayerConfigs().ToArray();
        byte index = 0;
        
        foreach (Stats playerStats in sortedArray)
        {
            
            listStatsNameplate[index].SetStats(index, playerStats._damageDeal, playerStats._kill, playerStats._death);
            index++;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Destroy(ApplicationManager.Instance.gameObject);
        SceneManager.LoadScene(0);
    }
    
    private void SetupStatsMenu()
    {
        var posY = 25;
        for(var x = 0; x < _arrayStats.Length; x++)
        {
            var stats = Instantiate(prefabStats, transform.position, Quaternion.identity);
            stats.transform.parent = statsGoUI.transform;
            stats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
            posY -= 175;
            listStatsNameplate.Add(stats.GetComponent<StatsNameplate>());
        }
        
        statsGoUI.SetActive(true);
    }
}

[System.Serializable]
public class Stats
{
    public byte _playerIndex;
    public float _damageDeal;
    //[SerializeField] private int _damageReceive;
    public byte _death;
    public byte _kill;

    public Stats(byte playerIndex)
    {
        this._playerIndex = playerIndex;
    }
}
