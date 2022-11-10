using Data;
using System.Collections;
using UnityEngine;

public class PlayerShootRaph : MonoBehaviour
{
    // TODO : Quand on shoot trop pr�s d'un cube �a annule le tir, on perd pas de bouffe.

    #region Autres Scripts
    //============================
    private PlayerManager _playerManager;
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
        _playerManager = GetComponent<PlayerManager>();
        raycastRange = GameManager.Instance.LevelGenerator.Echelle * 4;
    }
    #endregion

    #region Custom_Functions
    public void OnShoot(Vector2 aimDirection)
    {
        if (_playerManager.State != PlayerState.KNOCKBACKED)
            _playerManager.SetPlayerState(PlayerState.Moving);

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

        if (_playerManager.eatAmount < (float)pourcentageNecessaire/100)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
            return;
        }

        ProjectileRaph projectile = GameManager.Instance.GetAvailableProjectile();
        projectile.owner = _playerManager;
        //projectile.color = PManager.color;
        projectile.transform.position = transform.position;
        projectile.gravity = gravity;
        projectile.forceDuRebond = forceDuRebond;
        projectile.pourcentageInflige = pourcentageInflige;
        projectile.knockBackForce = knockBackForce;

        //ApplyShootOppositeForce(aimDirection);

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, vitesseInitiale);

        _playerManager.eatAmount -= pourcentageNecessaire/100;

        cdTimer = cooldown;
        StartCoroutine(Cooldown());
    }

    /*public void HoldShoot()
    {
        if (_playerManager.State != PlayerState.KNOCKBACKED && _playerManager.State != PlayerState.STUNNED)
        {
            if (_playerManager.eatAmount >= pourcentageNecessaire/100)
            {
                _playerManager.SetPlayerState(PlayerState.Shooting);
                //aimPivot.gameObject.SetActive(true);
            }
        }
    }*/

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

#if UNITY_EDITOR
        if (!hit1)
            Debug.DrawRay(rayOrigin, aimDirection * raycastRange, Color.red, 5f);
        else
            Debug.DrawLine(rayOrigin, hit1.point, Color.red, 5f);
#endif

        rayOrigin = (Vector2)transform.position - (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, aimDirection, raycastRange);

#if UNITY_EDITOR
        if (!hit2)
            Debug.DrawRay(rayOrigin, aimDirection * raycastRange, Color.red, 5f);
        else
            Debug.DrawLine(rayOrigin, hit2.point, Color.red, 5f);
#endif

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

            RaycastHit2D[] hits = GameManager.Instance.SquareCast((Vector2)winnerHit.transform.position + GameManager.Instance.LevelGenerator.Echelle * winnerHit.normal, GameManager.Instance.LevelGenerator.Echelle * .9f, true);

            foreach (RaycastHit2D hit2D in hits)
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

    public void Aim()
    {
        Debug.Log("Aim");
        _playerManager.SetPlayerState(PlayerState.Aiming);
    }
    #endregion
}