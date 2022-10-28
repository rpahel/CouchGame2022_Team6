using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class Projectile : MonoBehaviour {
    
    
    private GameObject _playerGo;
    private float _shootImpactSatietyPercent;
    private float _shootBounce;
    private float _cubeBounce;
    private bool _alreadyHitPlayer;
    
    private void OnCollisionEnter2D(Collision2D col) {

        Cube cube = null;
        PlayerMovement mov = null;

        if (col.gameObject.transform.parent.TryGetComponent<Cube>(out cube)) {
            
            GameObject block = LevelGenerator.Instance.cubeEdible;
            Vector3 offset = (Vector2)sign(col.contacts[0].normal) * block.transform.localScale;
            
            Debug.Log("normal " + col.contacts[0].normal);
            Debug.Log("offset " + offset);
            
            Instantiate(block,col.gameObject.transform.parent.position + offset,Quaternion.identity);
        }
        else if(col.gameObject.TryGetComponent<PlayerMovement>(out mov)) {
            _alreadyHitPlayer = true;
            PlayerManager playerManager = mov.GetComponent<PlayerManager>();
            Rigidbody2D rb = mov.GetComponent<Rigidbody2D>();
            playerManager.eatAmount -= _shootImpactSatietyPercent / 100;
            rb.AddForce(-col.contacts[0].normal * _shootBounce,ForceMode2D.Impulse);
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
