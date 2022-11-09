using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class Cube_TNT : CubeDestroyable {
    
    public TNT pattern;

    public void Explode(Transform colParent) {
        foreach (Vector2 dir in pattern.pattern) {
            if (dir != Vector2.zero) {
                Vector3 direction = new Vector3(dir.x,dir.y,colParent.position.z);

                foreach (CubeDestroyable c in FindCubeInDirection(direction, FindObjectsOfType<CubeDestroyable>().ToList(), colParent.gameObject)) { 
                    c.OnExploded();
                    
                    if (c.gameObject != this.gameObject && c is Cube_TNT)
                      ((Cube_TNT)c).Explode(c.transform);
                }
            }
        }
    }
    
    private List<CubeDestroyable> FindCubeInDirection(Vector3 direction,List<CubeDestroyable> cubes,GameObject origin) {
        List<Vector3> allPositions = new List<Vector3>();

        for (int i = 0; i < 100; i++) 
            allPositions.Add(origin.transform.position + direction * i);

        List<CubeDestroyable> cubesInDir = new List<CubeDestroyable>();

        foreach (Cube cube in cubes) {
            if(allPositions.Contains(cube.transform.position) && cube is CubeDestroyable)
                cubesInDir.Add((CubeDestroyable)cube);
        }

        return cubesInDir;
    }
    


}
