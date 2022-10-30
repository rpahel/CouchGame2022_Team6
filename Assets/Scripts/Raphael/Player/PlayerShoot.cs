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
    [SerializeField, Tooltip("Force de poussée arrière sur ce joueur suite à son propre tir.")]
    private float forceOpposee;

    [Header("Projectile")]
    [SerializeField] private float vitesseInitiale;
    [SerializeField] private float gravity;
    [SerializeField] private float forceDuRebond;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retiré au joueur ennemi touché.")]
    private int pourcentageInflige;
    [SerializeField, Tooltip("Force du knockback infligé au joueur ennemi.")]
    private float knockBackForce;
    #endregion

    #region Custom_Functions
    public void OnShoot(Vector2 aimDirection)
    {
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = PManager;
        projectile.color = PManager.color;
        projectile.transform.position = transform.position;
        projectile.gravity = gravity;
        projectile.forceDuRebond = forceDuRebond;
        projectile.pourcentageInflige = pourcentageInflige;
        projectile.knockBackForce = knockBackForce;

        if(aimDirection == Vector2.zero)
            aimDirection = PManager.PMovement.SensDuRegard;

        PManager.Rb2D.AddForce(-aimDirection * forceOpposee, ForceMode2D.Impulse);

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, vitesseInitiale);
    }
    #endregion
}
