using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Data;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    #region Autres_Scripts
    //============================
    private PlayerInputManager pInputManager;
    [SerializeField] private CameraManager cManager;
    [SerializeField] private LevelGenerator lGenerator;
    public LevelGenerator LGenerator => lGenerator;
    #endregion

    #region Variables
    //============================
    public static GameManager Instance { get; private set; }

    //============================
    [Header("Données")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private int projectileNombre;

    //============================
    public  GAME_STATE GameState { get; private set; }

    //============================
    private Transform[] playerTransforms = new Transform[4];
    public  Transform[] PlayerTransforms { get; }

    //============================
    private Transform[] spawnPositions = new Transform[4];
    public  Transform[] SpawnPositions { get { return spawnPositions; } set { spawnPositions = value; } }

    //============================
    private List<Projectile> projPool = new List<Projectile>();
    public List<Projectile> ProjectilePool => projPool;

    //=============================
    private Transform projPoolTransform;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        GameState = GAME_STATE.NONE;

        if (!TryGetComponent(out pInputManager))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerInputManager component found in GameManager game object !");
        }

        if (projectile == null)
            throw new Exception("Pas de projectile référencé dans le Game Manager.");

        if (projectileNombre <= 0)
            Debug.LogError($"ATTENTION t'as mis projectileNombre à {projectile} dans le Game Manager. Aucun projectile ne sera spawné.");

        GenerateProjectilePool(projectileNombre);
    }
    #endregion

    #region Custom_Functions
    public void ChangeGameState(GAME_STATE newState)
    {
        GameState = newState;
        switch (GameState)
        {
            case GAME_STATE.PLAYING:
                pInputManager.EnableJoining();
                break;
        }
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        if (pi.playerIndex >= 3)
            return;

        if (spawnPositions[pi.playerIndex] == null)
            throw new System.Exception("Not enough Spawn positions in level for all players.");

        Debug.Log($"Player {pi.playerIndex} joined!");

        pi.transform.position = spawnPositions[pi.playerIndex].position;
        playerTransforms[pi.playerIndex] = pi.transform;
        cManager.UpdatePlayers(pi.transform);
    }

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
    #endregion
}
