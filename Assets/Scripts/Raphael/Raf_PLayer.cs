using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf_PLayer : MonoBehaviour
{
    private CharacterController chaC;
    public Transform pointeur;

    private void Awake()
    {
        chaC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        chaC.SimpleMove(Vector3.right * Input.GetAxis("Horizontal") * 10f);
        pointeur.Rotate(new Vector3(0, 0, Input.GetAxis("Vertical")));

    }
}
