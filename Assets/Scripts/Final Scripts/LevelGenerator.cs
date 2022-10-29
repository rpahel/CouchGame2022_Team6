using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D imageDeReference;
    public GameObject cubeEdible;
    public GameObject cubeBedrock;
    public GameObject cubeTrap;

    private Transform[] iniSpawns = new Transform[4];
    public Transform[] IniSpawns => iniSpawns;

    private void Awake()
    {
        GenerateLevel();
    }

    [ContextMenu("Generate level")]
    public void GenerateLevel()
    {
        if (!imageDeReference)
        {
            throw new System.NullReferenceException();
        }

        GameObject parentObjCubes = new GameObject("Cubes");
        parentObjCubes.transform.parent = transform;

        GameObject parentObjSpawns = new GameObject("Initial Spawns");
        parentObjSpawns.transform.parent = transform;

        // Check la couleur de chaque pixel dans l'image et fait spawn un cube aux coordonn�es correspondantes
        int n = 0;
        for (int i = 0; i < imageDeReference.height; i++)
        {
            for (int j = 0; j < imageDeReference.width; j++)
            {
                Color pixColor = imageDeReference.GetPixel(j, i);
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
                        throw new System.Exception("Plus de pixel bleu dans l'image de LD que le max de spawns autoris�s (4).");
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

    void CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width * cubeToCreate.transform.localScale.x, height * cubeToCreate.transform.localScale.y, 0), Quaternion.identity);
        //cube.name = "Cube " + cube.GetComponent<Cube>().CubeType + " (" + width.ToString() + ", " + height.ToString() + ")";
        cube.transform.parent = parentObj;
    }
}