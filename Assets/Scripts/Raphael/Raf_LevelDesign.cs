using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf_LevelDesign : MonoBehaviour
{
    public Texture2D imageDeReference;
    public GameObject cubeMangeable;

    private void Start()
    {
        for (int i = 0; i < imageDeReference.height; i++)
        {
            for (int j = 0; j < imageDeReference.width; j++)
            {
                if (imageDeReference.GetPixel(j, i) == Color.black)
                {
                    GameObject cube = Instantiate(cubeMangeable, new Vector3(j, i, 0), Quaternion.identity);
                    cube.name = "Cube (" + j.ToString() + ", " + i.ToString() + ")";
                    cube.transform.parent = transform;
                }
            }
        }
    }
}
