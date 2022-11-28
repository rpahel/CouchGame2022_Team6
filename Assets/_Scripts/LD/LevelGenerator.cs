using UnityEngine;
using Data;
using System.Collections;
using DG.Tweening;

public class LevelGenerator : MonoBehaviour
{
    #region Variables
    //========================================================
    [Header("Image de référence.")]
    [SerializeField] private Texture2D image;
    public Texture2D ImageRef => image;

    [SerializeField, Header("Scale des cubes."), Range(1f, 3f)]
    private float scale = 2.857143f;
    public float Scale => scale;

    //========================================================
    [Header("Cubes à utiliser.")]
    [SerializeField] private GameObject cubeEdible;
    public GameObject CubeEdible => cubeEdible;
    [SerializeField] private GameObject cubeBedrock;
    public GameObject CubeBedrock => cubeBedrock;
    [SerializeField] private GameObject cubeTrap;
    public GameObject CubeTrap => cubeTrap;

    //========================================================
    [Header("Animation de spawn des cubes.")]
    [SerializeField] private SPAWN_ANIMATION spawnAnim;
    [SerializeField] private Ease smoothType;

    //========================================================
    [Header("Durées de l'animation.")]
    [SerializeField, Range(0.1f, 2f)] private float beforeStart;
    [SerializeField, Range(.1f, .3f)] private new float animation;
    [SerializeField, Range(0f, .1f)] private float betweenCubes;
    [SerializeField, Range(0f, .1f)] private float betweenLines;

    //========================================================
    private LEVEL_STATE levelState = LEVEL_STATE.NONE;
    public LEVEL_STATE LevelState => levelState;

    private Transform[] iniSpawns = new Transform[4];
    public Transform[] IniSpawns => iniSpawns;

    private GameObject parentObjCubes;
    public Transform ParentObjCubes => parentObjCubes.transform;

    //========================================================
    private Transform[,] cubesArray; // Représentation en code du niveau.
    public Transform[,] CubesArray => cubesArray;

    private int coroutinesRunning = 0;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        cubesArray = new Transform[image.width, image.height];
    }

    private void Start()
    {
        GenerateLevel();
        StartCoroutine(PlayAnimation());
    }
    #endregion

    #region Custom_Functions
    //========================================================
    public void GenerateLevel()
    {
        if (!image)
        {
            throw new System.NullReferenceException("Pas d'image de référence dans le Level Generator.");
        }

        levelState = LEVEL_STATE.INITIALISING;
        GameManager.Instance.ChangeGameState(GAME_STATE.LOADING);

        parentObjCubes = new GameObject("Cubes");
        parentObjCubes.transform.parent = transform;

        GameObject parentObjSpawns = new GameObject("Initial Spawns");
        parentObjSpawns.transform.parent = transform;

        // Check la couleur de chaque pixel dans l'image et fait spawn un cube aux coordonnées correspondantes
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
                        throw new System.Exception("Plus de pixel bleu dans l'image de référence que le max de spawns autorisés (4).");
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

        GameManager.Instance.spawnPositions = iniSpawns;
    }

    void CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width, bool visible = true)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width, height, 0) * scale, Quaternion.identity);
        cube.GetComponent<Cube>().unscaledPosition = new Vector2Int(width, height);
        cube.name = "Cube " + cube.GetComponent<Cube>().CubeType + " (" + width.ToString("00") + ", " + height.ToString("00") + ")";
        cube.transform.localScale = Vector3.one * scale;
        cube.transform.parent = parentObj;
        cubesArray[width, height] = cube.transform;
        if (!visible)
            cube.SetActive(false);
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
                            if(betweenCubes != 0)
                                yield return new WaitForSecondsRealtime(betweenCubes);
                        }
                    }

                    if(betweenLines != 0)
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
                    tempCube.InitCubes(j, i, image.width, image.height);
                }
            }
        }

        levelState = LEVEL_STATE.LOADED;
        GameManager.Instance.ChangeGameState(GAME_STATE.PLAYING);

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