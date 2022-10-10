using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Raf_LevelDesign : MonoBehaviour
{
    public Texture2D    imageDeReference;
    public GameObject   cubeMangeable;
    public GameObject   cubeBedrock;
    public GameObject   cubePiege;

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

        GameObject parentObj = new GameObject("Cubes");
        parentObj.transform.parent = transform;

        // Check la couleur de chaque pixel dans l'image et fait spawn un cube aux coordonnées correspondantes
        for (int i = 0; i < imageDeReference.height; i++)
        {
            for (int j = 0; j < imageDeReference.width; j++)
            {
                Color pixColor = imageDeReference.GetPixel(j, i);
                if (pixColor == Color.green)
                {
                    CreateCubeOnPlay(cubeMangeable, parentObj.transform, i, j);
                }
                else if (pixColor == Color.black)
                {
                    CreateCubeOnPlay(cubeBedrock, parentObj.transform, i, j);
                }
                else if (pixColor == Color.red)
                {
                    CreateCubeOnPlay(cubePiege, parentObj.transform, i, j);
                }
            }
        }
    }

    void CreateCubeOnPlay(GameObject cubeToCreate, Transform parentObj, int height, int width)
    {
        GameObject cube = Instantiate(cubeToCreate, new Vector3(width, height, 0), Quaternion.identity);
        cube.name = "Cube Mangeable (" + width.ToString() + ", " + height.ToString() + ")";
        cube.transform.parent = parentObj;
    }
}
