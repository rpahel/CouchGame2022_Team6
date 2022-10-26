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
    public GameObject blockToPlace;

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
        if (playerManager.eatAmount >= playerManager.maxEatValue - (playerManager.maxEatValue - playerManager.maxEatValue * percentageEat)) {
            float maxDistance = 0.2f;

            if (direction.x != 0 && direction.y != 0) {
                direction = new Vector3(direction.x, 0f, 0f);
                maxDistance = 1.5f;
            }
            
            direction.y = Mathf.Abs(direction.y);

            Vector3 end = transform.position - (Vector3)(direction * (transform.GetComponent<BoxCollider2D>().size / 2));
            Vector3 spawnPosition = transform.position - direction * maxDistance;

            
            while (Vector3.Distance(transform.position, spawnPosition) <= Vector3.Distance(transform.position, end)) {
                maxDistance += 2.2f;
                spawnPosition = transform.position - direction * maxDistance;
            }

            GameObject instance = Instantiate(blockToPlace, transform.position - direction * (maxDistance), Quaternion.identity);

            List<GameObject> cubes = GameObject.FindGameObjectsWithTag("CubeEdible").ToList();
            cubes.Remove(instance);
            
            
            foreach (GameObject cube in cubes) {
                BoxCollider2D cubeCollider = cube.GetComponentInChildren<BoxCollider2D>();
                
                if (cubeCollider != null &&cubeCollider.bounds.Intersects(instance.GetComponentInChildren<BoxCollider2D>().bounds)) {
                    instance.transform.position = cube.transform.position - ((Vector3) (instance.transform.localScale * -sign(direction)));
                    break;
                }
            }

            playerManager.eatAmount -= playerManager.eatAmount * percentageEat;
            Mathf.Clamp(playerManager.eatAmount, 0, playerManager.maxEatValue);
        }
    }

 

}
