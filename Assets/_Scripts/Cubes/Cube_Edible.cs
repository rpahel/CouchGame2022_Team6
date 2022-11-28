using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Edible : Cube
{
    #region Variables
    //======================================================================
    [SerializeField, Tooltip("Les restes en enfant ce gameObject.")]
    private GameObject[] leftOvers = new GameObject[4];
    [SerializeField, Range(0.01f, .5f), Tooltip("Le temps que met le cube a parcourir la distance départ -> joueur")]
    private float moveDuration;
    [SerializeField, Range(-180, 180), Tooltip("Le degré de rotation du cube lors de l'animation quand tu le manges.")]
    private float rotationDegrees;
    [SerializeField, Range(0.01f, 2f), Tooltip("Le temps que met le cube à apparaitre après être spawné par projectile.")]
    private float apparitionDuration;

    //======================================================================
    private List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }
    #endregion

    #region Custom_Functions
    //======================================================================
    /// <summary>
    /// Regarde les cubes qu'il y a autour et les ajoute à la liste cubesAutour
    /// </summary>
    public void InitCubes(int width, int height, int maxWidth, int maxHeight)
    {
        Vector2Int dir = Vector2Int.zero;
        Vector2Int targetPos;
        Transform targetCubeTransform;

        for (int i = 0; i < 4; i++)
        {
            cubesAutour.Add(null);

            switch (i) // Je peux pas utiliser les Quaternions sur des Vector2Int
            {
                case 0:
                    dir = Vector2Int.up;
                    break;

                case 1:
                    dir = Vector2Int.right;
                    break;

                case 2:
                    dir = Vector2Int.down;
                    break;

                case 3:
                    dir = Vector2Int.left;
                    break;
            }

            targetPos = unscaledPosition + dir;

            // Merci à QUENTIN d'avoir trouvé le bug qui m'a forcé à faire cette vérif
            if (targetPos.x < 0 || targetPos.x >= maxWidth || targetPos.y < 0 || targetPos.y >= maxHeight)
            {
                continue;
            }

            targetCubeTransform = GameManager.Instance.LevelGenerator.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];
            cubesAutour[i] = targetCubeTransform.GetComponent<Cube>();
        }

        if (!gameObject.activeSelf)
        {
            GetEaten(null, false);
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Fais disparaitre le mesh du cube et le remplace par des meshs de miettes en fonction des cubes autour.
    /// </summary>
    /// <param name="playerTransform">(facultatif) Le transform du joueur qui mange le cube.</param>
    public void GetEaten(Transform playerTransform = null, bool showRestes = true)
    {
        isEaten = true;

        // Signale aux cubes voisins que ce cube s'est fait mangé
        for (int i = 0; i < 4; i++)
        {
            if (cubesAutour[i] && cubesAutour[i].CubeType == CUBE_TYPE.EDIBLE)
            {
                (cubesAutour[i] as Cube_Edible).NeighbourGotEaten((i + 2) % 4);
            }
        }

        if(!playerTransform)
            cube.SetActive(false);

        // Fais apparaitre les restes en fonction des cubes voisins qui sont toujours là
        if (showRestes)
        {
            for (int i = 0; i < 4; i++)
            {
                if (cubesAutour[i] && !cubesAutour[i].IsEaten)
                {
                    leftOvers[i].SetActive(true);
                }
            }
        }
        
        if(playerTransform)
        {
            cube.transform.parent = null;
            cube.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(Suck(cube, playerTransform));
        }
    }

    IEnumerator Suck(GameObject cube, Transform player)
    {
        Vector3 cubeStartPos = cube.transform.position;
        Vector3 cubeStartRot = cube.transform.rotation.eulerAngles;
        Vector3 cubeEndRot = cubeStartRot + new Vector3(0, 0, rotationDegrees);
        cube.transform.position -= Vector3.forward * 0.05f;
        float scaleFactor;
        float t = 0;

        while(t < 1f)
        {
            cube.transform.rotation = Quaternion.Euler(Vector3.Lerp(cubeStartRot, cubeEndRot, t));
            cube.transform.position = Vector3.Lerp(cubeStartPos, player.position, t);
            cube.transform.position = DOVirtual.EasedValue(cubeStartPos, player.position, t, Ease.InBack, 3f);
            scaleFactor = DOVirtual.EasedValue(2.857143f, 0, t, Ease.InBack, 2f);
            cube.transform.localScale = Vector3.one * scaleFactor;
            t += Time.deltaTime / moveDuration;
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
    public void NeighbourGotEaten(int indexDuVoisin)
    {
        if (isEaten)
        {
            leftOvers[indexDuVoisin].SetActive(false);
        }
    }

    /// <summary>
    /// Refais apparaitre ce cube sur la carte.
    /// </summary>
    public void GetBarfed(Vector2 impactPos)
    {
        foreach(GameObject reste in leftOvers)
        {
            reste.SetActive(false);
        }

        cube.transform.localScale = Vector3.one;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.SetActive(true);
        isEaten = false;

        StartCoroutine(BarfedAnimation(impactPos));
    }

    IEnumerator BarfedAnimation(Vector2 impactPos)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Vector3 startPos = impactPos - (Vector2)transform.position;
        Vector3 endPos = Vector3.zero;
        float t = 0;

        while(t <= 1f)
        {
            cube.transform.localScale = DOVirtual.EasedValue(startScale, endScale, t, Ease.OutElastic, .5f);
            cube.transform.localPosition = DOVirtual.EasedValue(startPos, endPos, Mathf.InverseLerp(startScale.x, endScale.x, cube.transform.localScale.x), Ease.Linear);
            t += Time.fixedDeltaTime / apparitionDuration;
            yield return new WaitForFixedUpdate();
        }

        cube.transform.localScale = endScale;
        cube.transform.localPosition = endPos;
    }

    #endregion
}