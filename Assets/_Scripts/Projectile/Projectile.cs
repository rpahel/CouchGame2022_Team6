using System;
using System.Collections;
using UnityEngine;
using CustomMaths;

public class Projectile : MonoBehaviour
{
    #region Variables
    //=============================================
    public SpriteRenderer insideSpriteRenderer; // On devrait plus en avoir besoin une fois qu'on aura les sprites des GA
    [SerializeField, Range(0, 3)] private float lifetime;
    private float age;

    //=============================================
    [HideInInspector] public PlayerManager owner;
    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private Collider2D col;
    private Coroutine lifeTimeCoroutine;
    private bool hasHurt;

    //=============================================
    [HideInInspector] public Color color;
    [HideInInspector] public float gravity;
    [HideInInspector] public float bounceForce;
    [HideInInspector] public int percentageDealt;
    [HideInInspector] public float knockBackForce;
    [HideInInspector] public Vector2 ownerVelocityAtLaunch;
    #endregion

    #region Unity_Functions
    //=============================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        //insideSpriteRenderer.color = Color.black;
        rb.gravityScale = gravity;
        age = lifetime;
        hasHurt = false;
        lifeTimeCoroutine = StartCoroutine(DecreaseLifetime());
    }

    private void OnDisable()
    {
        insideSpriteRenderer.color = Color.white;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        col.isTrigger = true;
        transform.position = Vector2.zero;

        if(lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }

        age = 0;
        hasHurt = false;
    }

    // Si le projectile entre dans un joueur, il lui applique des dégats et rebondit.
    // S'il entre dans un piège, il rebondit.
    // S'il entre dans un bloc Destructible ou Indestructible, il essaie de spawn un cube et disparait.
    // S'il entre dans un autre projectile, il rebondit. J'ai séparé ce cas du piège au cas où on veut faire d'autres trucs avec.
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != owner.gameObject)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 sensDuKnockBack = (collider.transform.position - transform.position).x > 0 ? new Vector2(-1, 1f) : new Vector2(1, 1f);
                sensDuKnockBack.Normalize();
                collider.gameObject.GetComponent<PlayerManager>().OnDamage(owner, percentageDealt, sensDuKnockBack * knockBackForce);
                rb.velocity = bounceForce * currentVelocity.magnitude * new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y);
                hasHurt = true;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, CustomVectors.ApproxNormal(transform.position - collider.transform.position)).normalized;
                rb.velocity = currentVelocity.magnitude * bounceForce * sensDuRebond;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Destructible") || collider.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
            {
                if (CanSpawnCubeAt(PositionInNormalDirection(collider.transform.position, transform.position - collider.transform.position, GameManager.Instance.LevelGenerator.Scale), owner.transform.position - collider.transform.position))
                    SpawnCube(collider);
                else
                    Debug.Log("Not enough space to spawn cube.");

                gameObject.SetActive(false);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, CustomVectors.ApproxNormal(transform.position - collider.transform.position)).normalized;
                rb.velocity = currentVelocity.magnitude * bounceForce * sensDuRebond;
            }
        }
    }

    // Rend le projectile solide quand il sort du joueur qui l'a tiré.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == owner.gameObject)
        {
            col.isTrigger = false;
        }
    }

    // Si le projectile touche un joueur, il lui applique des dégats et rebondit.
    // S'il touche un piège, il rebondit.
    // S'il touche un bloc Destructible ou Indestructible, il essaie de spawn un cube et disparait.
    // S'il touche un autre projectile, il rebondit. J'ai séparé ce cas du piège au cas où on veut faire d'autres trucs avec.
    // S'il touche le joueur qui l'a tiré, il rebondit.
        // TODO : Quand on tire pile entre deux cubes ça créer deux cubes <- faut pas que ça le fasse du coup
            // Un bool pour savoir si le projectile a déjà placer un cube devrait faire l'affaire.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != owner.gameObject)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 sensDuKnockBack = collision.GetContact(0).normal.x > 0 ? new Vector2(-1, 1f) : new Vector2(1, 1f);
                sensDuKnockBack.Normalize();
                collision.gameObject.GetComponent<PlayerManager>().OnDamage(owner, percentageDealt, sensDuKnockBack * knockBackForce);
                rb.velocity = bounceForce * currentVelocity.magnitude * new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y);
                hasHurt = true;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
                rb.velocity = currentVelocity.magnitude * bounceForce * sensDuRebond;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible") || collision.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
            {
                if (CanSpawnCubeAt(PositionInNormalDirection(collision.transform.position, collision.GetContact(0).normal, GameManager.Instance.LevelGenerator.Scale), owner.transform.position - collision.transform.position))
                    SpawnCube(collision);
                else
                    Debug.Log("Not enough space to spawn cube.");

                gameObject.SetActive(false);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
                rb.velocity = currentVelocity.magnitude * bounceForce * sensDuRebond;
            }
        }
        else
        {
            Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
            rb.velocity = sensDuRebond * currentVelocity.magnitude * bounceForce;
        }
    }

    private void LateUpdate()
    {
        currentVelocity = rb.velocity;
    }
    #endregion

    #region Custom_Functions
    //=============================================

    /// <summary>
    /// Envoie le projectile dans la direction donnée.
    /// </summary>
    /// <param name="dir">La direction de tir.</param>
    /// <param name="speed">La vitesse initiale du projectile.</param>
    public void Shoot(Vector2 dir, float speed)
    {
        rb.velocity = dir * speed;
    }

    private IEnumerator DecreaseLifetime()
    {
        while(age > 0)
        {
            yield return new WaitForFixedUpdate();
            age -= Time.deltaTime;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Spawn un cube en fonction du cube touché et de la position du projectile.
    /// </summary>
    /// <param name="collision">Les informations de la collision.</param>
    private void SpawnCube(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 targetPos = PositionInNormalDirection(collision.transform.position / GameManager.Instance.LevelGenerator.Scale, normal);
        Transform targetTransform = GameManager.Instance.LevelGenerator.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];
        Cube_Edible cube;
        if(targetTransform.TryGetComponent(out cube))
        {
            cube.GetVomited(collision.GetContact(0).point);
        }
    }

    /// <summary>
    /// Spawn un cube en fonction du cube pénétré et de la position du projectile.
    /// </summary>
    /// <param name="collision">Le collider du pauvre cube.</param>
    private void SpawnCube(Collider2D collider)
    {
        Vector2 normal = CustomVectors.ApproxNormal(transform.position - collider.transform.position);
        Vector2 targetPos = PositionInNormalDirection(collider.transform.position / GameManager.Instance.LevelGenerator.Scale, normal);
        Transform targetTransform = GameManager.Instance.LevelGenerator.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];
        Cube_Edible cube;
        if (targetTransform.TryGetComponent(out cube))
        {
            cube.GetVomited(transform.position);
        }
    }

    /// <summary>
    /// Retourne une position en fonction d'une normale approximée de la normale fournie et d'une position de d'origine.
    /// </summary>
    /// <param name="originalPos">La position d'origine (exemple : le cube touché).</param>
    /// <param name="normal">La normale brute (exemple : la normale de la collision avec le cube).</param>
    /// <param name="scale">L'échelle du monde.</param>
    private Vector2 PositionInNormalDirection(Vector2 originalPos, Vector2 normal, float scale = 1f)
    {
        return originalPos + scale * CustomVectors.ApproxNormal(normal);
    }

    /// <summary>
    /// Vérifie si un cube a la place de spawner. Si le joueur qui a tiré sera dans la zone de spawn du cube, il est poussé un peu pour faire de la place.
    /// </summary>
    /// <param name="position">Position de spawn du cube.</param>
    /// <param name="impactedCubeToOwner">Direction entre le cube touché et le joueur.</param>
        // TODO : Retirer les debug
    bool CanSpawnCubeAt(Vector2 position, Vector2 impactedCubeToOwner)
    {
        RaycastHit2D[] hits = CustomPhysics.SquareCast(position, GameManager.Instance.LevelGenerator.Scale * .9f, true);

        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i] && hits[i].transform.gameObject.layer != LayerMask.NameToLayer("Projectile"))
            {
                if(hits[i].transform.GetComponent<PlayerManager>() == owner)
                {
                    if (hasHurt) return false;

                    if(ClosestAxis(impactedCubeToOwner, true).x == 0)
                    {
                        #if UNITY_EDITOR
                        {
                            Debug.DrawRay((Vector2)owner.transform.position - impactedCubeToOwner, impactedCubeToOwner, Color.blue, 5f);
                        }
                        #endif

                        Vector2 safe = position + (owner.PCollider.bounds.extents.y + .5f * GameManager.Instance.LevelGenerator.Scale) * ClosestAxis(impactedCubeToOwner, true);
                        owner.MoveOverTo(safe + new Vector2(owner.transform.position.x - safe.x, 0));
                    }
                    else
                    {
                        #if UNITY_EDITOR
                        {
                            Debug.DrawRay((Vector2)owner.transform.position - impactedCubeToOwner, impactedCubeToOwner, Color.blue, 5f);
                        }
                        #endif

                        Vector2 safe = position + (owner.PCollider.bounds.extents.x + .5f * GameManager.Instance.LevelGenerator.Scale) * ClosestAxis(impactedCubeToOwner, true);
                        owner.MoveOverTo(safe + new Vector2(0, owner.transform.position.y - safe.y));
                    }
                    return true;
                }
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Retourne l'axe (up, down, left, right) le plus proche de la direction donnée.
    /// </summary>
    /// <param name="direction">La direction que l'on souhaite arrondir à l'axe le plus proche.</param>
    /// <param name="predictUsingOwnerVelocity">Est-ce qu'on utilise la vitesse du joueur pour prédire quel sera l'axe le plus proche ?</param>
    private Vector2 ClosestAxis(Vector2 direction, bool predictUsingOwnerVelocity = false)
    {
        if (!predictUsingOwnerVelocity)
        {
            return CustomVectors.ClosestAxis(direction);
        }
        else
        {
            direction = CustomVectors.ApproxNormal(direction);

            #if UNITY_EDITOR
            {
                Debug.DrawRay(owner.transform.position, ownerVelocityAtLaunch.normalized * 4f, Color.magenta, 5f);
                //Debug.Log(ownerVelocityAtLaunch);
            }
            #endif

            if(ownerVelocityAtLaunch == Vector2.zero) { ownerVelocityAtLaunch = Vector2.down; }
            Vector2 ownerVelocity = CustomVectors.ClosestAxis(ownerVelocityAtLaunch);

            #if UNITY_EDITOR
            {
                Debug.DrawRay(owner.transform.position, ownerVelocity * 4f, Color.yellow, 5f);
            }
            #endif

            if (direction == Vector2.up || direction == Vector2.down || direction == Vector2.left || direction == Vector2.right)
            {
                if (ownerVelocity == Vector2.left || ownerVelocity == Vector2.right) return ownerVelocity == Vector2.left ? Vector2.right : Vector2.left;
                else if (ownerVelocity == Vector2.down || ownerVelocity == Vector2.up) return ownerVelocity == Vector2.down ? Vector2.up : Vector2.down;
            }
            else if (direction == Vector2.one)
            {
                if(ownerVelocity == Vector2.left) return Vector2.right;
                else if(ownerVelocity == Vector2.down) return Vector2.up;
            }
            else if(direction == -Vector2.one)
            {
                if (ownerVelocity == Vector2.right) return Vector2.left;
                else if (ownerVelocity == Vector2.up) return Vector2.down;
            }
            else if(direction == new Vector2(1, -1))
            {
                if (ownerVelocity == Vector2.left) return Vector2.right;
                else if (ownerVelocity == Vector2.up) return Vector2.down;
            }
            else if(direction == new Vector2(-1, 1))
            {
                if (ownerVelocity == Vector2.right) return Vector2.left;
                else if (ownerVelocity == Vector2.down) return Vector2.up;
            }

            // C'est pas censé arriver là mais au cas où je mets ça là
            if (ownerVelocity == Vector2.left || ownerVelocity == Vector2.right) return ownerVelocity == Vector2.left ? Vector2.right : Vector2.left;
            else return ownerVelocity == Vector2.down ? Vector2.up : Vector2.down;
        }
    }
    #endregion
}
