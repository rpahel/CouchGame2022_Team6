using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Edible : CubeDestroyable
{
    #region Variables
    //======================================================================
    
    

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


    #endregion
}