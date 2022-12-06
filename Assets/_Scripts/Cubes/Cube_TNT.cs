using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cube_TNT : CubeDestroyable {
    
    public TNT pattern;
    [Range(0, 100)] 
    public int damageEat;

    public void Explode(Transform colParent) => StartCoroutine(OnExplosion(colParent));
    
    private IEnumerator OnExplosion(Transform colParent) {
        yield return new WaitForSeconds(1f);
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

                if (hit.collider != null)
                    hit.collider.GetComponent<PlayerManager>().fullness -= damageEat;

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
