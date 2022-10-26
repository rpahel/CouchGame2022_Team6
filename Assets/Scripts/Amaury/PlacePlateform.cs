using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using static Unity.Mathematics.math;


public class PlacePlateform : MonoBehaviour {

    private Rigidbody2D rb;
    private PlayerManager playerManager;
    private BoxCollider2D collider;
    public GameObject blockToPlace;

    public Vector3 direction;

    [Range(0,1)]
    public float percentageEat;

    private GameObject instance;

    public bool canOverpassPlacement;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerManager = GetComponent<PlayerManager>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        
        if(rb.velocity != Vector2.zero)
            direction = rb.velocity.normalized;
        
    }

    public void OnPlace() {
        if (playerManager.eatAmount >= playerManager.maxEatValue - (playerManager.maxEatValue - playerManager.maxEatValue * percentageEat)) {
            float maxDistance = 0.2f;

            if (direction.x != 0 && direction.y != 0) {
                direction = new Vector3(0f, direction.y, 0f);
                maxDistance = 3f;
                Debug.Log("jump and move");
            }

        
            direction.y = Mathf.Abs(direction.y);

            Vector3 end = transform.position - (Vector3)(direction * (transform.GetComponent<BoxCollider2D>().size / 2));
            Vector3 spawnPosition = transform.position - direction * maxDistance;

            
            while (Vector3.Distance(transform.position, spawnPosition) <= Vector3.Distance(transform.position, end)) {
                maxDistance += 2.2f;
                spawnPosition = transform.position - direction * maxDistance;
            }

            instance = Instantiate(blockToPlace, transform.position - direction * (maxDistance), Quaternion.identity);

            List<GameObject> cubes = GameObject.FindGameObjectsWithTag("CubeEdible").ToList();
            cubes.Remove(instance);
            
            
            foreach (GameObject cube in cubes) {
                BoxCollider2D cubeCollider = cube.GetComponentInChildren<BoxCollider2D>();
                
                if (cubeCollider != null &&cubeCollider.bounds.Intersects(instance.GetComponentInChildren<BoxCollider2D>().bounds)) {
                    if(canOverpassPlacement)
                        instance.transform.position = cube.transform.position - ((Vector3) (instance.transform.localScale * -sign(direction)));
                    else
                        Destroy(instance);

                    break;
                }
            }
            
            Debug.Log("direction " + (Vector3)sign(direction));
            
            playerManager.eatAmount -= playerManager.eatAmount * percentageEat;
            Mathf.Clamp(playerManager.eatAmount, 0, playerManager.maxEatValue);
        }
    }
}
