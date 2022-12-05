using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Edible : CubeDestroyable
{
    #region Variables
    //======================================================================
    [SerializeField, Range(0.01f, 2f), Tooltip("Le temps que met le cube à apparaitre après être spawné par projectile.")]
    private float apparitionDuration;
    [HideInInspector]
    public bool isOriginalCube;

    //======================================================================
    private List<Cube> cubesAutour = new List<Cube>(4);
    public List<Cube> CubesAutour { get => cubesAutour; set => cubesAutour = value; }
    #endregion

    #region Custom_Functions
    //======================================================================
    /// <summary>
    /// Regarde les cubes qu'il y a autour et les ajoute à la liste cubesAutour
    /// </summary>
    public void InitCubes(int maxWidth, int maxHeight)
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

        if (!playerTransform)
            cube.SetActive(false);

        if (playerTransform)
        {
            cube.transform.parent = null;
            cube.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(Suck(cube, playerTransform));
        }

        if (isOriginalCube)
            levelGenerator.AddToRespawnList(this);
    }

    

    /// <summary>
    /// Retire le reste collé au cube voisin qui vient de se faire manger.
    /// </summary>
    /// <param name="indexDuVoisin">L'index du voisin.</param>
    public void NeighbourGotEaten(int indexDuVoisin)
    {
        //if (isEaten)
        //{
        //    leftOvers[indexDuVoisin].SetActive(false);
        //}

        // Changer cette fonction pour update le sprite du cube en fonction des cubes autour
    }

    /// <summary>
    /// Refais apparaitre ce cube sur la carte.
    /// </summary>
    public void GetBarfed(Vector2 impactPos, Color color)
    {
        cube.transform.localScale = Vector3.one;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        cube.SetActive(true);
        isEaten = false;

        if(isOriginalCube)
            levelGenerator.RemoveFromRespawnList(this);

        StartCoroutine(BarfedAnimation(impactPos));
    }

    public void GetBarfed(Vector2 impactPos)
    {
        cube.transform.localScale = Vector3.one;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.SetActive(true);
        isEaten = false;

        if (isOriginalCube)
            levelGenerator.RemoveFromRespawnList(this);

        StartCoroutine(BarfedAnimation(impactPos));
    }

    IEnumerator BarfedAnimation(Vector2 impactPos)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Vector3 startPos = impactPos - (Vector2)transform.position;
        Vector3 endPos = Vector3.zero;
        float t = 0;

        while (t <= 1f)
        {
            IsInAnimation = true;
            cube.transform.localScale = DOVirtual.EasedValue(startScale, endScale, t, Ease.OutElastic, .5f);
            cube.transform.localPosition = DOVirtual.EasedValue(startPos, endPos, Mathf.InverseLerp(startScale.x, endScale.x, cube.transform.localScale.x), Ease.Linear);
            t += Time.fixedDeltaTime / apparitionDuration;
            yield return new WaitForFixedUpdate();
        }

        cube.transform.localScale = endScale;
        cube.transform.localPosition = endPos;

        IsInAnimation = false;
    }

    #endregion
}