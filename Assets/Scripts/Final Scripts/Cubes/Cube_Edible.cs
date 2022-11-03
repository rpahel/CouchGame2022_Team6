using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using DG.Tweening;

public class Cube_Edible : Cube
{
    [SerializeField] private GameObject[] restes = new GameObject[4];
    [SerializeField] private GameObject loot;

    private List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }
    
    


    [Header("Effect")] 
    [SerializeField] private float doMoveEatTiming = 0.7f;
    [SerializeField] private float doScaleEatTiming = 0.8f;
    [SerializeField] private float doScaleExplodeTiming = 0.5f;
    [SerializeField] private bool rotateOnExplode = true;

    /*private void Start()
    {
        InitCubes();
    }*/
    
    /// <summary>
    /// Regarde les cubes qu'il y a autour et les ajoute à la liste cubesAutour
    /// </summary>
    public void InitCubes()
    {
        for (int i = 0; i < 4; i++)
        {
            cubesAutour.Add(null);

            Vector3 dir = Quaternion.Euler(0, 0, -90 * i) * Vector3.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, transform.localScale.x);
            if (hit)
            {
                Cube cubeClone;
                if (hit.transform.parent && hit.transform.parent.TryGetComponent<Cube>(out cubeClone))
                {
                    cubesAutour[i] = cubeClone;
                }
            }
        }
    }

    public void GetManged(EatScript eat)
    {
        // Signale aux cubes voisins que ce cube s'est fait mangé
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && cubesAutour[i].CubeType == CUBETYPE.EDIBLE)
            {
                (cubesAutour[i] as Cube_Edible).VoisinGotManged((i + 2) % 4);
            }
        }

        StartCoroutine(EatEffect(eat));

    }

    private IEnumerator EatEffect(EatScript eat)
    {
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        transform.DOMove(eat.gameObject.transform.position, doMoveEatTiming).SetEase((Ease.OutBounce));
        transform.DOScale(Vector3.zero, doScaleEatTiming);
        yield return new WaitForSeconds(doMoveEatTiming);
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

    // Retire le reste collé au cube voisin qui vient de se faire manger
    public void VoisinGotManged(int indexDuVoisin)
    {
        if (!cube.activeSelf)
        {
            restes[indexDuVoisin].SetActive(false);
        }
    }

    public void OnExploded()
    {
        float rand = Random.Range(0f, 1f);
        if (rand >= 0.90f)
        {
            Debug.Log($"{gameObject.name}, {rand}.");
            Instantiate(loot, transform.position, Quaternion.identity);
        }

        GetExploded();
    }

    private void GetExploded()
    {
        // Signale aux cubes voisins que ce cube s'est fait mangé
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && cubesAutour[i].CubeType == CUBETYPE.EDIBLE)
            {
                (cubesAutour[i] as Cube_Edible).VoisinGotManged((i + 2) % 4);
            }
        }

        StartCoroutine(ExplodeEffect());
    }
    
    private IEnumerator ExplodeEffect()
    {
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        transform.DOScale(Vector3.zero, doScaleExplodeTiming);
        if(rotateOnExplode)
            transform.DORotate(new Vector3(0, 0, Random.Range(-180, 181)), doScaleExplodeTiming);
        yield return new WaitForSeconds(doScaleExplodeTiming);
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
}
