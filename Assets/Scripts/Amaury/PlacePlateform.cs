using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private int colorIndex;
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
                maxDistance += 2f;
                spawnPosition = transform.position - direction * maxDistance;
            }

            GameObject instance = Instantiate(blockToPlace, transform.position - direction * (maxDistance), Quaternion.identity);

           // Collider2D[] colliders = Physics2D.OverlapCircleAll(instance.transform.position, (instance.transform.localScale / 2).magnitude);
 
           // A Partir de la et a partir de la position du joueur il faut check le dernier pris 

           RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 5, 1 >> 6);
           float distance = 5;
           
           while (hit.collider != null && hit.collider.gameObject.tag.Equals("CubeEdible"))
           {
               distance++;
               hit = Physics2D.Raycast(transform.position, direction, distance, 1 >> 6);
           }
    
            if(hit.collider != null)
                Debug.Log("finalhit " + hit.collider.gameObject + " " + hit.collider.gameObject.transform.position);

           //Debug.Log("length " + colliders.Length);
           /*if (colliders.Length >= 2) {
               Transform farest = null;
               foreach (Collider2D col in colliders) {
                   if (col.gameObject.tag.Equals("CubeEdible")) {

                       if (farest == null)
                           farest = col.gameObject.transform;
                       else {
                           if (Vector2.Distance(transform.position, farest.position) <
                               Vector2.Distance(transform.position, col.gameObject.transform.position)) {
                               farest = col.gameObject.transform;
                               farest.GetComponent<MeshRenderer>().material.color = colorIndex % 2 == 0 ? Color.red : Color.blue;
                               colorIndex++;
                           }
                       }
                   }
               }
               
               Vector3 sign = new Vector3(direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0,direction.y > 0 ? 1 : direction.y < 0 ? -1 : 0,0f);
               Vector3 scaleOffset = farest.parent.localScale;
               scaleOffset.x *= sign.x;
               scaleOffset.y *= sign.y;

               scaleOffset.z = 0;
               

               Vector3 posOffset = farest.position - scaleOffset;
               Debug.Log("posOffset " + posOffset);
               
               instance.transform.position = posOffset;
           }
*/
           //playerManager.eatAmount -= playerManager.eatAmount * percentageEat;
           //Mathf.Clamp(playerManager.eatAmount, 0, playerManager.maxEatValue);
        }
    }

}
