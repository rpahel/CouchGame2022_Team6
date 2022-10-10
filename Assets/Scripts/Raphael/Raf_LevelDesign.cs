using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf_LevelDesign : MonoBehaviour
{
    public Texture2D imageDeReference;
    public GameObject cubeMangeable;
    public GameObject cubeBedrock;
    public GameObject cubePiege;

    private void Start()
    {
        // Check la couleur de chaque pixel dans l'image et fait spawn un cube aux coordonnées correspondantes
        for (int i = 0; i < imageDeReference.height; i++)
        {
            for (int j = 0; j < imageDeReference.width; j++)
            {
                Color pixColor = imageDeReference.GetPixel(j, i);
                if (pixColor == Color.green)
                {
                    GameObject cube = Instantiate(cubeMangeable, new Vector3(j, i, 0), Quaternion.identity);
                    cube.name = "Cube (" + j.ToString() + ", " + i.ToString() + ") Mangeable";
                    cube.transform.parent = transform;
                }
                else if (pixColor == Color.black)
                {
                    GameObject cube = Instantiate(cubeBedrock, new Vector3(j, i, 0), Quaternion.identity);
                    cube.name = "Cube (" + j.ToString() + ", " + i.ToString() + ") Bedrock";
                    cube.transform.parent = transform;
                }
                else if (pixColor == Color.red)
                {
                    GameObject cube = Instantiate(cubePiege, new Vector3(j, i, 0), Quaternion.identity);
                    cube.name = "Cube (" + j.ToString() + ", " + i.ToString() + ") Piege";
                    cube.transform.parent = transform;
                }
            }
        }
    }
}
