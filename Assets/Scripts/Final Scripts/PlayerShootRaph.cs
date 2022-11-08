using System;
using Data;
using System.Collections;
using UnityEngine;

public class PlayerShootRaph : MonoBehaviour
{
    // TODO : Quand on shoot trop pr�s d'un cube �a annule le tir, on perd pas de bouffe.

    #region Autres Scripts
    //============================
    private PlayerManager _playerManager;
    private Movement _movement;
    #endregion

    #region Variables
    //============================
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilis� pour tirer.")]
    private int pourcentageNecessaire;
    [SerializeField, Range(0, 2), Tooltip("Laps de temps entre chaque tir.")]
    private float cooldown;
    private float cdTimer;
    [SerializeField, Tooltip("Force de pouss�e arri�re sur ce joueur suite � son propre tir.")]
    private float forceOpposee;
    [SerializeField]
    private Transform aimPivot;

    [Header("Projectile")]
    [SerializeField] private float vitesseInitiale;
    [SerializeField] private float gravity;
    [SerializeField] private float forceDuRebond;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retir� au joueur ennemi touch�.")]
    private int pourcentageInflige;
    [SerializeField, Tooltip("Force du knockback inflig� au joueur ennemi.")]
    private float knockBackForce;
    #endregion

    #region Unity_Functions

    private void Awake()
    {
        _movement = GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        /*if (_playerManager.State == PlayerState.Shooting)
        {
            aimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, PManager.AimDirection) - 90f);
        }
        else if (aimPivot.gameObject.activeSelf)
        {
            aimPivot.gameObject.SetActive(false);
        }*/
    }
    #endregion

    #region Custom_Functions
    public void OnShoot()
    {
        if (_playerManager.State == PlayerState.STUNNED)
        {
            Debug.Log("Vous �tes stunned et ne pouvez donc pas tirer.");
            return;
        }

        if (cdTimer > 0)
        {
            Debug.Log($"Attendez le cooldown du tir ({cdTimer:0.000}s)");
            return;
        }

        /*if(PManager.PEat.Remplissage < pourcentageNecessaire)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }*/

        ProjectileRaph projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = _playerManager;
        //projectile.color = PManager.color;
        projectile.transform.position = transform.position;
        projectile.gravity = gravity;
        projectile.forceDuRebond = forceDuRebond;
        projectile.pourcentageInflige = pourcentageInflige;
        projectile.knockBackForce = knockBackForce;

        var dir = _playerManager.InputVector;
        if (dir == Vector2.zero)
            dir = _movement.lookAtRight ? Vector2.right : Vector2.left;

            //ApplyShootOppositeForce(aimDirection);

        projectile.gameObject.SetActive(true);
        projectile.Shoot(dir, vitesseInitiale);

        //PManager.PEat.Remplissage -= pourcentageNecessaire;
        //PManager.PEat.Remplissage = Mathf.Clamp(PManager.PEat.Remplissage, 0, 100);
        //PManager.UpdatePlayerScale();

        cdTimer = cooldown;
        StartCoroutine(Cooldown());

        if (_playerManager.State != PlayerState.KNOCKBACKED)
            _playerManager.SetPlayerState(PlayerState.Moving);
    }

    public void HoldShoot()
    {
        if (_playerManager.State != PlayerState.KNOCKBACKED && _playerManager.State != PlayerState.STUNNED)
        {
            //if (PManager.PEat.Remplissage >= pourcentageNecessaire)
            {
                _playerManager.SetPlayerState(PlayerState.Shooting); 
                aimPivot.gameObject.SetActive(true);
            }
        }
    }

    //void ApplyShootOppositeForce(Vector2 aimDirection)
    //{
    //    Vector2 opposite = -aimDirection;
    //
    //    if(opposite.y < 0.2f && Mathf.Abs(opposite.x) > 0.1f)
    //    {
    //        if (PManager.PMovement.GroundCheck)
    //        {
    //            opposite = new Vector2(opposite.x, .5f);
    //            opposite.Normalize();
    //        }
    //    }
    //
    //    PManager.Rb2D.AddForce(opposite * forceOpposee, ForceMode2D.Impulse);
    //    PManager.PlayerState = Data.PLAYER_STATE.SHOOTING;
    //}

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