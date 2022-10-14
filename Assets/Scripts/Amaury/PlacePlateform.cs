using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacePlateform : MonoBehaviour {

    // Si création du cube et que joueur dans le cube ca le repousse vers le haut 

    public GameObject blockToPlace;

    private Rigidbody2D rb;

    public float range;

    public Vector3 direction;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(rb.velocity != Vector2.zero)
            direction = rb.velocity.normalized;
    }

    public void OnPlace() {
        Debug.Log("vel " + rb.velocity);
        Vector3 direction = rb.velocity.normalized;
        Debug.Log("normalized " + rb.velocity.normalized);
        if (direction.x != 0 && direction.y != 0)
            direction = new Vector3(0f, -direction.y, direction.z);
        
        Debug.Log("my direction " + direction);
        
        Instantiate(blockToPlace, transform.position - direction * range, Quaternion.identity);
    }
    
}
