using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Net.Mail;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
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

    private bool IsFinished;

    private void Awake()
    {
        applicationManager = ApplicationManager.Instance;
        CheckLevels();
    }

    public void InitializeStats()
    {
        int count = GameManager.Instance.ListPlayersGo.Count;
        _arrayStats = new Stats[count];

        for (byte x = 0; x < count; x++) {
            _arrayStats[x] = new Stats(x);
            
            PlayerConfiguration playerConfig = ApplicationManager.Instance.GetPlayerConfigs()[x];
            _arrayStats[x]._source = GameManager.Instance.ListPlayersGo[x].GetComponent<PlayerManager>();
        }
    }

    [ContextMenu("ShowStats")]
    public void ShowStats()
    {
        StartCoroutine(SetupStatsMenu());
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
    
    private IEnumerator SetupStatsMenu() {
        Vector2 startPosition = new Vector2(-51,-22);

        for(var x = 0; x < _arrayStats.Length; x++)
        {
            var stats = Instantiate(prefabStats, transform.position, Quaternion.identity);
            stats.transform.parent = statsGoUI.transform;

            startPosition.x = -51 + (465 * x);
            stats.GetComponent<RectTransform>().anchoredPosition = startPosition;
            listStatsNameplate.Add(stats.GetComponent<StatsNameplate>());
            
            stats.SetActive(false);
        }

        statsGoUI.SetActive(true);

        var sortedArray = _arrayStats.OrderByDescending(x => x._damageDeal);
        var listStats = sortedArray.ToList();

        if (IsFinished) {
            listStats.Clear();
            foreach (PlayerConfiguration config in ApplicationManager.Instance.GetPlayerConfigs()) {
                listStats.Add(config.globalStats.globalStats);
            }

            listStats = listStats.OrderByDescending(x => x._damageDeal).ToList();
            GameManager.Instance.podiumText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            GameManager.Instance.podiumText.gameObject.SetActive(false);
            
            // Tester avec vainqueur bleue derniere map mais vert bainqueur all time 
        }

        for (int i = 0; i < listStats.Count; i++) {
            var playerStats = listStats[i];

            var playerConfigs = ApplicationManager.Instance.GetPlayerConfigs();
            
            float damageDeal = IsFinished ?  playerConfigs[playerStats._playerIndex].globalStats.globalStats._damageDeal : playerStats._damageDeal;
            int kill = IsFinished ? playerConfigs[playerStats._playerIndex].globalStats.globalStats._kill : playerStats._kill;
            int death = IsFinished ? playerConfigs[playerStats._playerIndex].globalStats.globalStats._death : playerStats._death;
            
            listStatsNameplate[i].SetStats((byte)i, listStats[i]._playerIndex,damageDeal,kill,death);
        }

        for (int i = listStatsNameplate.Count - 1;i >= 0;i--) {
            listStatsNameplate[i].gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1.5f);
        }

        if (IsFinished) {
            // Recup tt les objets ou le rank n'est pas 1 
            yield return new WaitForSeconds(1f);
            
            FindObjectOfType<PlayerStateSystem>().PlaySound("Player_Kill");
            
            foreach (StatsNameplate statsNameplate in FindLoosers()) {
                GameObject looserBackground = statsNameplate.textRank.transform.parent.GetChild(statsNameplate.textRank.transform.parent.childCount - 1).gameObject;
                looserBackground.SetActive(true);
            }
        }
        
        yield return null;
    }


    private List<StatsNameplate> FindLoosers() {
        return listStatsNameplate.Where(statsNameplate => int.Parse(statsNameplate.textRank.text) != 1).ToList();
    }

    public Color32 ToColor(int HexVal)
    {
        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, 255);
    }
    

    [ContextMenu("CheckLevels")]
    public void CheckLevels()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        
       if (numberOfScenes > index)
        {
            canGoToNextLevel = true;
            nextLevelGoButton.Select();
            IsFinished = false;
        }
        else
        {
            canGoToNextLevel = false;
            UpdateStatsButton();
            IsFinished =  true;
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

public class GlobalStats {
    public Stats globalStats;

    public GlobalStats(byte playerIndex) => globalStats = new Stats(playerIndex);
    
}
