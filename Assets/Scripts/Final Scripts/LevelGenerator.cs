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

    //========================================================
    [Header("Cubes à utiliser.")]
    [SerializeField] private GameObject cubeEdible;
    [SerializeField] private GameObject cubeBedrock;
    [SerializeField] private GameObject cubeTrap;

    //========================================================
    [Header("Animation de spawn des cubes.")]
    [SerializeField] private SPAWN_ANIMATION spawnAnim;
    [SerializeField] private Ease smoothType;

    //========================================================
    [Header("Durées")]
    [SerializeField, Range(0.1f, 2f)] private float attenteAvantAnim;
    [SerializeField, Range(.1f, .3f)] private new float animation;
    [SerializeField, Range(0f, .1f)] private float entreCubesAnim;
    [SerializeField, Range(0f, .1f)] private float entreLignesAnim;

    //========================================================
    private LEVEL_STATE levelState = LEVEL_STATE.NONE;
    public LEVEL_STATE LevelState => levelState;

    private Transform[] iniSpawns = new Transform[4];
    public Transform[] IniSpawns => iniSpawns;

    //========================================================
    private Transform[,] cubesArray;
    private int coroutinesRunning = 0;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        cubesArray = new Transform[image.height, image.width];
    }

    private void Start()
    {
        GenerateLevel();
        StartCoroutine(PlayAnimation());
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
        GameManager.Instance.ChangeGameState(GAME_STATE.LOADING);

        GameObject parentObjCubes = new GameObject("Cubes");
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
                else if (pixColor == Color.blue)
                {
                    if(n >= 4)
                    {
                        throw new System.Exception("Plus de pixel bleu dans l'image de référence que le max de spawns autorisés (4).");
                    }

                    iniSpawns[n] = new GameObject($"Spawn {n + 1}").transform;
                    iniSpawns[n].position = new Vector3(j * cubeEdible.transform.localScale.x, i * cubeEdible.transform.localScale.y, 0);
                    iniSpawns[n].localScale = Vector3.one;
                    iniSpawns[n].rotation = Quaternion.identity;
                    iniSpawns[n].transform.parent = parentObjSpawns.transform;

                    n++;
                }
            }
        }

        GameManager.Instance.SpawnPositions = iniSpawns;
    }

    void CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width * cubeToCreate.transform.localScale.x, height * cubeToCreate.transform.localScale.y, 0), Quaternion.identity);
        cube.name = "Cube " + cube.GetComponent<Cube>().CubeType + " (" + width.ToString() + ", " + height.ToString() + ")";
        cube.transform.parent = parentObj;
        cubesArray[height, width] = cube.transform;
    }
    #endregion

    #region Animations
    //============================
    IEnumerator PlayAnimation()
    {
        coroutinesRunning++;

        Vector3 endScale = cubeEdible.transform.localScale;

        for (int i = 0; i < image.height; ++i)
        {
            for (int j = 0; j < image.width; ++j)
            {
                if (cubesArray[i,j] != null)
                {
                    cubesArray[i, j].localScale = Vector3.zero;
                }
            }
        }

        yield return new WaitForSecondsRealtime(attenteAvantAnim);
        levelState = LEVEL_STATE.LOADING;

        switch(spawnAnim)
        {
            case SPAWN_ANIMATION.LEFT_TO_RIGHT:
                for(int i = 0; i < image.width; ++i)
                {
                    for(int j = image.height - 1; j >= 0; --j)
                    {
                        if(cubesArray[j, i] != null)
                        {
                            StartCoroutine(ScaleCubeAnimation(cubesArray[j, i], endScale));
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

            case SPAWN_ANIMATION.TOP_TO_BOTTOM:
                for (int i = image.height - 1; i >= 0; --i)
                {
                    for (int j = image.width - 1; j >= 0; --j)
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

            case SPAWN_ANIMATION.BOTTOM_TO_TOP:
                for (int i = 0; i < image.height; ++i)
                {
                    for (int j = image.width - 1; j >= 0; --j)
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

            default:
                for (int i = 0; i < image.height; ++i)
                {
                    for (int j = 0; j < image.width; ++j)
                    {
                        if (cubesArray[i, j] != null)
                        {
                            cubesArray[i, j].localScale = endScale;
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
                Cube_Edible tempCube;
                if (cubesArray[i, j] != null && cubesArray[i, j].TryGetComponent(out tempCube))
                {
                    tempCube.InitCubes();
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