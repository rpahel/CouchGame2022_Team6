using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Mathematics.math;

public class Projectile : MonoBehaviour {
    
    
    private GameObject _playerGo;
    private float _shootImpactSatietyPercent;
    private float _shootBounce;
    private float _cubeBounce;
    private bool _alreadyHitPlayer;
    
    private void OnCollisionEnter2D(Collision2D col) {
        
        if (col.gameObject.transform.parent.TryGetComponent<Cube>(out Cube cube)) {
            
          /*  GameObject block = LevelGenerator.Instance.cubeEdible;
            Vector3 offset = (Vector2)sign(col.contacts[0].normal) * block.transform.localScale;
            
            Debug.Log("normal " + col.contacts[0].normal);
            Debug.Log("offset " + offset);
            
            Instantiate(block,col.gameObject.transform.parent.position + offset,Quaternion.identity);
            */
        }
        else if(col.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement mov)) {
            _alreadyHitPlayer = true;
            PlayerManager playerManager = mov.GetComponent<PlayerManager>();
            Rigidbody2D rb = mov.GetComponent<Rigidbody2D>();
            playerManager.eatAmount -= _shootImpactSatietyPercent / 100;
            rb.AddForce(-col.contacts[0].normal * _shootBounce,ForceMode2D.Impulse);
        }
        
        
        if (col.gameObject.transform.parent.gameObject.TryGetComponent<TNT>(out TNT tnt))  {

            RaycastHit2D[] hits = Physics2D.RaycastAll(col.gameObject.transform.position,new Vector2(0,1),1000, 1 << 8);//1 >> 8

            Debug.Log("size " + hits.Length);
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider != null) {
                    if (hit.collider.gameObject.layer != 8 && hit.collider.gameObject.TryGetComponent<Cube_Edible>(out Cube_Edible c)) {
                        //c.GetManged();
                    }
                        
                }
            }
        }
        
        Destroy(transform.gameObject);
    }

    public void InitializeValue(float impactSatietyPercent, float force,float cubeBounce, GameObject player) {
        _shootImpactSatietyPercent = impactSatietyPercent;
        _shootBounce = force;
        _cubeBounce = cubeBounce;
        _playerGo = player;
    }
}
