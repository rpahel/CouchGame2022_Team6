using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public bool drawSquare;
    public bool drawCustomShape;

    public AnimationCurve curve;
    
    void Start()
    {
        Debug.Log("size " + filter.mesh.vertices.Length);

        

    }

    public void DrawCustomShape() {
        Mesh originalMesh = filter.sharedMesh;

        Mesh mesh = new Mesh();
        mesh.name = "MYOBJECT";
        mesh.vertices = new Vector3[] {Vector3.zero,Vector3.right,Vector3.up,Vector3.up + Vector3.right};
        mesh.triangles = new int[] {
            0,1,2   ,2,1,3, 
        };

        mesh.normals = originalMesh.normals;
        
        filter.sharedMesh = mesh;
    }
    
    
    public void DrawSquare() {
        Mesh originalMesh = filter.sharedMesh;

        Mesh mesh = new Mesh();
        mesh.name = "MYOBJECT";
        mesh.vertices = new Vector3[] {Vector3.zero,Vector3.right,Vector3.up,Vector3.up + Vector3.right,Vector3.up + Vector3.back,Vector3.forward * -1
            ,Vector3.forward * -1 + Vector3.right,Vector3.forward * -1 + Vector3.right + Vector3.up};
        mesh.triangles = new int[] {
            0,1,2   ,2,1,3,   0,2,4,    0,4,5,   4,6,5, 6,4,7, 1,6,3, 3,6,7, 4,2,7, 7,2,3, 0,5,6, 0,6,1 
        };

        mesh.normals = originalMesh.normals;
        
        filter.sharedMesh = mesh;
    }

    private void OnDrawGizmos() {
        if (drawSquare) {
            int verticeIndex = 0;

            Color[] vertexColor = new Color[] { Color.red,Color.blue,Color.cyan,Color.yellow,Color.magenta,Color.gray,Color.white,Color.green};
        
            foreach (Vector3 vertex in filter.sharedMesh.vertices) {
                //  Debug.Log("vertex " + vertex);
          
                Gizmos.color = vertexColor[verticeIndex];
                Gizmos.DrawSphere(filter.transform.position + vertex, 0.05f);

                verticeIndex++;

            }

            Vector3[] vertices = filter.sharedMesh.vertices;
            int[] triangles = filter.sharedMesh.triangles;
            
            GUI.color = Color.cyan;

            Handles.Label(filter.transform.position + vertices[0] + new Vector3(-0.01f,0.1f,0), "" +triangles[0] + " Vector3.Zero");
            Handles.Label(filter.transform.position + vertices[1] + new Vector3(-0.01f,0.1f,0), "" +triangles[1] + " Vector3.Right");
            Handles.Label(filter.transform.position + vertices[2] + new Vector3(-0.01f,0.1f,0), "" +triangles[2] + " Vector3.Up");
            Handles.Label(filter.transform.position + vertices[3] + new Vector3(-0.01f,0.1f,0), "" +triangles[5] + " Vector3.Up + Vector3.Right");
        
            Handles.Label(filter.transform.position + vertices[4] + new Vector3(-0.01f,0.1f,0), "" +triangles[8] + " Vector3.Up + Vector3.Back" );
            Handles.Label(filter.transform.position + vertices[5] + new Vector3(-0.01f,0.1f,0), "" +triangles[11] + " Vector3.Back");
            Handles.Label(filter.transform.position + vertices[6] + new Vector3(-0.01f,0.1f,0), "" +triangles[13] + " Vector3.Back + Vector3.Right");
            Handles.Label(filter.transform.position + vertices[7] + new Vector3(-0.01f,0.1f,0), "" +triangles[17] + " Vector3.Back + Vector3.Right + Vector3.Up");

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
