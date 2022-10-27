using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Data;

public class Cube_Edible : Cube
{
    #region Variables
    //======================================================================
    [SerializeField, Tooltip("Les restes en enfant ce gameObject.")]
    private GameObject[] restes = new GameObject[4];
    [SerializeField, Tooltip("Le prefab de l'item que ce cube droppera lors de sa destruction.")]
    private GameObject loot;
    [SerializeField, Range(0, 100), Tooltip("La chance en pourcentage que ce cube drop un loot lors de sa destruction.")]
    private int lootChance;
    //[SerializeField, Range(1, 50)] private float vitesseDeDeplacement;
    [SerializeField, Range(0.01f, .5f), Tooltip("Le temps que met le cube a parcourir la distance départ -> joueur")]
    private float dureeDeDeplacement;
    [SerializeField, Range(-180, 180), Tooltip("Le degré de rotation du cube lors de l'animation quand tu le manges.")]
    private float degresDeRotation;

    //======================================================================
    private List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }
    #endregion

    #region Unity_Functions
    #endregion

    #region Custom_Functions
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
                if (hit.transform.parent && hit.transform.parent.TryGetComponent(out cubeClone))
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

            StartCoroutine(Aspiration(cube, playerTransform));
        }
    }

    IEnumerator Aspiration(GameObject cube, Transform player)
    {
        Vector3 cubeStartPos = cube.transform.position;
        Vector3 cubeStartRot = cube.transform.rotation.eulerAngles;
        Vector3 cubeEndRot = cubeStartRot + new Vector3(0, 0, degresDeRotation);
        cube.transform.position -= Vector3.forward * 0.05f;
        float scaleFactor;
        float t = 0;
        //float distanceCubeStart_Player;
        //float distanceCubeCurrent_Player;
        //Vector2 direction;

        //while (!isInRadius(cube.transform.position, player.transform.position, .5f))
        while(t < 1f)
        {
            //direction = (player.transform.position - cube.transform.position).normalized;
            //cube.transform.position += (Vector3)direction * (Time.deltaTime * vitessDeDeplacement);

            //distanceCubeStart_Player = (player.transform.position - cubeStartPos).magnitude;
            //distanceCubeCurrent_Player = (player.transform.position - cube.transform.position).magnitude;
            //alpha = distanceCubeCurrent_Player / distanceCubeStart_Player;

            cube.transform.rotation = Quaternion.Euler(Vector3.Lerp(cubeStartRot, cubeEndRot, t));
            cube.transform.position = Vector3.Lerp(cubeStartPos, player.position, t);
            cube.transform.position = DOVirtual.EasedValue(cubeStartPos, player.position, t, Ease.InBack, 3f);
            scaleFactor = DOVirtual.EasedValue(2.857143f, 0, t, Ease.InBack, 2f);
            cube.transform.localScale = Vector3.one * scaleFactor;
            t += Time.deltaTime / dureeDeDeplacement;
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