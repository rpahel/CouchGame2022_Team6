using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Net.Mail;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Image = UnityEngine.UI.Image;

public class StatisticsManager : MonoBehaviour
{
    [HideInInspector] public Stats[] _arrayStats;
    public Stats[] ArrayStats => _arrayStats;

    [SerializeField] private int numberOfScenes;
    [SerializeField] private GameObject prefabStats;
    [SerializeField] private GameObject statsGoUI;
    [SerializeField] private Button nextLevelGoButton;
    [SerializeField] private Button mainMenuButton; 
    private List<StatsNameplate> listStatsNameplate = new List<StatsNameplate>();

    private ApplicationManager applicationManager;
    private bool canGoToNextLevel;
    
    public bool CanGoToNextLevel => canGoToNextLevel;

    private void Awake()
    {
        applicationManager = ApplicationManager.Instance;
        CheckLevels();
    }

    public void InitializeStats()
    {
        int count = GameManager.Instance.ListPlayersGo.Count;
        _arrayStats = new Stats[count];

        for (byte x = 0; x < count; x++)
        {
            _arrayStats[x] = new Stats(x);
            _arrayStats[x]._source = GameManager.Instance.ListPlayersGo[x].GetComponent<PlayerManager>();
        }
    }

    [ContextMenu("ShowStats")]
    public void ShowStats()
    {
        SetupStatsMenu();
        
        var sortedArray = _arrayStats.OrderByDescending(x => x._damageDeal);
        var listStats = sortedArray.ToList();
        
        byte index = 0;
        
        foreach (Stats playerStats in sortedArray)
        {
            listStatsNameplate[index].SetStats(index, listStats[index]._playerIndex, playerStats._damageDeal, playerStats._kill, playerStats._death);
            index++;
        }
    }

    public PlayerManager FindWinner() {
        List<Stats> winners = _arrayStats.OrderByDescending(x => x._damageDeal).ToList();
        
        // Attention non fonctionnement si plusieurs winner
        return winners[0]._source;
    }

    public void NextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        applicationManager.StartCoroutine(applicationManager.LoadAsynchronously(index + 1));
    }

    public void MainMenu()
    {
        Destroy(ApplicationManager.Instance.gameObject);
        SceneManager.LoadScene(0);
    }
    
    private void SetupStatsMenu()
    {
        var posY = 25;
        
        Debug.Log("size " + _arrayStats.Length);
        
        for(var x = 0; x < _arrayStats.Length; x++)
        {
            Debug.Log("enter");
            var stats = Instantiate(prefabStats, transform.position, Quaternion.identity);
            stats.transform.parent = statsGoUI.transform;
            stats.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
            posY -= 175;
            listStatsNameplate.Add(stats.GetComponent<StatsNameplate>());
        }
        
        statsGoUI.SetActive(true);
    }

    [ContextMenu("CheckLevels")]
    public void CheckLevels()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        
       if (numberOfScenes > index)
        {
            canGoToNextLevel = true;
            nextLevelGoButton.Select();
        }
        else
        {
            canGoToNextLevel = false;
            UpdateStatsButton();
        }
            
    }

    private void UpdateStatsButton()
    {
        nextLevelGoButton.gameObject.SetActive(false);
        var rectTranform = mainMenuButton.gameObject.GetComponent<RectTransform>();
        rectTranform.anchoredPosition = new Vector2(0,  rectTranform.anchoredPosition.y);
        mainMenuButton.Select();
    }
}

[System.Serializable]
public class Stats
{
    public PlayerManager _source;
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
