using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Raf_PLayer : MonoBehaviour
{
    private CharacterController chaC;
    public Transform pointeur;
    public Transform pointeurBase;
    public float reach = 1f;
    private Vector2 mousePos2;
    private Vector2 mousePos3;
    private Vector2 playerToMouse;
    private float angle;

    private void Awake()
    {
        chaC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        mousePos2 = Input.mousePosition;

        chaC.SimpleMove(Vector3.right * Input.GetAxis("Horizontal") * 10f);

        mousePos3 = Camera.main.ScreenToWorldPoint(new Vector3(mousePos2.x, mousePos2.y, -Camera.main.transform.position.z));
        playerToMouse = (mousePos3 - (Vector2)pointeur.position).normalized;
        angle = Mathf.Atan2(playerToMouse.y, playerToMouse.x);
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);


        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            if (Physics.Raycast(pointeurBase.position, playerToMouse, out hit, reach))
            {
                if (hit.transform.parent.CompareTag("CubeMangeable"))
                {
                    Raf_CubeMangeable cubeMangeable;
                    if (hit.transform.parent.TryGetComponent<Raf_CubeMangeable>(out cubeMangeable))
                        cubeMangeable.GetManged();
                    else
                        print("Pas de Raf_CubeMangeable dans le cube visé.");
                }
            }
        }
    }
}
