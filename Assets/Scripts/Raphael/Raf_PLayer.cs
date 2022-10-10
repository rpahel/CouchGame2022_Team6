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
    public Transform            pointeurBase;   // Point de départ des raycasts
    public float                reach = 1f;     // Longueur du raycast
    
    private Vector2             aimingPos;      // La position du stick droit
    private Vector2             aimingDir;      // La direction de visée
    private float               angle;          // Angle entre le vecteur joueur -> souris et le vecteur right
    
    private bool                canEat = true;
    private float               satiety = 0f;   // La satiété
    private float               eatCooldown = 0.5f;
    private Coroutine           cooldownCoroutine;
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
    
    public void TryEat(InputAction.CallbackContext context)
    {
        if (!canEat)
        {
            print("I'm on eating cooldown !");
            return;
        }
    
        if(satiety < 1f)
        {
            if (context.action.IsPressed())
            {
                RaycastHit hit;
                if (Physics.Raycast(pointeurBase.position, aimingDir, out hit, reach))
                {
                    if (hit.transform.parent.CompareTag("CubeEdible"))
                    {
                        Cube_Edible cubeMangeable;
                        if (hit.transform.parent && hit.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
                            Eat(cubeMangeable);
                        else
                            print("Pas de Raf_CubeMangeable dans le cube visé.");
                    }
                }
            }
        }
        else
        {
            print("I'm full !!");
        }
    }
    
    public void Eat(Cube_Edible cubeMangeable)
    {
        cubeMangeable.GetManged();
        satiety += .2f;
        satiety = Mathf.Clamp(satiety, 0f, 1f);
        canEat = false;
        cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        print(satiety);
    }
    
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
        StopCoroutine(cooldownCoroutine);
    }
    
    #endregion
}