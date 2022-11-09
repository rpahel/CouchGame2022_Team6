using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

using DG.Tweening;

public class CubeDestroyable : Cube {
    
    protected List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }

    
    [SerializeField] private GameObject loot;
    
    [SerializeField] private float doScaleExplodeTiming = 0.5f;
    [SerializeField] private bool rotateOnExplode = true;
    
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
    
    public void OnExploded() {
        float rand = Random.Range(0f, 1f);
        if (rand >= 0.90f && loot != null)
        {
            // Debug.Log($"{gameObject.name}, {rand}.");
            Instantiate(loot, transform.position, Quaternion.identity);
        }

        GetExploded();
    }

    public void GetExploded()
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
    
    public IEnumerator ExplodeEffect()
    {
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        transform.DOScale(Vector3.zero, doScaleExplodeTiming);
        if(rotateOnExplode)
            transform.DORotate(new Vector3(0, 0, Random.Range(-180, 181)), doScaleExplodeTiming);
        yield return new WaitForSeconds(doScaleExplodeTiming);
        cube.SetActive(false);

        // Fais apparaitre les restes en fonction des cubes voisins qui sont toujours là
        
    }
}
