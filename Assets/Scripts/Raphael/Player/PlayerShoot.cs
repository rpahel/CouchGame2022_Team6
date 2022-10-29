using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    #region Autres Scripts
    //============================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Variables
    //============================
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilisé pour tirer.")]
    private int pourcentageNecessaire;
    [SerializeField, Range(0, 2), Tooltip("Laps de temps entre chaque tir.")]
    private float cooldown;
    [SerializeField, Tooltip("Force de poussée arrière sur ce joueur.")]
    private float forceOpposee;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retiré au joueur s'il se fait tirer dessus.")]
    private int pourcentagePerdu;

    [Header("Projectile")]
    [SerializeField] private float vitesseInitiale;
    [SerializeField] private float gravity;
    [SerializeField] private float forceDuRebond;
    #endregion

    #region Custom_Functions
    public void OnShoot(Vector2 aimDirection)
    {
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = gameObject;
        Debug.Log("avant le set de couleur");
        projectile.PrColor = PManager.color;
        Debug.Log("après le set de couleur");
        projectile.transform.position = transform.position;
        projectile.Gravity = gravity;
        projectile.ForceDuRebond = forceDuRebond;

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, vitesseInitiale);
    }
    #endregion
}
