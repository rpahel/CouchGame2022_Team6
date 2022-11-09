using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Cube_TNT : Cube {
    
    public Vector2[] pattern;

    public void Explode(Transform colParent) {
        foreach (Vector2 dir in pattern) {
            if (dir != Vector2.zero) {
                Vector3 direction = new Vector3(dir.x,dir.y,colParent.position.z);

                foreach (Cube c in FindCubeInDirection(direction, FindObjectsOfType<Cube>().ToList(), colParent.gameObject)) {
                    if(c is Cube_Edible)
                        ((Cube_Edible)c).OnExploded();
                    if(c is Cube_TNT)
                        ((Cube_TNT)c).Explode(c.transform.parent);
                }
            }
        }
    }
    
    private List<Cube> FindCubeInDirection(Vector3 direction,List<Cube> cubes,GameObject origin) {
        List<Vector3> allPositions = new List<Vector3>();

        for (int i = 0; i < 100; i++) 
            allPositions.Add(origin.transform.position + direction * i);

        List<Cube> cubesInDir = new List<Cube>();

        foreach (Cube cube in cubes) {
            if(allPositions.Contains(cube.transform.position) && (cube is Cube_Edible || cube is Cube_TNT))
                cubesInDir.Add(cube);
        }

        return cubesInDir;
    }
}
