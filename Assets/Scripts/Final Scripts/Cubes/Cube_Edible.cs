using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class Cube_Edible : Cube
{
    [SerializeField] private GameObject[] restes = new GameObject[4];

    private List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }

    private void Start()
    {
        InitCubes();
    }

    protected void InitCubes()
    {
        for (int i = 0; i < 4; i++)
        {
            cubesAutour.Add(null);

            RaycastHit hit;
            Vector3 dir = Quaternion.Euler(0, 0, -90 * i) * Vector3.up;
            if (Physics.Raycast(transform.position, dir, out hit, transform.localScale.x))
            {
                Cube cubeClone;
                if (hit.transform.parent && hit.transform.parent.TryGetComponent<Cube>(out cubeClone))
                {
                    cubesAutour[i] = cubeClone;
                }
            }
        }
    }

    public void GetManged()
    {
        // Signale aux cubes voisins que ce cube s'est fait mangé
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && cubesAutour[i].CubeType == CUBETYPE.EDIBLE)
            {
                (cubesAutour[i] as Cube_Edible).VoisinGotManged((i + 2) % 4);
            }
        }

        cube.SetActive(false);

        // Fais apparaitre les restes en fonction des cubes voisins qui sont toujours là
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && !cubesAutour[i].isManged())
            {
                restes[i].SetActive(true);
            }
        }
    }

    // Retire le reste collé au cube voisi qui vient de se faire manger
    public void VoisinGotManged(int indexDuVoisin)
    {
        if (!cube.activeSelf)
        {
            restes[indexDuVoisin].SetActive(false);
        }
    }


}
