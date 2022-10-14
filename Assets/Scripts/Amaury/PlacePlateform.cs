using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacePlateform : MonoBehaviour {

    // Si création du cube et que joueur dans le cube ca le repousse vers le haut 

    public GameObject blockToPlace;
    private Vector3 direction;

    private Rigidbody2D rb;

    public float range;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update() {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        
        transform.Translate(movement * Time.deltaTime * 10);
    }

    public void OnPlace(InputAction.CallbackContext e) {
        if (e.performed) {
            Debug.Log("direction " + direction );
            Instantiate(blockToPlace, transform.position - direction * range, Quaternion.identity);
        }
    }

    public void OnMove(InputAction.CallbackContext e) {
        if(e.performed)
            direction = e.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext e)
    {
        if (e.performed) {
            rb.AddForce(Vector3.up * 15, ForceMode2D.Impulse);
            direction = new Vector3(0, 1, 0);
        }
    }
}
