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

    // Si création du cube et que joueur dans le cube ca le repousse vers le haut 

    private PlayerManager playerManager;
    public GameObject blockToPlace;

    private Rigidbody2D rb;

    public Vector3 direction;

    [Range(0,1)]
    public float percentageEat;

    private int colorIndex;
    
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerManager = GetComponent<PlayerManager>();
    }

    void Update() {
        
        if(rb.velocity != Vector2.zero)
            direction = rb.velocity.normalized;
        
        Debug.DrawLine(transform.position,direction,Color.red);

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
                maxDistance += 2f;
                spawnPosition = transform.position - direction * maxDistance;
            }

            GameObject instance = Instantiate(blockToPlace, transform.position - direction * (maxDistance), Quaternion.identity);

            List<GameObject> cubesList = GameObject.FindGameObjectsWithTag("CubeEdible").ToList();
            cubesList.Remove(instance);

            if (cubesList.Count > 0) {


                GameObject next = cubesList[0];
                
                BoxCollider2D cubeCollider = next.GetComponentInChildren<BoxCollider2D>();
                BoxCollider2D instanceCollider = instance.GetComponentInChildren<BoxCollider2D>();
                int i = 1;


                while (i < cubesList.Count && cubeCollider.bounds.Intersects(instanceCollider.bounds)) {
                    next = cubesList[i];
                    cubeCollider = cubesList[i].GetComponentInChildren<BoxCollider2D>();
                    i++;
                }

                Debug.Log("next " + next);


                //     next.GetComponentInChildren<MeshRenderer>().material.color = colorIndex % 2 == 0 ? Color.blue : Color.red;

                if (next != null) {
                    float3 scaleVec = instance.transform.localScale;
                    float3 signDir = sign(direction);

                    scaleVec.xyz *= signDir.xyz;
                    instance.transform.position = next.transform.position - (Vector3)scaleVec;
                }
                
            }


            //playerManager.eatAmount -= playerManager.eatAmount * percentageEat;
            //Mathf.Clamp(playerManager.eatAmount, 0, playerManager.maxEatValue);
        }
    }

}
