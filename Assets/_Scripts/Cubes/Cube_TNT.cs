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

    [SerializeField] private MeshRenderer rendererFire;
    [SerializeField] private MeshRenderer rendererFire2;
    private CinemachineImpulseSource shakeSource;

    private void Awake()
    {
        rendererFire.sortingOrder = 200;
        rendererFire2.sortingOrder = 200;
        shakeSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Explode(Transform colParent) => StartCoroutine(OnExplosion(colParent));
    
    private IEnumerator OnExplosion(Transform colParent) {
        GameManager.Instance.AudioManager.Play("TNT_Trigger");
        yield return new WaitForSeconds(1f);
        shakeSource.GenerateImpulse(2f);
        GameManager.Instance.AudioManager.Play("TNT_Explode");
        foreach (Vector2 dir in pattern.pattern) {
            if (dir != Vector2.zero) {
                Vector3 direction = new Vector3(dir.x,dir.y,colParent.position.z);
                foreach (CubeDestroyable c in FindCubeInDirection(direction, FindObjectsOfType<CubeDestroyable>().ToList(), colParent.gameObject)) {
                    StartCoroutine(c.Suck(c.transform.GetChild(0).gameObject, colParent));

                    levelGenerator.AddToRespawnList(c);
                    
                    if (c.gameObject != this.gameObject && c is Cube_TNT) 
                        ((Cube_TNT)c).Explode(c.transform);
                }

                RaycastHit2D hit = Physics2D.Raycast(colParent.position, direction, 1000, 1 << 3);

                if (hit.collider != null && hit.collider.TryGetComponent<PlayerManager>(out PlayerManager playerManager)) {
                    playerManager.fullness -= damageEat;
                    playerManager.UpdatePlayerScale();
                }
            }
        }
        
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
