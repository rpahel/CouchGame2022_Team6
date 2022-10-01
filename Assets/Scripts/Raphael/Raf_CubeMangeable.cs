using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf_CubeMangeable : MonoBehaviour
{
    [SerializeField] private GameObject              cube;
    [SerializeField] private GameObject[]            restes = new GameObject[4];
    [SerializeField] private List<Raf_CubeMangeable> cubesAutour = new List<Raf_CubeMangeable>(4);
    public List<Raf_CubeMangeable> CubesAutour { get => cubesAutour; set => cubesAutour = value; }

    private void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            cubesAutour.Add(null);

            RaycastHit hit;
            Vector3 dir = Quaternion.Euler(0, 0, -90 * i) * Vector3.up;
            if (Physics.Raycast(transform.position, dir, out hit, transform.localScale.x))
            {
                Raf_CubeMangeable cubeClone;
                if (hit.transform.parent.TryGetComponent<Raf_CubeMangeable>(out cubeClone))
                {
                    cubesAutour[i] = cubeClone;
                }
            }
        }
    }

    public void GetManged()
    {
        for(int i = 0; i < 4; i++)
        {
            if (cubesAutour[i])
            {
                cubesAutour[i].VoisinGotManged((i + 2) % 4);
            }
        }

        cube.SetActive(false);

        for(int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && !cubesAutour[i].isManged())
            {
                restes[i].SetActive(true);
            }
        }
    }

    public void VoisinGotManged(int indexDuVoisin)
    {
        if (!cube.activeSelf)
        {
            restes[indexDuVoisin].SetActive(false);
        }
    }

    public bool isManged()
    {
        return !cube.activeSelf;
    }
}
