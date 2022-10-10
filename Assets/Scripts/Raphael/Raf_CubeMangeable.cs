using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf_CubeMangeable : MonoBehaviour
{
    [SerializeField] private GameObject              cube;                          // Le cube principal
    [SerializeField] private GameObject[]            restes = new GameObject[4];    // Les restes du cubes (haut, droite, bas, gauche)
    
    private List<Raf_CubeMangeable> cubesAutour = new List<Raf_CubeMangeable>(4);
    public List<Raf_CubeMangeable> CubesAutour { get => cubesAutour; set => cubesAutour = value; }

    private void Start()
    {
        // Check si y'a des cubes voisins autour de cube et les ajoute à la liste cubesAutour (haut, droite, bas, gauche)
        for(int i = 0; i < 4; i++)
        {
            cubesAutour.Add(null);

            RaycastHit hit;
            Vector3 dir = Quaternion.Euler(0, 0, -90 * i) * Vector3.up;
            if (Physics.Raycast(transform.position, dir, out hit, transform.localScale.x))
            {
                Raf_CubeMangeable cubeClone;
                //if (hit.transform.parent.TryGetComponent<Raf_CubeMangeable>(out cubeClone))
                //{
                //    cubesAutour[i] = cubeClone;
                //}

                 //à revoir
            }
        }
    }

    public void GetManged()
    {

        // Signale aux cubes voisins que ce cube s'est fait mangé
        for(int i = 0; i < 4; i++)
        {
            if (cubesAutour[i])
            {
                cubesAutour[i].VoisinGotManged((i + 2) % 4);
            }
        }

        cube.SetActive(false);

        // Fais apparaitre les restes en fonction des cubes voisins qui sont toujours là
        for(int i = 0; i < 4; i++)
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

    // Vérifie si ce cube a été mangé ou pas
    public bool isManged()
    {
        return !cube.activeSelf;
    }
}
