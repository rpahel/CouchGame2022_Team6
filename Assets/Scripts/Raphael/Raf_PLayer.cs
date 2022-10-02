using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Raf_PLayer : MonoBehaviour
{
    private CharacterController chaC;

    public Transform            pointeur;       // Le Parent du viseur pour la rotation du "bras"
    public Transform            pointeurBase;   // Point de départ des raycasts
    public float                reach = 1f;     // Longueur du raycast

    private Vector2             mousePos2;      // Position de la souris à l'écran
    private Vector2             mousePos3;      // Position de la souris dans le monde
    private Vector2             playerToMouse;  // Vecteur entre le joueur et la souris
    private float               angle;          // Angle entre le vecteur joueur -> souris et le vecteur right

    private void Awake()
    {
        chaC = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Déplacement
        chaC.SimpleMove(Vector3.right * Input.GetAxis("Horizontal") * 10f);

        // Viser avec la souris
        mousePos2 = Input.mousePosition;
        mousePos3 = Camera.main.ScreenToWorldPoint(new Vector3(mousePos2.x, mousePos2.y, -Camera.main.transform.position.z));
        playerToMouse = (mousePos3 - (Vector2)pointeur.position).normalized;
        angle = Mathf.Atan2(playerToMouse.y, playerToMouse.x);
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);

        // Si on touche un cube mangeable, on le mange.
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
