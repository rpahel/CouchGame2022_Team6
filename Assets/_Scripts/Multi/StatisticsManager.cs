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

    [SerializeField] private GameObject statsGoUI;
    [SerializeField] private List<GameObject> listStatsUI= new List<GameObject>();
    [SerializeField] private List<Image> listImagePlayer = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> listTextDamage = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> listKillsDamage = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> listDeathDamage = new List<TextMeshProUGUI>();

    private ApplicationManager _applicationManager;

    private void Awake()
    {
        _applicationManager = ApplicationManager.Instance;
    }

    /*public void InitializeStats()
    {
        int count = GameManager.Instance.ListPlayersGo.Count;
        _arrayStats = new Stats[count];

        for (byte x = 0; x < count; x++)
        {
            _arrayStats[x] = new Stats(x);
        }
    }*/

    [ContextMenu("ShowStats")]
    public void ShowStats()
    {
        SetupStatsMenu();
        
        var sortedArray = _arrayStats.OrderByDescending(x => x._kill);

        var playerConfigs = _applicationManager.GetPlayerConfigs().ToArray();
        var index = 0;
        
        foreach (Stats playerStats in sortedArray)
        {
            //listImagePlayer[index].color = playerConfigs[playerStats._playerIndex].sprite;
            listTextDamage[index].text = playerStats._damageDeal.ToString();
            listKillsDamage[index].text = playerStats._kill.ToString();
            listDeathDamage[index].text = playerStats._death.ToString();
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
        switch (_arrayStats.Length)
        {
            case 3:
                listStatsUI[3].SetActive(false);
                break;
            case 2:
                listStatsUI[3].SetActive(false);
                listStatsUI[2].SetActive(false);
                break;
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
