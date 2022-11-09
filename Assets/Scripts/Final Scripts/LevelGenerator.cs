using System;
using UnityEngine;
using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class LevelGenerator : MonoBehaviour
{
    #region Variables
    
    [SerializeField] private CinemachineTargetGroup cinemachine;
    [SerializeField] private GameObject[] playersUI;
    [SerializeField] private GameObject playerPrefab;

    //========================================================
    [Header("Image de r�f�rence.")]
    [SerializeField] private Texture2D image;
    public Texture2D ImageRef => image;

    [SerializeField, Header("Scale des cubes."), Range(1f, 3f)]
    private float echelle = 2.857143f;
    public float Echelle => echelle;

    //========================================================
    [Header("Cubes � utiliser.")]
    [SerializeField] private GameObject cubeEdible;
    public GameObject CubeEdible => cubeEdible;
    [SerializeField] private GameObject cubeBedrock;
    public GameObject CubeBedrock => cubeBedrock;
    [SerializeField] private GameObject cubeTrap;
    public GameObject CubeTrap => cubeTrap;

    [SerializeField] private GameObject cubeTNT;

    //========================================================
    [Header("Animation de spawn des cubes.")]
    [SerializeField] private SPAWN_ANIMATION spawnAnim;
    [SerializeField] private Ease smoothType;

    //========================================================
    [Header("Dur�es")]
    [SerializeField, Range(0.1f, 2f)] private float attenteAvantAnim;
    [SerializeField, Range(.1f, .3f)] private new float animation;
    [SerializeField, Range(0f, .1f)] private float entreCubesAnim;
    [SerializeField, Range(0f, .1f)] private float entreLignesAnim;
    [SerializeField,Range(0f,60f)] private float tntDelay;
    private float tntTimer;

    //========================================================
    private LEVEL_STATE levelState = LEVEL_STATE.NONE;
    public LEVEL_STATE LevelState => levelState;

    private Transform[] iniSpawns = new Transform[4];
    public Transform[] IniSpawns => iniSpawns;

    private GameObject parentObjCubes;
    public Transform ParentObjCubes => parentObjCubes.transform;

    //========================================================
    private Transform[,] cubesArray;
    public Transform[,] CubesArray { get => cubesArray; private set => cubesArray = value; }

    private int coroutinesRunning = 0;

    private List<TNT> allPaterns;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        cubesArray = new Transform[image.width, image.height];
        allPaterns = new List<TNT>();
    }

    private void Start()
    {
        GenerateLevel();
        StartCoroutine(PlayAnimation());
    }

    private void Update() {
        tntTimer += Time.deltaTime;

        if (tntTimer >= tntDelay) {
            tntTimer = 0f;

            int randX = UnityEngine.Random.Range(0,ImageRef.width - 1);
            int randY = UnityEngine.Random.Range(0, ImageRef.height - 1);

            Vector3 randomPos = new Vector3(randX, randY, 0);
            
            RaycastHit2D hit = Physics2D.CircleCast(randomPos,cubeTNT.transform.localScale.magnitude / 2,Vector3.right,cubeTNT.transform.localScale.magnitude / 2,1 << 3 | 1 << 6 | 1 << 8 | 1 << 10 | 1 << 11);

            while (hit.collider != null) {
                randX = UnityEngine.Random.Range(0,ImageRef.width - 1);
                randY = UnityEngine.Random.Range(0, ImageRef.height - 1);

                randomPos = new Vector3(randX, randY, 0);
                
                hit = Physics2D.CircleCast(randomPos,cubeTNT.transform.localScale.magnitude / 2,Vector3.right,cubeTNT.transform.localScale.magnitude / 2,1 << 3 | 1 << 6 | 1 << 8 | 1 << 10 | 1 << 11);
            }
            
            GameObject tnt = Instantiate(cubeTNT, randomPos, Quaternion.identity,parentObjCubes.transform);
            tnt.transform.localScale = Vector3.one * echelle;
            AssignRandomPattern(tnt.GetComponent<Cube_TNT>());
            
            foreach(Vector3 vec in tnt.GetComponent<Cube_TNT>().pattern.pattern)
                Debug.Log("vec " + vec);
        }
    }
    
    #endregion

    #region Custom_Functions
    [ContextMenu("Generate level")]
    public void GenerateLevel()
    {
        if (!image)
        {
            throw new System.NullReferenceException();
        }

        levelState = LEVEL_STATE.INITIALISING;
        GameManager.Instance.SetGameState(GAME_STATE.LOADING);

        parentObjCubes = new GameObject("Cubes");
        parentObjCubes.transform.parent = transform;

        GameObject parentObjSpawns = new GameObject("Initial Spawns");
        parentObjSpawns.transform.parent = transform;
        
        
        

        // Check la couleur de chaque pixel dans l'image et fait spawn un cube aux coordonn�es correspondantes
        int n = 0;
        for (int i = 0; i < image.height; i++)
        {
            for (int j = 0; j < image.width; j++)
            {
                Color pixColor = image.GetPixel(j, i);
                
                
                if(pixColor != Color.green && pixColor != Color.black && pixColor != Color.red && pixColor != Color.blue && pixColor != Color.white)
                    Debug.Log("color " + pixColor + " model " + new Color(1,1,0,1f));
                
                
                if (pixColor == Color.green)
                {
                    CreateCubeOnPlay(cubeEdible, parentObjCubes.transform, i, j);
                }
                else if (pixColor == Color.black)
                {
                    CreateCubeOnPlay(cubeBedrock, parentObjCubes.transform, i, j);
                }
                else if (pixColor == Color.red)
                {
                    CreateCubeOnPlay(cubeTrap, parentObjCubes.transform, i, j);
                }
                else if (pixColor == new Color(1f,1f,0,1f))
                {
                    CreateCubeOnPlay(cubeTNT,parentObjCubes.transform,i,j);
                }
                else if (pixColor == Color.blue)
                {
                    if(n >= 4)
                    {
                        throw new System.Exception("Plus de pixel bleu dans l'image de r�f�rence que le max de spawns autoris�s (4).");
                    }

                    iniSpawns[n] = new GameObject($"Spawn {n + 1}").transform;
                    iniSpawns[n].position = new Vector3(j * echelle, i * echelle, 0);
                    iniSpawns[n].localScale = Vector3.one;
                    iniSpawns[n].rotation = Quaternion.identity;
                    iniSpawns[n].transform.parent = parentObjSpawns.transform;

                    n++;
                }
            }
        }
        
        FindTNTPatterns();
    }


    void CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width, height, 0) * echelle, Quaternion.identity);
        cube.GetComponent<Cube>().unscaledPosition = new Vector2(width, height);
        cube.name = "Cube " + cube.GetComponent<Cube>().CubeType + " (" + width.ToString() + ", " + height.ToString() + ")";
        cube.transform.localScale = Vector3.one * echelle;
        cube.transform.parent = parentObj;
        //cubesArray[height, width] = cube.transform;
        cubesArray[width, height] = cube.transform;
    }
    #endregion

    #region Animations
    //============================
    IEnumerator PlayAnimation()
    {
        coroutinesRunning++;

        Vector3 endScale = Vector3.one * echelle;

        for (int i = 0; i < image.height; ++i)
        {
            for (int j = 0; j < image.width; ++j)
            {
                if (cubesArray[j, i] != null)
                {
                    cubesArray[j, i].localScale = Vector3.zero;
                }
            }
        }

        yield return new WaitForSecondsRealtime(attenteAvantAnim);
        // = LEVEL_STATE.LOADING;

        switch(spawnAnim)
        {
            case SPAWN_ANIMATION.LEFT_TO_RIGHT:
                for(int i = 0; i < image.width; ++i)
                {
                    for(int j = image.height - 1; j >= 0; --j)
                    {
                        if(cubesArray[i, j] != null)
                        {
                            StartCoroutine(ScaleCubeAnimation(cubesArray[i, j], endScale));
                            if(entreCubesAnim != 0)
                                yield return new WaitForSecondsRealtime(entreCubesAnim);
                        }
                    }

                    if(entreLignesAnim != 0)
                        yield return new WaitForSecondsRealtime(entreLignesAnim);
                }
                break;

            case SPAWN_ANIMATION.RIGHT_TO_LEFT:
                for (int i = image.width - 1; i >= 0; --i)
                {
                    for (int j = image.height - 1; j >= 0; --j)
                    {
                        if (cubesArray[i, j] != null)
                        {
                            StartCoroutine(ScaleCubeAnimation(cubesArray[i, j], endScale));
                            if (entreCubesAnim != 0)
                                yield return new WaitForSecondsRealtime(entreCubesAnim);
                        }
                    }

                    if (entreLignesAnim != 0)
                        yield return new WaitForSecondsRealtime(entreLignesAnim);
                }
                break;

            case SPAWN_ANIMATION.TOP_TO_BOTTOM:
                for (int i = image.height - 1; i >= 0; --i)
                {
                    for (int j = image.width - 1; j >= 0; --j)
                    {
                        if (cubesArray[j, i] != null)
                        {
                            StartCoroutine(ScaleCubeAnimation(cubesArray[j, i], endScale));
                            if (entreCubesAnim != 0)
                                yield return new WaitForSecondsRealtime(entreCubesAnim);
                        }
                    }

                    if (entreLignesAnim != 0)
                        yield return new WaitForSecondsRealtime(entreLignesAnim);
                }
                break;

            case SPAWN_ANIMATION.BOTTOM_TO_TOP:
                for (int i = 0; i < image.height; ++i)
                {
                    for (int j = image.width - 1; j >= 0; --j)
                    {
                        if (cubesArray[j, i] != null)
                        {
                            StartCoroutine(ScaleCubeAnimation(cubesArray[j, i], endScale));
                            if (entreCubesAnim != 0)
                                yield return new WaitForSecondsRealtime(entreCubesAnim);
                        }
                    }

                    if (entreLignesAnim != 0)
                        yield return new WaitForSecondsRealtime(entreLignesAnim);
                }
                break;

            default:
                for (int i = 0; i < image.height; ++i)
                {
                    for (int j = 0; j < image.width; ++j)
                    {
                        if (cubesArray[j, i] != null)
                        {
                            cubesArray[j, i].localScale = endScale;
                        }
                    }
                }
                break;
        }

        yield return new WaitUntil(() => coroutinesRunning == 1);

        for (int i = 0; i < image.height; ++i)
        {
            for (int j = 0; j < image.width; ++j)
            {
                CubeDestroyable tempCube;
                if (cubesArray[j, i] != null && cubesArray[j, i].TryGetComponent(out tempCube))
                {
                    tempCube.InitCubes();
                }
            }
        }

        levelState = LEVEL_STATE.LOADED;
        GameManager.Instance.SetGameState(GAME_STATE.PLAYING);
        
        var playerConfigs = PlayerConfigurationManager.Instance.GetPlayerConfigs().ToArray();
        cinemachine.m_Targets = new CinemachineTargetGroup.Target[playerConfigs.Length];

        for (int i = 0; i < playerConfigs.Length; i++) {
            var player = Instantiate(playerPrefab, iniSpawns[i].position, iniSpawns[i].rotation,
                gameObject.transform);
            playersUI[i].SetActive(true);
            player.GetComponent<PlayerManager>().imageUI = playersUI[i].transform.GetChild(0).GetComponent<Image>();
            player.GetComponent<PlayerManager>().textUI = playersUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            player.GetComponent<PlayerInputHandler>().InitializePlayer(playerConfigs[i]);
            cinemachine.m_Targets[i].target = player.transform;
            cinemachine.m_Targets[i].weight = 1;
        }

        coroutinesRunning--;
    }

    IEnumerator ScaleCubeAnimation(Transform cube, Vector3 endScale)
    {
        coroutinesRunning++;

        float t = 0f;
        float alpha;
        while(t < 1)
        {
            alpha = DOVirtual.EasedValue(0, 1, t, smoothType);
            cube.localScale = endScale * alpha;
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime / animation;
        }

        cube.localScale = endScale;

        coroutinesRunning--;
    }

    private void FindTNTPatterns() {
        foreach (TNT tntComp in GetComponents<TNT>()) 
            allPaterns.Add(tntComp);
        
    }

    private void AssignRandomPattern(Cube_TNT cube) => cube.pattern = allPaterns[Random.Range(0, allPaterns.Count - 1)];
    

    //public void PrintLevelAscii()
    //{
    //    string ascii = "";
    //    for (int i = image.height - 1; i >= 0; i--)
    //    {
    //        for (int j = 0; j < image.width; j++)
    //        {
    //            if (CubesArray[j, i])
    //                ascii += "G ";
    //            else
    //                ascii += "O ";
    //        }
    //
    //        ascii += "\n";
    //    }
    //
    //    Debug.Log(ascii);
    //}
    #endregion
}

enum SPAWN_ANIMATION
{
    NONE,
    LEFT_TO_RIGHT,
    RIGHT_TO_LEFT,
    TOP_TO_BOTTOM,
    BOTTOM_TO_TOP,
}