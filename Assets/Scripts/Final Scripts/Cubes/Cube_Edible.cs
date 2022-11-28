using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using DG.Tweening;

public class Cube_Edible : CubeDestroyable
{
    [SerializeField] private GameObject[] restes = new GameObject[4];



    [Header("Effect")] 
    [SerializeField] private float doMoveEatTiming = 0.7f;
    [SerializeField] private float doScaleEatTiming = 0.8f;

    /// <summary>
    /// Regarde les cubes qu'il y a autour et les ajoute à la liste cubesAutour
    /// </summary>
    public void InitCubes(int width, int height)
    {
        Vector3 dir;
        Vector2 targetPos;
        Transform targetCubeTransform;

        for (int i = 0; i < 4; i++)
        {
            cubesAutour.Add(null);

            dir = Quaternion.Euler(0, 0, -90 * i) * Vector3.up;

            targetPos = unscaledPosition + (Vector2)dir;
            targetCubeTransform = GameManager.Instance.LevelGenerator.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];

            if (targetCubeTransform)
            {
                cubesAutour[i] = targetCubeTransform.GetComponent<Cube>();
            }
        }

        if (!gameObject.activeSelf)
        {
            GetManged(null, false);
            gameObject.SetActive(true);
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

    public void GetManged(Transform playerTransform = null, bool showRestes = true)
    {
        isManged = true;

        // Signale aux cubes voisins que ce cube s'est fait mangé
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && cubesAutour[i].CubeType == CUBETYPE.EDIBLE)
            {
                (cubesAutour[i] as Cube_Edible).VoisinGotManged((i + 2) % 4);
            }
        }

        if (!playerTransform)
            cube.SetActive(false);

        // Fais apparaitre les restes en fonction des cubes voisins qui sont toujours là
        if (showRestes)
        {
            for (int i = 0; i < 4; i++)
            {
                if (cubesAutour[i] && !cubesAutour[i].IsManged)
                {
                    restes[i].SetActive(true);
                }
            }
        }

        if (playerTransform)
        {
            cube.transform.parent = null;
            cube.GetComponent<Collider2D>().enabled = false;

            //StartCoroutine(Aspiration(cube, playerTransform));
        }
    }

    public void GetVomited(Vector2 impactPos)
    {
        foreach (GameObject reste in restes)
        {
            reste.SetActive(false);
        }

        cube.transform.localScale = Vector3.one;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.SetActive(true);
        isManged = false;

        //StartCoroutine(VomitedAnimation(impactPos));
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
            if (cubesAutour[i] && !cubesAutour[i].IsManged)
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

    public IEnumerator ExplodeEffect() {
        base.ExplodeEffect();
        
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
            if (cubesAutour[i] && !cubesAutour[i].IsManged)
            {
                restes[i].SetActive(true);
            }
        }

        yield return null;
    }
   
}
