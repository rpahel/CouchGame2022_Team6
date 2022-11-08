using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEditor.PackageManager.Requests;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private List<GameObject> _listPlayersGo = new List<GameObject>();
    
    public List<GameObject> ListPlayersGo=> _listPlayersGo;
    
    public  GAME_STATE GameState { get; private set; }
    
    //============================
    [Header("Donn�es")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private int projectileNombre;
    private List<ProjectileRaph> projPool = new List<ProjectileRaph>();
    public List<ProjectileRaph> ProjectilePool => projPool;
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
            DontDestroyOnLoad(Instance);
        }
        GameState = GAME_STATE.NONE;
        
        if (projectile == null)
            throw new Exception("Pas de projectile r�f�renc� dans le Game Manager.");

        if (projectileNombre <= 0)
            Debug.LogError($"ATTENTION t'as mis projectileNombre � {projectile} dans le Game Manager. Aucun projectile ne sera spawn�.");

        GenerateProjectilePool(projectileNombre);
        
    }

    public void AddPlayer(GameObject playerGo)
    {
        _listPlayersGo.Add(playerGo);
    }

    public void SetGameState(GAME_STATE state)
    {
        GameState = state;
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
            projPool.Add(p.GetComponent<ProjectileRaph>());
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
            projPool.Insert(i, p.GetComponent<ProjectileRaph>());
        }
    }

    public ProjectileRaph GetAvailableProjectile()
    {
        ProjectileRaph p = projPool[0];

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
}
