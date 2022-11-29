using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Data;

public class GameManager : MonoBehaviour
{
    #region Autres_Scripts
    //============================
    private PlayerInputManager pInputManager;
    [SerializeField] private CameraManager cManager;
    [SerializeField] private LevelGenerator levelGenerator;
    public LevelGenerator LevelGenerator => levelGenerator;
    #endregion

    #region Variables
    //============================
    public static GameManager Instance { get; private set; }

    //============================
    [Header("Donn�es")]
    [SerializeField, Tooltip("Le prefab du projectile.")]
    private GameObject projectile;
    [SerializeField, Tooltip("Le nombre de projectiles en cache au d�but du jeu.")]
    private int projectileNumber;

    //============================
    public  GAME_STATE GameState { get; private set; }

    //============================
    private Transform[] playerTransforms = new Transform[4];
    public  Transform[] PlayerTransforms { get; }
    [SerializeField] private Color[] playerColors;

    //============================
    [HideInInspector] public Transform[] spawnPositions = new Transform[4];

    //============================
    private List<Projectile> projPool = new List<Projectile>();
    private Transform projPoolTransform; // Utile uniquement pour bien ranger les projectiles dans la hierarchy
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        GameState = GAME_STATE.NONE;

        if (!TryGetComponent(out pInputManager))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerInputManager component found in GameManager game object !");
        }

        if (!projectile)
            throw new Exception("Pas de projectile r�f�renc� dans le Game Manager.");

        if (projectileNumber <= 0)
            Debug.LogError($"ATTENTION t'as mis projectileNumber � {projectile} dans le Game Manager. Aucun projectile ne sera spawn�.");

        if(!levelGenerator)
            levelGenerator = FindObjectOfType<LevelGenerator>();

        GenerateProjectilePool(projectileNumber);
    }
    #endregion

    #region Custom_Functions

    /// <summary>
    /// Change l'�tat du jeu. Peut activer des fonctions en fonction de l'�tat choisit.
    /// </summary>
    /// <param name="newState">Le nouvel �tat du jeu.</param>
    public void ChangeGameState(GAME_STATE newState)
    {
        GameState = newState;
        switch (GameState) // J'utilise un switch au cas o� on a besoin de faire d'autres features pour d'autres �tats
        {
            case GAME_STATE.PLAYING:
                pInputManager.EnableJoining();
                break;
        }
    }

    /// <summary>
    /// Spawn un joueur et le place � un spawnPosition. Update la camera pour prendre en compte ce nouveau joueur.
    /// </summary>
    /// <exception cref="System.Exception"> Erreur quand il y a plus de joueurs que de points de spawn disponibles.</exception>
    public void OnPlayerJoined(PlayerInput pi)
    {
        if (pi.playerIndex >= 4)
            return;

        if (!spawnPositions[pi.playerIndex])
            throw new Exception("Not enough Spawn positions in level for all players.");

        Debug.Log($"Player {pi.playerIndex} joined!");

        pi.transform.position = spawnPositions[pi.playerIndex].position;
        playerTransforms[pi.playerIndex] = pi.transform;
        pi.GetComponent<PlayerSystemManager>().color = playerColors[pi.playerIndex];
        cManager.UpdatePlayers(pi.transform);
    }

    /// <summary>
    /// G�n�re un cache de projectiles pour �viter de les Instantiate � chaque fois.
    /// </summary>
    /// <param name="number">Nombre de projectiles � mettre en cache.</param>
    private void GenerateProjectilePool(int number)
    {
        GameObject parent = new GameObject();
        parent.name = "Projectile Pool";
        projPoolTransform = parent.transform;

        for(int i = 0; i < number; i++)
        {
            GameObject p = Instantiate(projectile, projPoolTransform);
            p.name = "Projectile " + i.ToString("00");
            projPool.Add(p.GetComponent<Projectile>());
            p.SetActive(false);
        }
    }

    /// <summary>
    /// Ajoute un projectile au cache de projectiles.
    /// </summary>
    /// <param name="number">Le nombre de projectile � ajouter.</param>
    private void AddProjectileToPool(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject p = Instantiate(projectile, projPoolTransform);
            p.name = "Projectile" + projPool.Count.ToString("00");
            p.SetActive(false);
            p.transform.SetSiblingIndex(i);
            projPool.Insert(i, p.GetComponent<Projectile>());
        }
    }

    /// <summary>
    /// Retourne un projectile qui n'est pas actif dans la sc�ne. S'il n'y en a pas, �a en cr�er 2 nouveaux.
    /// </summary>
    public Projectile GetAvailableProjectile()
    {
        Projectile p = projPool[0];

        if (p.gameObject.activeSelf)
        {
            AddProjectileToPool(2);
            SetAsLastOfList(projPool, p);
            p.transform.SetAsLastSibling();
            return GetAvailableProjectile(); // C'est dangereux mais normalement c'est cens� aller qu'� une seule profondeur. On devrait pas StackOverflow � cause de �a.
        }
        else
        {
            SetAsLastOfList(projPool, p);
            p.transform.SetAsLastSibling();
            return p;
        }
    }

    /// <summary>
    /// D�place un �l�ment � la fin de la liste.
    /// </summary>
    private void SetAsLastOfList<T>(List<T> list, T element)
    {
        list.Remove(element);
        list.Add(element);
    }
    #endregion
}
