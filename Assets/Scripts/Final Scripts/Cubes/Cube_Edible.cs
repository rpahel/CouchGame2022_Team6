using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using DG.Tweening;

public class Cube_Edible : CubeDestroyable
{



    [Header("Effect")] 
    [SerializeField] private float doMoveEatTiming = 0.7f;
    [SerializeField] private float doScaleEatTiming = 0.8f;

    /// <summary>
    /// Regarde les cubes qu'il y a autour et les ajoute à la liste cubesAutour
    /// </summary>
    

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

    
   
}
