using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Raf_PLayer : MonoBehaviour
{
    #region VARIABLES
    private CharacterController chaC;

    private Vector2             moveDir;        // Input Move direction

    public Transform            pointeur;       // Le Parent du viseur pour la rotation du "bras"
    public Transform            pointeurBase;   // Point de d�part des raycasts
    public float                reach = 1f;     // Longueur du raycast

    private Vector2             aimingPos;      // La position du stick droit
    private Vector2             aimingDir;      // La direction de vis�e
    private float               angle;          // Angle entre le vecteur joueur -> souris et le vecteur right
    #endregion
    #region UNITY
    private void Awake()
    {
        chaC = GetComponent<CharacterController>();
        pointeur.gameObject.SetActive(false);
    }

    private void Update()
    {
        aimingDir = aimingPos.normalized;
        angle = Mathf.Atan2(aimingDir.y, aimingDir.x);
    }

    private void FixedUpdate()
    {
        chaC.SimpleMove(Vector3.right * moveDir.normalized * 10f);
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }
    #endregion
    #region INPUTS

    public void GetMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>().sqrMagnitude > 0.1f ? context.ReadValue<Vector2>() : Vector2.zero;
    }

    public void GetAiming(InputAction.CallbackContext context)
    {
        aimingPos = context.ReadValue<Vector2>();
        pointeur.gameObject.SetActive(aimingPos.sqrMagnitude > 0.1f ? true : false);
    }

    public void Eat(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            RaycastHit hit;
            if (Physics.Raycast(pointeurBase.position, aimingDir, out hit, reach))
            {
                if (hit.transform.parent.CompareTag("CubeMangeable"))
                {
                    Raf_CubeMangeable cubeMangeable;
                    if (hit.transform.parent.TryGetComponent<Raf_CubeMangeable>(out cubeMangeable))
                        cubeMangeable.GetManged();
                    else
                        print("Pas de Raf_CubeMangeable dans le cube vis�.");
                }
            }
        }
    }

    #endregion
}