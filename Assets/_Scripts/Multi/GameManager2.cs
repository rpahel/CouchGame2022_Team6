using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Data;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance { get; private set; }
    
    private List<GameObject> _listPlayersGo = new List<GameObject>();
    public List<GameObject> ListPlayersGo => _listPlayersGo;


    //============================ Game Options
    [Header("Game Options")]
    [SerializeField, Tooltip("Time of the game, showing stats when 0")] private float gameDuration;
    private float _currentGameCooldown;

    [SerializeField] private bool skipCountdown = false;
    //[SerializeField] private bool showStatistics = false;
    [SerializeField] private TextMeshProUGUI gameCooldownText;

    //============================ Spawn/Respawn
    [Header("Spawn Data")]
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private CinemachineTargetGroup _cinemachine;
    [SerializeField] private GameObject[] _playersUI;
    [SerializeField] private GameObject _playerPrefab;

    private SpawnManager _spawnManager;
    private ApplicationManager _applicationManager;
    public StatisticsManager StatsManager { get; private set; }
    
    public LevelGenerator LevelGenerator => _levelGenerator;
    public CinemachineTargetGroup CinemachineTargetGroup => _cinemachine;
    public GameObject[] PlayersUI => _playersUI;
    public GameObject PlayerPrefab => _playerPrefab;
    
    //============================ Game Animator UI

    [SerializeField] private Animator animatorUI;

    //============================ Projectile
    [Header("Projectile Data")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private int projectileNombre;
    private List<Projectile> projPool = new List<Projectile>();
    public List<Projectile> ProjectilePool => projPool;
    private Transform projPoolTransform;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of GameManager");
        }
        else
        {
            Instance = this;
        }
        
        _spawnManager = GetComponent<SpawnManager>();
        StatsManager = GetComponent<StatisticsManager>();
        _applicationManager = ApplicationManager.Instance;
        _currentGameCooldown = gameDuration;

        if (projectile == null)
            throw new Exception("Pas de projectile r�f�renc� dans le Game Manager.");

        if (projectileNombre <= 0)
            Debug.LogError($"ATTENTION t'as mis projectileNombre � {projectile} dans le Game Manager. Aucun projectile ne sera spawn�.");

        GenerateProjectilePool(projectileNombre);
    }

    private void Update()
    {
        if (_applicationManager.GameState != GAME_STATE.PLAYING) return;
        
        _currentGameCooldown -= Time.deltaTime;
        gameCooldownText.text = ((int)_currentGameCooldown).ToString();
            
        if (_currentGameCooldown <= 0f)
        {
            _applicationManager.SetGameState(GAME_STATE.END);
            DisableAllInputs();
            StatsManager.ShowStats();
        }
    }

    public void SpawnAllPlayers()
    {
        _spawnManager.SpawnPlayers();
        if (!skipCountdown)
            animatorUI.SetTrigger("Countdown");
        else
            StartGame();
    }

    public void StartGame()
    {
        EnableAllInputs();
        _applicationManager.SetGameState(GAME_STATE.PLAYING);
    }

    private void EnableAllInputs()
    {
        foreach (var playerGo in _listPlayersGo)
        {
            //playerGo.GetComponent<PlayerManager>().EnableInputs();
        }
    }

    private void DisableAllInputs()
    {
        foreach (var playerGo in _listPlayersGo)
        {
            //playerGo.GetComponent<PlayerManager>().DisableInputs();
        }
    }

    /*public void RespawnPlayer(PlayerInput pi)
    {
        pi.transform.position = spawnPositions[pi.playerIndex].position;
        cManager.UpdatePlayers(pi.transform);
    }*/

    public void AddPlayer(GameObject playerGo)
    {
        _listPlayersGo.Add(playerGo);
    }

    public void RemovePlayer(GameObject playerGo)
    {
        _listPlayersGo.Remove(playerGo);
    }

    #region Projectile
    private void GenerateProjectilePool(int number)
    {
        GameObject parent = new GameObject();
        parent.name = "Projectile Pool";
        projPoolTransform = parent.transform;

        for(int i = 0; i < number; i++)
        {
            GameObject p = Instantiate(projectile, projPoolTransform);
            p.name = "Projectile " + i;
            projPool.Add(p.GetComponent<Projectile>());
            p.SetActive(false);
        }
    }

    private void AddProjectileToPool(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject p = Instantiate(projectile, projPoolTransform);
            p.name = "Projectile" + projPool.Count;
            p.SetActive(false);
            p.transform.SetSiblingIndex(i);
            projPool.Insert(i, p.GetComponent<Projectile>());
        }
    }

    public Projectile GetAvailableProjectile()
    {
        Projectile p = projPool[0];

        if (p.gameObject.activeSelf)
        {
            AddProjectileToPool(2);
            SetAsLastOfList(projPool, p);
            p.transform.SetAsLastSibling();
            return GetAvailableProjectile();
        }
        else
        {
            SetAsLastOfList(projPool, p);
            p.transform.SetAsLastSibling();
            return p;
        }
    }

    private void SetAsLastOfList<T>(List<T> list, T element)
    {
        list.Remove(element);
        list.Add(element);
    }

    public RaycastHit2D[] SquareCast(Vector2 origin, float size, bool drawDebug = false)
    {
        Vector2[] localPositions = new Vector2[8];
        Vector2[] worldPositions = new Vector2[8];
        RaycastHit2D[] hits = new RaycastHit2D[8];

        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0: localPositions[i] = new Vector2(0, 1); break;
                case 1: localPositions[i] = new Vector2(1, 1); break;
                case 2: localPositions[i] = new Vector2(1, 0); break;
                case 3: localPositions[i] = new Vector2(1, -1); break;
                case 4: localPositions[i] = new Vector2(0, -1); break;
                case 5: localPositions[i] = new Vector2(-1, -1); break;
                case 6: localPositions[i] = new Vector2(-1, 0); break;
                case 7: localPositions[i] = new Vector2(-1, 1); break;
            }

            worldPositions[i] = origin + .5f * size * localPositions[i];
        }

        for (int i = 0; i < 8; i++)
        {
            hits[i] = Physics2D.Linecast(worldPositions[i], worldPositions[(i + 4) % 8]);

#if UNITY_EDITOR
            if (drawDebug)
                Debug.DrawLine(worldPositions[i], worldPositions[(i + 4) % 8], Color.red, 5f);
#endif
        }

        return hits;
    }
    
    #endregion Projectile
}
