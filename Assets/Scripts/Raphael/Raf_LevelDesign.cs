using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf_LevelDesign : MonoBehaviour
{
    public Texture2D image;
    public GameObject cubeMangeable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int i = 0; i < image.width; i++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    if (image.GetPixel(i, j) == Color.black)
                    {
                        GameObject cube = Instantiate(cubeMangeable, new Vector3(i, j, 0), Quaternion.identity);
                        cube.transform.parent = transform;
                    }
                }
            }
        }
    }
}
