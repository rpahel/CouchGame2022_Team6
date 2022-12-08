using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class Cube_TNT : CubeDestroyable {
    
    public TNT pattern;
    [Range(0, 100)] 
    public int damageEat;

    [HideInInspector] public bool startExplode;
    [SerializeField] private MeshRenderer rendererFire;
    [SerializeField] private MeshRenderer rendererFire2;
    private CinemachineImpulseSource shakeSource;

    // Un cube peut respawn sur la tnt 
    
    
    // On peut faire sauter la tnt*
    // Largeur degat rayons
    // Faire en sorte que les dégats qu'on inflige aux autre comptent pour nos propre damage a la fin (c'est pas le cas actuellement)
   // Fix la zone d'explosion à la position de la TNT (et non pas son endroit de spawn)
    
    private void Awake() {
        rendererFire.sortingOrder = 200;
        rendererFire2.sortingOrder = 200;
        shakeSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Explode(Transform colParent,PlayerManager source) => StartCoroutine(OnExplosion(colParent,source));
    
    private IEnumerator OnExplosion(Transform colParent,PlayerManager source) {
        startExplode = true;
        GameManager.Instance.AudioManager.Play("TNT_Trigger");
        yield return new WaitForSeconds(1f);
        shakeSource.GenerateImpulse(2f);
        GameManager.Instance.AudioManager.Play("TNT_Explode");
        foreach (Vector2 dir in pattern.pattern) {
            if (dir != Vector2.zero) {
                Vector3 direction = new Vector3(dir.x,dir.y,colParent.position.z);
                foreach (CubeDestroyable c in FindCubeInDirection(direction, FindObjectsOfType<CubeDestroyable>().ToList(), colParent.GetChild(0).gameObject)) {
                    StartCoroutine(c.Suck(c.transform.GetChild(0).gameObject, colParent));

                    levelGenerator.AddToRespawnList(c);
                    
                    if (c.gameObject != this.gameObject && c is Cube_TNT) 
                        ((Cube_TNT)c).Explode(c.transform,source);
                }

                RaycastHit2D hit = Physics2D.BoxCast(colParent.position,new Vector2(levelGenerator.Scale,levelGenerator.Scale),90,dir, Mathf.Infinity,1 << 3);

                if (hit.collider != null && hit.collider.TryGetComponent<PlayerManager>(out PlayerManager playerManager)) 
                    playerManager.OnDamage(source, damageEat, Vector3.zero);
            }
        }

        startExplode = false;
        levelGenerator.AddToRespawnList(colParent.gameObject.GetComponent<CubeDestroyable>());
    }
    
    private List<CubeDestroyable> FindCubeInDirection(Vector3 direction,List<CubeDestroyable> cubes,GameObject origin) {
        List<Vector3> allPositions = new List<Vector3>();

        for (int i = 0; i < 100; i++) 
            allPositions.Add(origin.transform.position + direction * i);

        List<CubeDestroyable> cubesInDir = new List<CubeDestroyable>();
        
        foreach (Cube cube in cubes) {
            foreach (Vector3 checkPos in allPositions) {
                if(cube.GetComponentInChildren<Collider2D>() != null && cube.GetComponentInChildren<Collider2D>().bounds.Contains(checkPos) && cube is CubeDestroyable)
                    cubesInDir.Add((CubeDestroyable)cube);
            }
        }

        return cubesInDir;
    }
    

    


}
