using Data;
using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
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
    #endregion

    #region Unity_Functions
    private void Start()
    {
        raycastRange = LevelGenerator.Instance.Echelle * 4;
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
        projectile.ownerVelocityAtLaunch = PManager.Rb2D.velocity;

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
        Vector2 rayOrigin = (Vector2)transform.position + (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin, aimDirection, raycastRange);

        //#if UNITY_EDITOR
        //    if (!hit1)
        //        Debug.DrawRay(rayOrigin, aimDirection * raycastRange, Color.red, 5f);
        //    else
        //        Debug.DrawLine(rayOrigin, hit1.point, Color.red, 5f);
        //#endif

        rayOrigin = (Vector2)transform.position - (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, aimDirection, raycastRange);

        //#if UNITY_EDITOR
        //    if(!hit2)
        //        Debug.DrawRay(rayOrigin, aimDirection * raycastRange, Color.red, 5f);
        //    else
        //        Debug.DrawLine(rayOrigin, hit2.point, Color.red, 5f);
        //#endif

        RaycastHit2D winnerHit;

        if (hit1 && hit2)
        {
            if (hit1.distance < hit2.distance)
                winnerHit = hit1;
            else
                winnerHit = hit2;
        }
        else if (hit1)
            winnerHit = hit1;
        else
            winnerHit = hit2;

        if (winnerHit)
        {
            if (winnerHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                return true;
        
            RaycastHit2D[] hits = GameManager.Instance.SquareCast((Vector2)winnerHit.transform.position + LevelGenerator.Instance.Echelle * winnerHit.normal, LevelGenerator.Instance.Echelle * .9f);
        
            foreach(RaycastHit2D hit2D in hits)
            {
                if (hit2D)
                    return false;
            }
        }

        return true;
    }

    IEnumerator Cooldown()
    {
        while (cdTimer > 0)
        {
            cdTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        cdTimer = 0;
    }
    #endregion
}
