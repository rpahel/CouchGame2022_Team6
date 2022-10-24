using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlacePlateform : MonoBehaviour {

    // Si création du cube et que joueur dans le cube ca le repousse vers le haut 

    private PlayerManager playerManager;
    public GameObject blockToPlace;

    private Rigidbody2D rb;

    public Vector3 direction;

    [Range(0,1)]
    public float percentageEat;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerManager = GetComponent<PlayerManager>();
    }

    void Update() {
        
        if(rb.velocity != Vector2.zero)
            direction = rb.velocity.normalized;

    }

    public void OnPlace() {
        
        Debug.Log("value01 " + playerManager.eatAmount);
        Debug.Log("value02 " + (playerManager.maxEatValue - (playerManager.maxEatValue - playerManager.maxEatValue * percentageEat)));
        
        if (playerManager.eatAmount >= playerManager.maxEatValue - (playerManager.maxEatValue - playerManager.maxEatValue * percentageEat)) {
            float maxDistance = 1f;

            if (direction.x != 0 && direction.y != 0) {
                direction = new Vector3(direction.x, 0f, 0f);
                maxDistance = 1.5f;
            }

            direction.y = Mathf.Abs(direction.y);

            Vector3 end = transform.position - (Vector3)(direction * (transform.GetComponent<BoxCollider2D>().size / 2));

            Vector3 spawnPosition = transform.position - direction * maxDistance;


            while (Vector3.Distance(transform.position, end) > Vector3.Distance(transform.position, spawnPosition)) {
                maxDistance++;
                spawnPosition = transform.position - direction * maxDistance;
            }

            Instantiate(blockToPlace, transform.position - direction * (maxDistance + 1.2f), Quaternion.identity);
            playerManager.eatAmount -= playerManager.eatAmount * percentageEat;
            Mathf.Clamp(playerManager.eatAmount, 0, playerManager.maxEatValue);
        }
    }

}
