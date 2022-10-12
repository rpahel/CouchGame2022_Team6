using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class Absorption : MonoBehaviour
{

    private Vector2 _inputVector,aimingDir;

    public Transform pointeur;
    private float angle;

    public MeshFilter filter;
    
    void Start()
    {
        Debug.Log("size " + filter.mesh.vertices.Length);

        Vector3[] verticesCustom = new Vector3[25];
        int i = 0;

        Mesh originalMesh = filter.mesh;

        foreach (Vector3 vertice in originalMesh.vertices)
        {
            verticesCustom[i] = vertice;
            i++;
        }

        verticesCustom[24] = new Vector3(0.5f, 0.9f, 0.1f);
        
        Mesh mesh = new Mesh();
        mesh.name = "MYOBJECT";
        mesh.vertices = new Vector3[] {Vector3.zero,Vector3.right,Vector3.up,Vector3.up + Vector3.right,Vector3.up + Vector3.back,Vector3.forward * -1};
        mesh.triangles = new int[] {
            0,1,2   ,2,1,3,   0,2,4,    0,4,5
        };
        
        
        filter.sharedMesh = mesh;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 vertex in filter.sharedMesh.vertices)
        {
          //  Debug.Log("vertex " + vertex);
            Gizmos.DrawSphere(filter.transform.position + vertex, 0.05f);
            
        }
      
       
    }

    void Update()
    {
        float directionX = Input.GetAxis("Horizontal") * 2 * Time.deltaTime;
        transform.Translate(directionX,0f,0f);
        
        aimingDir = _inputVector;
        angle = Mathf.Atan2(aimingDir.y, aimingDir.x);
        
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);

    }

    public void OnAim(InputAction.CallbackContext e)
    {
        _inputVector = e.ReadValue<Vector2>();
        pointeur.gameObject.SetActive(_inputVector.sqrMagnitude > 0.1f ? true : false);
        angle = Mathf.Atan2(_inputVector.y, _inputVector.x);
    }

    public void OnEat(InputAction.CallbackContext e)
    {
        
    }
}
