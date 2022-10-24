using UnityEngine;
using Data;
using System.Collections;
using DG.Tweening;

public class LevelGenerator : MonoBehaviour
{
    #region Variables
    //============================
    [Header("Image de référence.")]
    [SerializeField] private Texture2D image;

    //============================
    [Header("Cubes à utiliser.")]
    [SerializeField] private GameObject cubeEdible;
    [SerializeField] private GameObject cubeBedrock;
    [SerializeField] private GameObject cubeTrap;

    //============================
    [Header("Animation de spawn des cubes.")]
    [SerializeField] private SPAWN_ANIMATION spawnAnim;
    [SerializeField] private Ease smoothType;

    [Header("Durées")]
    [SerializeField, Range(0.1f, 2f)] private float attenteAvantAnim;
    [SerializeField, Range(.1f, .3f)] private float anim;
    [SerializeField, Range(0f, .1f)] private float entreCubesAnim;
    [SerializeField, Range(0f, .5f)] private float entreLignesAnim;

    //============================
    private LEVEL_STATE levelState = LEVEL_STATE.NONE;
    public LEVEL_STATE LevelState => levelState;

    private Transform[] iniSpawns = new Transform[4];
    public Transform[] IniSpawns => iniSpawns;

    private Transform[,] cubesArray;
    private int coroutinesRunning = 0;
    #endregion

    #region Unity_Functions
    //============================
    private void Awake()
    {
        cubesArray = new Transform[image.height, image.width];
        GenerateLevel();
        StartCoroutine(PlayAnimation());
    }
    #endregion

    #region Custom_Functions
    //============================
    [ContextMenu("Generate level")]
    public void GenerateLevel()
    {
        if (!image)
        {
            throw new System.NullReferenceException();
        }

        levelState = LEVEL_STATE.INITIALISING;

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
    }

    //============================
    void CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width * cubeToCreate.transform.localScale.x, height * cubeToCreate.transform.localScale.y, 0), Quaternion.identity);
        cube.name = "Cube " + cube.GetComponent<Cube>().CubeType + " (" + width.ToString() + ", " + height.ToString() + ")";
        cube.transform.parent = parentObj;
        cubesArray[height, width] = cube.transform;
    }

    //============================
    bool isOOB(XY pos, out XY outPos)
    {
        outPos = new XY();

        if((pos.y >= image.height || pos.y < 0) || (pos.x >= image.width || pos.x < 0))
        {
            if (pos.y >= image.height) { outPos.y = image.height - 1; }
            if (pos.x >= image.width) { outPos.x = image.width - 1; }
            if (pos.y < 0) { outPos.y = 0; }
            if (pos.x < 0) { outPos.x = 0; }
            return true;
        }
        else
        {
            outPos.x = pos.x;
            outPos.y = pos.y;
            return false;
        }
    }

    //============================
    ARRAY_MOVE NextMove(ARRAY_MOVE currentMove)
    {
        if (spawnAnim == SPAWN_ANIMATION.SPIRAL_CLOCKWISE)
        {
            switch (currentMove)
            {
                case ARRAY_MOVE.UP:
                    return ARRAY_MOVE.RIGHT;

                case ARRAY_MOVE.RIGHT:
                    return ARRAY_MOVE.DOWN;

                case ARRAY_MOVE.DOWN:
                    return ARRAY_MOVE.LEFT;

                case ARRAY_MOVE.LEFT:
                    return ARRAY_MOVE.UP;
            }
        }
        else if (spawnAnim == SPAWN_ANIMATION.SPIRAL_COUNTERCLOCKWISE)
        {
            switch (currentMove)
            {
                case ARRAY_MOVE.UP:
                    return ARRAY_MOVE.LEFT;

                case ARRAY_MOVE.RIGHT:
                    return ARRAY_MOVE.UP;

                case ARRAY_MOVE.DOWN:
                    return ARRAY_MOVE.RIGHT;

                case ARRAY_MOVE.LEFT:
                    return ARRAY_MOVE.DOWN;
            }
        }

        return ARRAY_MOVE.UP;
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

            //case SPAWN_ANIMATION.SPIRAL_CLOCKWISE:
            //
            //    if (entreCubesAnim == 0)
            //        entreCubesAnim = 0.05f;
            //
            //    XY currentPos = new XY();
            //    currentPos.x = (image.width % 2 == 0) ? (int)(image.width * 0.5f) : (int)((image.width + 1) * 0.5f);
            //    currentPos.y = (image.height % 2 == 0) ? (int)(image.height * 0.5f) : (int)((image.height + 1) * 0.5f);
            //
            //    ARRAY_MOVE nextMove = ARRAY_MOVE.UP;
            //    bool isLooping = true;
            //    int max_iterations = 1;
            //
            //    yield return new WaitForSecondsRealtime(attenteAvantAnim);
            //
            //    StartCoroutine(ScaleCubeAnimation(cubesArray[currentPos.y, currentPos.x], endScale));
            //    while (isLooping)
            //    {
            //        for(int i = 0; i < 2; i++)
            //        {
            //            for(int j = 0; j < max_iterations; j++)
            //            {
            //                yield return new WaitForSecondsRealtime(entreCubesAnim);
            //
            //                switch (nextMove)
            //                {
            //                    case ARRAY_MOVE.UP:
            //                        currentPos.y++;
            //                        break;
            //                    case ARRAY_MOVE.RIGHT:
            //                        currentPos.x++;
            //                        break;
            //                    case ARRAY_MOVE.DOWN:
            //                        currentPos.y--;
            //                        break;
            //                    case ARRAY_MOVE.LEFT:
            //                        currentPos.x--;
            //                        break;
            //                }
            //
            //                if (isOOB(currentPos, out currentPos))
            //                {
            //                    isLooping = false;
            //                    break;
            //                }
            //
            //                StartCoroutine(ScaleCubeAnimation(cubesArray[currentPos.y, currentPos.x], endScale));
            //            }
            //            if (!isLooping)
            //                break;
            //
            //            nextMove = NextMove(nextMove);
            //        }
            //        if (!isLooping)
            //            break;
            //        
            //        max_iterations++;
            //    }
            //
            //    break;

            default:
                break;
        }

        yield return new WaitUntil(() => coroutinesRunning == 1);
        levelState = LEVEL_STATE.LOADED;

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
            t += Time.fixedDeltaTime / anim;
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
    //SPIRAL_CLOCKWISE,
    //SPIRAL_COUNTERCLOCKWISE,
}

enum ARRAY_MOVE
{
    UP,
    RIGHT,
    DOWN,
    LEFT,
}

struct XY
{
    public int x;
    public int y;

    public XY(int col = 0, int row = 0)
    {
        x = col;
        y = row;
    }
}