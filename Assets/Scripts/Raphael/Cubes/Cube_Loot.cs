using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Loot : MonoBehaviour
{
    #region Variables
    //========================================================
    public float TestEat = .1f;
    #endregion

    #region Unity_Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager_JULIEN>().eatAmount += TestEat;
            Destroy(gameObject);
        }
    }
    #endregion
}
