using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Unity.VisualScripting;
using DG.Tweening;

public class Cube_Edible : Cube
{
    #region Variables
    //======================================================================
    [SerializeField] private GameObject[] restes = new GameObject[4];
    [SerializeField] private GameObject loot;
    [SerializeField, Range(0, 100)] private int lootChance;
    [SerializeField] private ANIMATION_TYPE typeAnimation;
    [SerializeField, Range(1, 50)] private float vitessDeDeplacement;

    //======================================================================
    private List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }
    #endregion

    #region Unity_Functions
    private void Start()
    {
        InitCubes();
    }
    #endregion

    #region Custom_Functions
    /// <summary>
    /// Regarde les cubes qu'il y a autour et les ajoute à la liste cubesAutour
    /// </summary>
    private void InitCubes()
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

    /// <summary>
    /// Fais disparaitre le mesh du cube et le remplace par des meshs de miettes en fonction des cubes autour.
    /// </summary>
    /// <param name="playerTransform">(facultatif) Le transform du joueur qui mange le cube.</param>
    public void GetManged(Transform playerTransform = null)
    {
        // Signale aux cubes voisins que ce cube s'est fait mangé
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && cubesAutour[i].CubeType == CUBE_TYPE.EDIBLE)
            {
                (cubesAutour[i] as Cube_Edible).VoisinGotManged((i + 2) % 4);
            }
        }

        if(!playerTransform)
            cube.SetActive(false);

        // Fais apparaitre les restes en fonction des cubes voisins qui sont toujours là
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && !cubesAutour[i].isManged())
            {
                restes[i].SetActive(true);
            }
        }

        if(playerTransform)
        {
            cube.transform.parent = null;
            cube.GetComponent<Collider2D>().enabled = false;

            switch (typeAnimation)
            {
                case ANIMATION_TYPE.VERS_CAMERA:
                    StartCoroutine(Aspiration(cube, playerTransform));
                    break;
            }

        }
    }

    IEnumerator Aspiration(GameObject cube, Transform player)
    {
        Vector3 cubeStartPos = cube.transform.position;
        cube.transform.position -= Vector3.forward * 0.05f;
        float scaleFactor;
        float distanceCubeStart_Player;
        float distanceCubeCurrent_Player;
        float alpha;
        Vector2 direction;

        while (!isInRadius(cube.transform.position, player.transform.position, .5f))
        {
            direction = (player.transform.position - cube.transform.position).normalized;
            cube.transform.position += (Vector3)direction * (Time.deltaTime * vitessDeDeplacement);

            distanceCubeStart_Player = (player.transform.position - cubeStartPos).magnitude;
            distanceCubeCurrent_Player = (player.transform.position - cube.transform.position).magnitude;
            alpha = distanceCubeCurrent_Player / distanceCubeStart_Player;
            scaleFactor = DOVirtual.EasedValue(0, 2.857143f, alpha, Ease.OutBack, 2f);
            cube.transform.localScale = Vector3.one * scaleFactor;
            yield return new WaitForFixedUpdate();
        }

        cube.transform.parent = this.transform;
        cube.transform.SetSiblingIndex(0);
        cube.transform.localPosition = Vector2.zero;
        cube.GetComponent<Collider2D>().enabled = true;
        cube.SetActive(false);
    }

    /// <summary>
    /// Retire le reste collé au cube voisin qui vient de se faire manger.
    /// </summary>
    /// <param name="indexDuVoisin">L'index du voisin.</param>
    public void VoisinGotManged(int indexDuVoisin)
    {
        if (!cube.activeSelf)
        {
            restes[indexDuVoisin].SetActive(false);
        }
    }

    /// <summary>
    /// Fais exploser le cube et drop un loot.
    /// </summary>
    public void OnExploded()
    {
        float rand = Random.Range(0f, 1f);
        if (rand >= lootChance / 100f)
        {
            Instantiate(loot, transform.position, Quaternion.identity);
        }

        GetManged();
    }
    #endregion

    private bool isInRadius(Vector3 position, Vector3 targetPosition, float radius)
    {
        float distanceSqr = (targetPosition - position).sqrMagnitude;
        return distanceSqr <= (radius * radius);
    }
}

enum ANIMATION_TYPE
{
    VERS_CAMERA,
}