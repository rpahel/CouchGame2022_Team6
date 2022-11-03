using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private float cdTimer;
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
        if (PManager.PlayerState == Data.PLAYER_STATE.STUNNED)
        {
            Debug.Log("Vous êtes stunned et ne pouvez donc pas tirer.");
            return;
        }

        if (cdTimer > 0)
        {
            Debug.Log($"Attendez le cooldown du tir ({cdTimer:0.000}s)");
            return;
        }

        if(PManager.PEat.Remplissage < pourcentageNecessaire)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }

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

        PManager.PEat.Remplissage -= pourcentageNecessaire;

        cdTimer = cooldown;
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        while(cdTimer > 0)
        {
            cdTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        cdTimer = 0;
    }
    #endregion
}
