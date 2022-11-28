using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTriggerScript : MonoBehaviour
{
    [SerializeField] private PlayerManager PManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PManager)
            throw new System.Exception("PlayerManager n'est pas référencé dans le SpecialTriggerScript");

        if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible"))
            collision.transform.parent.GetComponent<Cube_Edible>().GetEaten();
    }
}
