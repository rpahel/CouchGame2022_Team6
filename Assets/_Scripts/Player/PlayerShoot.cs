using CustomMaths;
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
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture utilisé pour tirer.")]
    private int necessaryFood;
    [SerializeField, Range(0, 2), Tooltip("Laps de temps entre chaque tir.")]
    private float cooldown;
    private float cdTimer;
    [SerializeField, Tooltip("Le gameObject AimPivot de ce Prefab.")]
    private Transform aimPivot;

    //============================
    [Header("Projectile")]
    [SerializeField] private float initialSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float bounceForce;
    [SerializeField, Range(0, 100), Tooltip("Pourcentage de nourriture retiré au joueur ennemi touché.")]
    private int inflictedFoodDamage;
    [SerializeField, Tooltip("Force du knockback infligé au joueur ennemi.")]
    private float knockBackForce;

    //============================
    private float raycastRange; // Utilisé pour voir si y'a assez de place pour tirer
    #endregion

    #region Unity_Functions
    private void Start()
    {
        raycastRange = GameManager.Instance.LevelGenerator.Scale * 4;
    }

    private void FixedUpdate()
    {
        if (PManager.PlayerState == PLAYER_STATE.SHOOTING)
        {
            aimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, PManager.AimDirection != Vector2.zero ? PManager.AimDirection : PManager.LookDirection) - 90f);
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
            Debug.Log("Vous etes stunned et ne pouvez donc pas tirer.");
            return;
        }

        if (cdTimer > 0)
        {
            Debug.Log($"Attendez le cooldown du tir ({cdTimer:0.000}s)");
            return;
        }

        if(PManager.PEat.fullness < necessaryFood)
        {
            Debug.Log("Pas assez de nourriture pour shoot.");
            return;
        }

        if(aimDirection == Vector2.zero)
            aimDirection = PManager.LookDirection;

        if (!IsThereEnoughSpace(aimDirection))
        {
            Debug.Log("Not enough space to spawn a cube.");
            return;
        }

        ShootProjectile(aimDirection);

        PManager.PEat.fullness = Mathf.Clamp(PManager.PEat.fullness - necessaryFood, 0, 100);
        PManager.UpdatePlayerScale();

        cdTimer = cooldown;
        StartCoroutine(Cooldown());
    }
    
    private void ShootProjectile(Vector2 aimDirection)
    {
        Projectile projectile = GameManager.Instance.GetAvailableProjectile();
        //projectile.owner = PManager;
        projectile.color = PManager.color;
        projectile.transform.position = transform.position;
        projectile.gravity = gravity;
        projectile.bounceForce = bounceForce;
        projectile.percentageDealt = inflictedFoodDamage;
        projectile.knockBackForce = knockBackForce;
        projectile.ownerVelocityAtLaunch = PManager.Rb2D.velocity;

        projectile.gameObject.SetActive(true);
        projectile.Shoot(aimDirection, initialSpeed);
    }

    public void HoldShoot()
    {
        if (PManager.PlayerState != PLAYER_STATE.KNOCKBACKED && PManager.PlayerState != PLAYER_STATE.STUNNED)
        {
            if (PManager.PEat.fullness >= necessaryFood)
            {
                PManager.PlayerState = PLAYER_STATE.SHOOTING;
                aimPivot.gameObject.SetActive(true);
            }
        }
    }

    private bool IsThereEnoughSpace(Vector2 aimDirection)
    {
        Vector2 rayOrigin = (Vector2)transform.position + (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin, aimDirection, raycastRange);

        #if UNITY_EDITOR
            if (!hit1)
                Debug.DrawRay(rayOrigin, aimDirection * raycastRange, Color.red, 1f);
            else
                Debug.DrawLine(rayOrigin, hit1.point, Color.red, 1f);
        #endif

        rayOrigin = (Vector2)transform.position - (Vector2)(Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, aimDirection) - 90f) * (.5f * Vector2.right));
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin, aimDirection, raycastRange);

        #if UNITY_EDITOR
            if(!hit2)
                Debug.DrawRay(rayOrigin, aimDirection * raycastRange, Color.red, 1f);
            else
                Debug.DrawLine(rayOrigin, hit2.point, Color.red, 1f);
        #endif

        RaycastHit2D closestHit;

        if (hit1 && hit2)
        {
            if (hit1.distance < hit2.distance)
                closestHit = hit1;
            else
                closestHit = hit2;
        }
        else if (hit1)
            closestHit = hit1;
        else
            closestHit = hit2;

        if (closestHit)
        {
            if (closestHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                return true;
        
            RaycastHit2D[] hits = CustomPhysics.SquareCast((Vector2)closestHit.transform.position + GameManager.Instance.LevelGenerator.Scale * closestHit.normal, GameManager.Instance.LevelGenerator.Scale * .9f);
        
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
