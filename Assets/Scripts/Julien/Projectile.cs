using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float explosionRadius;
    private float shootImpactSatietyPercent;
    private float shootForce;
    private void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,explosionRadius);

        foreach (Collider2D collider in hits) {
            switch (collider.tag) {
                case "CubeEdible":
                    Cube_Edible cube;
                    if (collider.transform.parent.TryGetComponent<Cube_Edible>(out cube)) {
                        if (!cube.isManged()) {
                            cube.OnExploded();
                        }
                    }
                    break;
                
                case "Player":
                    Movement mov;
                    if (collider.gameObject.TryGetComponent<Movement>(out mov))
                    {
                        PlayerManager_JULIEN playerManager = mov.GetComponent<PlayerManager_JULIEN>();
                        Rigidbody2D rb = mov.GetComponent<Rigidbody2D>();
                        playerManager.eatAmount -= playerManager.eatAmount * (shootImpactSatietyPercent / 100);
                        rb.AddForce(-col.contacts[0].normal * shootForce,ForceMode2D.Impulse);
                    }
                    break;
            }
        }
        
        Destroy(transform.gameObject);
    }

    public void InitializeValue(float explosion, float impactSatietyPercent, float force)
    {
        explosionRadius = explosion;
        shootImpactSatietyPercent = impactSatietyPercent;
        shootForce = force;

    }
}
