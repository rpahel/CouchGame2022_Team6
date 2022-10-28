using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_Loot : MonoBehaviour
{
    public float TestEat = .1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().eatAmount += TestEat;
            Destroy(gameObject);
        }
    }
}
