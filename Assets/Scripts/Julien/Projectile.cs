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
        
        
        if (col.gameObject.transform.parent.gameObject.TryGetComponent<TNT>(out TNT tnt)) {
            foreach (Vector2 dir in tnt.pattern) {
                if (dir != Vector2.zero) {
                    Vector3 direction = new Vector3(dir.x,dir.y,col.gameObject.transform.parent.position.z);

                    foreach (Cube_Edible c in FindCubeInDirection(direction, FindObjectsOfType<Cube>().ToList(), col.gameObject.transform.parent.gameObject))
                        c.OnExploded();
                    
                    col.gameObject.transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
                }
            }
        }
        
        Destroy(transform.gameObject);
    }

    private List<Cube_Edible> FindCubeInDirection(Vector3 direction,List<Cube> cubes,GameObject origin) {
        List<Vector3> allPositions = new List<Vector3>();

        for (int i = 0; i < 100; i++) 
            allPositions.Add(origin.transform.position + direction * i);

        List<Cube_Edible> cubesInDir = new List<Cube_Edible>();

        foreach (Cube cube in cubes) {
            if(allPositions.Contains(cube.transform.position) && cube is Cube_Edible)
                cubesInDir.Add((Cube_Edible)cube);
        }

        return cubesInDir;
        // Trouver tt les cubes qui ont leur position dans allPositions
    }

    public void InitializeValue(float impactSatietyPercent, float force,float cubeBounce, GameObject player) {
        _shootImpactSatietyPercent = impactSatietyPercent;
        _shootBounce = force;
        _cubeBounce = cubeBounce;
        _playerGo = player;
    }
}
