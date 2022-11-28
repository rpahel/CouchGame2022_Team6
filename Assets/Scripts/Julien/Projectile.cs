using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,_explosionRadius);

        foreach (Collider2D collider in hits) {
            switch (collider.tag) {
                case "CubeEdible":
                    Cube_Edible cube;
                    if (collider.transform.parent.TryGetComponent<Cube_Edible>(out cube)) {
                        if (!cube.IsManged) {
                            cube.OnExploded();
                        }
                    }
                    break;
                
                case "Player":
                    PlayerMovement mov;
                    if (collider.gameObject != _playerGo && collider.gameObject.TryGetComponent<PlayerMovement>(out mov) && !_alreadyHitPlayer)
                    {
                        _alreadyHitPlayer = true;
                        Debug.Log(collider.gameObject.name);
                        PlayerManager playerManager = mov.GetComponent<PlayerManager>();
                        Rigidbody2D rb = mov.GetComponent<Rigidbody2D>();
                        playerManager.eatAmount -= _shootImpactSatietyPercent / 100;
                        rb.AddForce(-col.contacts[0].normal * _shootForce,ForceMode2D.Impulse);
                    }
                    break;
            }
        }
        else if(col.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement mov)) {
            _alreadyHitPlayer = true;
            PlayerManager playerManager = mov.GetComponent<PlayerManager>();
            Rigidbody2D rb = mov.GetComponent<Rigidbody2D>();
            playerManager.eatAmount -= _shootImpactSatietyPercent / 100;
            rb.AddForce(-col.contacts[0].normal * _shootBounce,ForceMode2D.Impulse);
        }
        
        if (col.gameObject.transform.parent.gameObject.TryGetComponent<Cube_TNT>(out Cube_TNT tnt)) 
            tnt.Explode(col.gameObject.transform.parent);
        
        Destroy(transform.gameObject);
    }

    public void InitializeValue(float impactSatietyPercent, float force,float cubeBounce, GameObject player) {
        _shootImpactSatietyPercent = impactSatietyPercent;
        _shootBounce = force;
        _cubeBounce = cubeBounce;
        _playerGo = player;
    }
}
