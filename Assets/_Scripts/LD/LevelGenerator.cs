using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Variables
    //========================================================
    [Header("Image de r�f�rence.")]
    [SerializeField] private Texture2D image;
    public Texture2D ImageRef => image;

    [SerializeField, Header("Scale des cubes."), Range(1f, 3f)]
    private float scale = 2.857143f;
    public float Scale => scale;

    //========================================================
    [Header("Cubes � utiliser.")]
    [SerializeField] private GameObject cubeEdible;
    public GameObject CubeEdible => cubeEdible;
    [SerializeField] private GameObject cubeBedrock;
    public GameObject CubeBedrock => cubeBedrock;
    [SerializeField] private GameObject cubeTrap;
    public GameObject CubeTrap => cubeTrap;
    
    [SerializeField] private GameObject cubeTNT;
    public GameObject CubeTNT => cubeTNT;


    [SerializeField] private Sprite[] bedrockSprites;
    [SerializeField] private Color cubeEdibleDefaultColor;
    //========================================================
    [Header("Animation de spawn des cubes.")]
    [SerializeField] private SPAWN_ANIMATION spawnAnim;
    [SerializeField] private Ease smoothType;

    //========================================================
    [Header("Dur�es de l'animation.")]
    [SerializeField, Range(0.1f, 2f)] private float beforeStart;
    [SerializeField, Range(.1f, .3f)] private new float animation;
    [SerializeField, Range(0f, .1f)] private float betweenCubes;
    [SerializeField, Range(0f, .1f)] private float betweenLines;

    //========================================================
    [Header("Respawn des cubes.")]
    [SerializeField, Tooltip("Temps en secondes � attendre avant d'enclencher le respawn des cubes.")]
    private float _timeBeforeCubeRespawnStart;
    [SerializeField, Range(0.01f, 2f), Tooltip("Temps en secondes entre deux respawns de cubes.")]
    private float _respawnCooldown;
    [SerializeField, Range(0, 5), Tooltip("Temps en secondes avant de faire reapparaitre le premier cube de la liste de respawn, apres le temps d'enclenchement des respawns.")]
    private float _firstRespawnTime;
    private bool _canRespawnCube = true;
    private bool _canRespawnFirstCube;
    private List<CubeDestroyable> _cubeRespawnList = new();
    private Coroutine _respawnCoroutine;

    [Header("TNT")]
    //========================================================
    [SerializeField,Range(0f,60f)] private float tntDelay;
    private float tntTimer;
    [SerializeField,Tooltip("Available or no the random spawn of TNT ")] private bool randomSpawn;
    [SerializeField,Tooltip("The max TNT possible on the level ")] private int maxTNTCount;
    private int tntCount;
    [SerializeField,Tooltip("Count or not the number of tnt when the level is loaded")]private bool countLDTNT;
    [SerializeField,Tooltip("Available or no the possibility of tnt to respawn after his destruction")] private bool respawn;
    [HideInInspector] public List<Vector3> allEmptyPositions = new();
    private List<TNT> allPaterns;
    
    //========================================================
    private LEVEL_STATE levelState = LEVEL_STATE.NONE;
    public LEVEL_STATE LevelState => levelState;

    private Transform[] iniSpawns = new Transform[4];
    public Transform[] IniSpawns => iniSpawns;

    private GameObject parentObjCubes;
    public Transform ParentObjCubes => parentObjCubes.transform;

    //========================================================
    private Transform[,] cubesArray; // Repr�sentation en code du niveau.
    public Transform[,] CubesArray => cubesArray;
    private int coroutinesRunning = 0;
    
    
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

        tntCount = countLDTNT ? FindObjectsOfType<Cube_TNT>().Length : 0;
    }

    private void FixedUpdate()
    {
        _timeBeforeCubeRespawnStart = Mathf.Clamp(_timeBeforeCubeRespawnStart - Time.fixedDeltaTime, 0f, Mathf.Infinity);
        if(_timeBeforeCubeRespawnStart <= 0 && _respawnCoroutine == null)
            _respawnCoroutine = StartCoroutine(RespawnCube());
    }
    
    private void Update() {
        if (randomSpawn) {
            tntTimer += Time.deltaTime;
 
            if (tntTimer >= tntDelay) {
                tntTimer = 0f;
                GenerateRandomTNT();
            }
        }
    }
    
    
    #endregion

    #region Custom_Functions
    //========================================================
    
    private void GenerateRandomTNT() {
        
        if (tntCount >= maxTNTCount) {
            tntCount = 0;
            Debug.Log("Can't spawn random TNT. There is too much TNT on level. ");
            return;
        }

        Vector3 randomPos = allEmptyPositions[UnityEngine.Random.Range(0, allEmptyPositions.Count)];
                
        GameObject tnt = Instantiate(cubeTNT, randomPos, Quaternion.identity,parentObjCubes.transform);
        tnt.transform.localScale = Vector3.one * scale;
        AssignRandomPattern(tnt.GetComponent<Cube_TNT>());
        tntCount++;
    }
    
    
    private void FindTNTPatterns() {
        foreach (TNT tntComp in GetComponents<TNT>()) 
            allPaterns.Add(tntComp);
        
    }
 
    private void AssignRandomPattern(Cube_TNT cube) => cube.pattern = allPaterns[UnityEngine.Random.Range(0, allPaterns.Count)];
    
    public void GenerateLevel()
    {
        if (!image)
        {
            throw new System.NullReferenceException("Pas d'image de r�f�rence dans le Level Generator.");
        }

        levelState = LEVEL_STATE.INITIALISING;
        
        parentObjCubes = new GameObject("Cubes");
        parentObjCubes.transform.parent = transform;

        GameObject parentObjSpawns = new GameObject("Initial Spawns");
        parentObjSpawns.transform.parent = transform;
        
        FindTNTPatterns();

        // Check la couleur de chaque pixel dans l'image et fait spawn un cube aux coordonn�es correspondantes
        int n = 0;
        for (int i = 0; i < image.height; i++)
        {
            for (int j = 0; j < image.width; j++)
            {
                Color pixColor = image.GetPixel(j, i);

                if (pixColor == Color.white)
                {
                    CreateCubeOnPlay(cubeEdible, parentObjCubes.transform, i, j, false);
                }
                else if (pixColor == Color.green)
                {
                    CreateCubeOnPlay(cubeEdible, parentObjCubes.transform, i, j);
                }
                else if (pixColor == new Color(1f,1f,0,1f))
                {
                    GameObject obj = CreateCubeOnPlay(cubeTNT,parentObjCubes.transform,i,j);
                    AssignRandomPattern(obj.GetComponent<Cube_TNT>());
                }
                else if (pixColor == Color.black)
                {
                    CreateCubeOnPlay(cubeBedrock, parentObjCubes.transform, i, j);
                }
                else if (pixColor == Color.red)
                {
                    CreateCubeOnPlay(cubeTrap, parentObjCubes.transform, i, j);
                }
                else if (pixColor == Color.blue)
                {
                    CreateCubeOnPlay(cubeEdible, parentObjCubes.transform, i, j, false);

                    if (n >= 4)
                    {
                        throw new System.Exception("Plus de pixel bleu dans l'image de r�f�rence que le max de spawns autoris�s (4).");
                    }

                    iniSpawns[n] = new GameObject($"Spawn {n + 1}").transform;
                    iniSpawns[n].position = new Vector3(j * scale, i * scale, 0);
                    iniSpawns[n].localScale = Vector3.one;
                    iniSpawns[n].rotation = Quaternion.identity;
                    iniSpawns[n].transform.parent = parentObjSpawns.transform;

                    n++;
                }
                else
                {
                    CreateCubeOnPlay(cubeEdible, parentObjCubes.transform, i, j, false);
                }
            }
        }


        //allEmptyPositions = FindObjectsOfType<Cube_Edible>().ToList().Where(obj => obj.transform.GetChild(0).gameObject.activeSelf == false);

        foreach (Cube_Edible emptyCube in FindObjectsOfType<Cube_Edible>().ToList().Where(obj => obj.transform.GetChild(0).gameObject.activeSelf == false)) {
          allEmptyPositions.Add(emptyCube.transform.position);  
        }
    }

    GameObject CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width, bool visible = true)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width, height, 0) * scale, Quaternion.identity);
        Cube cubeScript = cube.GetComponent<Cube>();
        cubeScript.unscaledPosition = new Vector2Int(width, height);
        cubeScript.levelGenerator = this;
        cube.name = "Cube " + cube.GetComponent<Cube>().CubeType + " (" + width.ToString("00") + ", " + height.ToString("00") + ")";
        cube.transform.localScale = Vector3.one * scale;
        cube.transform.parent = parentObj;
        cubesArray[width, height] = cube.transform;

        if (!visible)
            cube.SetActive(false);
        else
        {
            if(cubeScript is Cube_Edible cubeEdible)
                cubeEdible.isOriginalCube = true;

        }
        if(cubeToCreate == cubeEdible)
        {
            cube.GetComponentInChildren<SpriteRenderer>().color = cubeEdibleDefaultColor;
        }

        if (cubeToCreate == cubeBedrock)
        {
            cubeToCreate.layer = LayerMask.NameToLayer("Limite");
            cubeToCreate.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Limite");

            if (height <= 0 || height >= image.height - 1 || width <= 0 || width >= image.width - 1)
            {

                cube.GetComponentInChildren<SpriteRenderer>().sprite = bedrockSprites[0];

            }
            else
            {
                cube.GetComponentInChildren<SpriteRenderer>().sprite = bedrockSprites[1];
            }
        }

        return cube;
    }

    public Vector2 FindSuitableRespawnSpot()
    {
        List<CubeDestroyable> list = new List<CubeDestroyable>();

        for(int y = 0; y < image.height; y++)
        {
            for(int x = 0; x < image.width; x++)
            {
                Transform cube = cubesArray[x, y];
                if(IsDestroyableAndEaten(cube))
                {
                    var cucube = cube.GetComponent<CubeDestroyable>();
                    if(CubeAboveIsEmpty(cucube))
                    {
                        list.Add(cucube);
                    }
                }
            }
        }

        if (list.Count == 0)
            return -Vector2.one;
        else
            return list[UnityEngine.Random.Range(0, list.Count)].transform.position;
    }

    private static bool IsDestroyableAndEaten(Transform cube)
    {
        if (cube.TryGetComponent(out CubeDestroyable cubeD))
        {
            if (cubeD.IsEaten)
                return true;
            else
                return false;
        }
        return false;
    }

    private bool CubeAboveIsEmpty(CubeDestroyable cube)
    {
        Vector2Int unscaledPos = cube.unscaledPosition;

        if (unscaledPos.y >= image.height)
            return false;

        if (IsDestroyableAndEaten(cubesArray[unscaledPos.x, unscaledPos.y]))
            return true;
        else
            return false;
    }
        #endregion

    #region Animations
    //=================================================
    IEnumerator PlayAnimation()
    {
        coroutinesRunning++;

        Vector3 endScale = Vector3.one * scale;

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

        yield return new WaitForSecondsRealtime(beforeStart);
        levelState = LEVEL_STATE.LOADING;

        switch (spawnAnim)
        {
            case SPAWN_ANIMATION.LEFT_TO_RIGHT:
                for (int i = 0; i < image.width; ++i)
                {
                    for (int j = image.height - 1; j >= 0; --j)
                    {
                        if (cubesArray[i, j] != null)
                        {
                            StartCoroutine(ScaleCubeAnimation(cubesArray[i, j], endScale));
                            if (betweenCubes != 0)
                                yield return new WaitForSecondsRealtime(betweenCubes);
                        }
                    }

                    if (betweenLines != 0)
                        yield return new WaitForSecondsRealtime(betweenLines);
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
                            if (betweenCubes != 0)
                                yield return new WaitForSecondsRealtime(betweenCubes);
                        }
                    }

                    if (betweenLines != 0)
                        yield return new WaitForSecondsRealtime(betweenLines);
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
                            if (betweenCubes != 0)
                                yield return new WaitForSecondsRealtime(betweenCubes);
                        }
                    }

                    if (betweenLines != 0)
                        yield return new WaitForSecondsRealtime(betweenLines);
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
                            if (betweenCubes != 0)
                                yield return new WaitForSecondsRealtime(betweenCubes);
                        }
                    }

                    if (betweenLines != 0)
                        yield return new WaitForSecondsRealtime(betweenLines);
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

        //PrintLevelAscii();

        // Initialisation des cubes
        for (int i = 0; i < image.height; ++i)
        {
            for (int j = 0; j < image.width; ++j)
            {
                Cube_Edible tempCube;
                if (cubesArray[j, i] && cubesArray[j, i].TryGetComponent(out tempCube))
                {
                    tempCube.InitCubes(image.width, image.height);
                }
            }
        }

        levelState = LEVEL_STATE.LOADED;
        GameManager.Instance.SpawnAllPlayers();
        coroutinesRunning--;
    }

    IEnumerator ScaleCubeAnimation(Transform cube, Vector3 endScale)
    {
        coroutinesRunning++;

        float t = 0f;
        float alpha;
        while (t < 1)
        {
            alpha = DOVirtual.EasedValue(0, 1, t, smoothType);
            cube.localScale = endScale * alpha;
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime / animation;
        }

        cube.localScale = endScale;

        coroutinesRunning--;
    }

    private IEnumerator RespawnCube() {
        
        if (!_canRespawnCube) yield break;
        if (_cubeRespawnList.Count <= 0) yield break;

        CubeDestroyable cube = _cubeRespawnList[0];
        
        RemoveFromRespawnList(cube);

        if(!GameManager.Instance.CanSpawnCubeAt(cube.transform.position))
        {
            AddToRespawnList(cube);
            _canRespawnFirstCube = false;
            StartCoroutine(RespawnFirstCube());
            yield break;
        }

        yield return new WaitUntil(() => _canRespawnFirstCube);
        yield return new WaitUntil(() => !cube.IsInAnimation);
        
        if (cube is Cube_TNT && !respawn)
            yield break;

        List<Cube_TNT> allTNTS = FindObjectsOfType<Cube_TNT>().ToList();
        List<GameObject> children = new List<GameObject>();
        
        foreach(Cube_TNT tnt in allTNTS)
            children.Add(tnt.transform.GetChild(0).gameObject);

        foreach (GameObject child in children) {
            if (Vector3Int.FloorToInt(child.transform.position) == Vector3Int.FloorToInt(cube.transform.position)) {
                Debug.Log("looog");
                RemoveFromRespawnList(cube);
                _canRespawnCube = false;
                StartCoroutine(CubeRespawnCooldown());
                _respawnCoroutine = null;
                yield break;
            }
        }

        cube.GetBarfed(cube.transform.position);
        cube.GetComponentInChildren<SpriteRenderer>().color = cubeEdibleDefaultColor;
        _canRespawnCube = false;
        StartCoroutine(CubeRespawnCooldown());
        _respawnCoroutine = null;
        
        yield break;
    }

    IEnumerator CubeRespawnCooldown()
    {
        yield return new WaitForSeconds(_respawnCooldown);
        _canRespawnCube = true;
        yield break;
    }

    IEnumerator RespawnFirstCube()
    {
        yield return new WaitForSeconds(_firstRespawnTime);
        _canRespawnFirstCube = true;
        yield break;
    }

    public void AddToRespawnList(CubeDestroyable cube)
    {
        if (!_cubeRespawnList.Contains(cube))
            _cubeRespawnList.Add(cube);

        if (_cubeRespawnList.Count == 1)
        {
            _canRespawnFirstCube = false;
            StartCoroutine(RespawnFirstCube());
        }
    }

    public void RemoveFromRespawnList(CubeDestroyable cube)
    {
        if (_cubeRespawnList.Contains(cube))
            _cubeRespawnList.Remove(cube);
    }

    /*public void PrintLevelAscii()
    {
        string ascii = "";
        for (int i = image.height - 1; i >= 0; i--)
        {
            for (int j = 0; j < image.width; j++)
            {
                if (CubesArray[j, i] && CubesArray[j, i].gameObject.activeSelf)
                    ascii += "G ";
                else
                    ascii += "O ";
            }
    
            ascii += "\n";
        }
    
        Debug.Log(ascii);
    }*/

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