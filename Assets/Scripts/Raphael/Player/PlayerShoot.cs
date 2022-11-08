using Data;
using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    // TODO : Quand on shoot trop pr�s d'un cube �a annule le tir, on perd pas de bouffe.

    #region Autres Scripts
    //============================
    public PlayerManager PManager { get; set; }
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

    //============================
    [Header("Projectile")]
    [SerializeField] private float vitesseInitiale;
    [SerializeField] private float gravity;
    [SerializeField] private float forceDuRebond;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retir� au joueur ennemi touch�.")]
    private int pourcentageInflige;
    [SerializeField, Tooltip("Force du knockback inflig� au joueur ennemi.")]
    private float knockBackForce;

    //============================
    private float raycastRange;
    private float limitDistance;
    #endregion

    #region Unity_Functions
    private void Start()
    {
        raycastRange = LevelGenerator.Instance.Echelle * 4;
        limitDistance = 1.5f * LevelGenerator.Instance.Echelle;
    }

    private void FixedUpdate()
    {
        if (PManager.PlayerState == PLAYER_STATE.SHOOTING)
        {
            aimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, PManager.AimDirection) - 90f);
        }
        else if (aimPivot.gameObject.activeSelf)
        {
            aimPivot.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Custom_Functions
    public void OnShoot(Vector2 aimDirection)
    {
        if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED)
            PManager.PlayerState = PLAYER_STATE.WALKING;

        if (PManager.PlayerState == PLAYER_STATE.STUNNED)
        {
            Debug.Log("Vous �tes stunned et ne pouvez donc pas tirer.");
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

        if(aimDirection == Vector2.zero)
            aimDirection = PManager.PMovement.SensDuRegard;

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
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

        //ApplyShootOppositeForce(aimDirection);

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, vitesseInitiale);

        PManager.PEat.Remplissage -= pourcentageNecessaire;
        PManager.PEat.Remplissage = Mathf.Clamp(PManager.PEat.Remplissage, 0, 100);
        PManager.UpdatePlayerScale();

        cdTimer = cooldown;
        StartCoroutine(Cooldown());
    }

    public void HoldShoot()
    {
        if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED && PManager.PlayerState != PLAYER_STATE.STUNNED)
        {
            if (PManager.PEat.Remplissage >= pourcentageNecessaire)
            {
                PManager.PlayerState = PLAYER_STATE.SHOOTING;
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

    private bool IsThereEnoughSpace(Vector2 aimDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, raycastRange);

        if (hit)
        {
            Vector2 selfToHit = hit.transform.position - transform.position;

            if (hit.collider.CompareTag("Player"))
                return true;

            if (Mathf.Abs(selfToHit.x) >= limitDistance + PManager.PCollider.bounds.extents.x || Mathf.Abs(selfToHit.y) >= limitDistance + PManager.PCollider.bounds.extents.y)
            {
                return true;
            }
            else
                return false;
        }

        return true;
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
