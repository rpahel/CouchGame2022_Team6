using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject _playerGo;
    private float _explosionRadius;
    private float _shootImpactSatietyPercent;
    private float _shootForce;
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
        
        Destroy(transform.gameObject);
    }

    public void InitializeValue(float explosion, float impactSatietyPercent, float force, GameObject player)
    {
        _explosionRadius = explosion;
        _shootImpactSatietyPercent = impactSatietyPercent;
        _shootForce = force;
        _playerGo = player;
    }
}
