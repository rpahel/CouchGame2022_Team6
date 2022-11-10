using System.Collections;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    #region Variables
    //=============================================
    [SerializeField, Range(0, 3)] private float dureeDeVie;
    private float age;

    //=============================================
    [HideInInspector] public PlayerManager owner;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 currentVelocity;
    private Coroutine lifeTime;
    private bool hasHurt;

    //=============================================
    [HideInInspector] public Color color;
    [HideInInspector] public float gravity;
    [HideInInspector] public float forceDuRebond;
    [HideInInspector] public int pourcentageInflige;
    [HideInInspector] public float knockBackForce;
    #endregion

    #region Unity_Functions
    //=============================================
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        spriteRenderer.color = Color.black;
        rb.gravityScale = gravity;
        age = dureeDeVie;
        lifeTime = StartCoroutine(DecreaseLifetime());
        hasHurt = false;
    }

    private void OnDisable()
    {
        spriteRenderer.color = Color.white;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        col.isTrigger = true;
        transform.position = Vector2.zero;
        if(lifeTime != null)
            StopCoroutine(lifeTime);
        age = 0;
        hasHurt = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != owner.gameObject)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 sensDuKnockBack = (collider.transform.position - transform.position).x > 0 ? new Vector2(-1, 1f) : new Vector2(1, 1f);
                sensDuKnockBack.Normalize();
                collider.gameObject.GetComponent<PlayerManager>().OnDamage(owner, pourcentageInflige, sensDuKnockBack * knockBackForce);
                rb.velocity = forceDuRebond * currentVelocity.magnitude * new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y);
                hasHurt = true;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, ApproxNormal(transform.position - collider.transform.position)).normalized;
                rb.velocity = currentVelocity.magnitude * forceDuRebond * sensDuRebond;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Destructible") || collider.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
            {
                if (CanSpawnCubeAt(PositionInNormalDirection(collider.transform.position, transform.position - collider.transform.position, LevelGenerator.Instance.Echelle), owner.transform.position - collider.transform.position))
                    SpawnCube(collider);
                else
                    Debug.Log("Not enough space to spawn cube.");

                gameObject.SetActive(false);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, ApproxNormal(transform.position - collider.transform.position)).normalized;
                rb.velocity = currentVelocity.magnitude * forceDuRebond * sensDuRebond;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == owner.gameObject)
        {
            col.isTrigger = false;
        }
    }

    // TODO : Quand on tire pile entre deux cubes ça créer deux cubes <- faut pas que ça le fasse du coup
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != owner.gameObject)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 sensDuKnockBack = collision.GetContact(0).normal.x > 0 ? new Vector2(-1, 1f) : new Vector2(1, 1f);
                sensDuKnockBack.Normalize();
                collision.gameObject.GetComponent<PlayerManager>().OnDamage(owner, pourcentageInflige, sensDuKnockBack * knockBackForce);
                rb.velocity = forceDuRebond * currentVelocity.magnitude * new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y);
                hasHurt = true;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
                rb.velocity = currentVelocity.magnitude * forceDuRebond * sensDuRebond;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible") || collision.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
            {
                if (CanSpawnCubeAt(PositionInNormalDirection(collision.transform.position, collision.GetContact(0).normal, LevelGenerator.Instance.Echelle), owner.transform.position - collision.transform.position))
                    SpawnCube(collision);
                else
                    Debug.Log("Not enough space to spawn cube.");

                gameObject.SetActive(false);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
                rb.velocity = currentVelocity.magnitude * forceDuRebond * sensDuRebond;
            }
        }
        else
        {
            Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
            rb.velocity = sensDuRebond * currentVelocity.magnitude * forceDuRebond;
        }
    }

    private void LateUpdate()
    {
        currentVelocity = rb.velocity;
    }
    #endregion

    #region Custom_Functions
    //=============================================
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

    private void SpawnCube(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 targetPos = PositionInNormalDirection(collision.transform.position / LevelGenerator.Instance.Echelle, normal);
        Transform targetTransform = LevelGenerator.Instance.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];
        Cube_Edible cube;
        if(targetTransform.TryGetComponent(out cube))
        {
            cube.GetVomited(collision.GetContact(0).point);
        }
    }

    private void SpawnCube(Collider2D collider)
    {
        Vector2 normal = ApproxNormal(transform.position - collider.transform.position);
        Vector2 targetPos = PositionInNormalDirection(collider.transform.position / LevelGenerator.Instance.Echelle, normal);
        Transform targetTransform = LevelGenerator.Instance.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];
        Cube_Edible cube;
        if (targetTransform.TryGetComponent(out cube))
        {
            cube.GetVomited(transform.position);
        }
    }

    private Vector2 PositionInNormalDirection(Vector2 originalPos, Vector2 normal, float scale = 1f)
    {
        return originalPos + scale * ApproxNormal(normal);
    }

    bool CanSpawnCubeAt(Vector2 position, Vector2 impactedCubeToOwner)
    {
        RaycastHit2D[] hits = GameManager.Instance.SquareCast(position, LevelGenerator.Instance.Echelle * .9f, true);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit && hit.transform.gameObject.layer != LayerMask.NameToLayer("Projectile"))
            {
                if(hit.transform.GetComponent<PlayerManager>() == owner)
                {
                    if (hasHurt) return false;

                    if(ClosestAxis(impactedCubeToOwner).x == 0)
                    {
                        Debug.DrawRay(position, impactedCubeToOwner, Color.blue, 5f);
                        Vector2 safe = position + (owner.PCollider.bounds.extents.y + .5f * LevelGenerator.Instance.Echelle) * ClosestAxis(impactedCubeToOwner);
                        owner.PousseToiVers(safe + new Vector2(owner.transform.position.x - safe.x, 0));
                    }
                    else
                    {
                        Debug.DrawRay(position, impactedCubeToOwner, Color.blue, 5f);
                        Vector2 safe = position + (owner.PCollider.bounds.extents.x + .5f * LevelGenerator.Instance.Echelle) * ClosestAxis(impactedCubeToOwner);
                        owner.PousseToiVers(safe + new Vector2(0, owner.transform.position.y - safe.y));
                    }

                    return true;
                }

                return false;
            }
        }

        return true;
    }

    Vector2 ApproxNormal(Vector2 normalToApproximate)
    {
        normalToApproximate.Normalize();
        const float cos30 = 0.866f;

        if (normalToApproximate.y > cos30 && (normalToApproximate.x > -.5f && normalToApproximate.x < .5f))
            return Vector2.up;            // N
        else if ((normalToApproximate.x > .5f && normalToApproximate.x < cos30) && (normalToApproximate.y > .5f && normalToApproximate.y < cos30))
            return Vector2.one;           // NE
        else if (normalToApproximate.x > cos30 && (normalToApproximate.y > -.5f && normalToApproximate.y < .5f))
            return Vector2.right;         // E
        else if ((normalToApproximate.x > .5f && normalToApproximate.x < cos30) && (normalToApproximate.y < -.5f && normalToApproximate.y > -cos30))
            return new Vector2(1, -1);    // SE
        else if (normalToApproximate.y < -cos30 && (normalToApproximate.x > -.5f && normalToApproximate.x < .5f))
            return Vector2.down;          // S
        else if ((normalToApproximate.x > -cos30 && normalToApproximate.x < -.5f) && (normalToApproximate.y < -.5f && normalToApproximate.y > -cos30))
            return -Vector2.one;           // SW
        else if (normalToApproximate.x < -cos30 && (normalToApproximate.y > -.5f && normalToApproximate.y < .5f))
            return Vector2.left;          // W
        else if ((normalToApproximate.x > -cos30 && normalToApproximate.x < -.5f) && (normalToApproximate.y > .5f && normalToApproximate.y < cos30))
            return new Vector2(-1, 1);    // NW

        Debug.Log("Tu ne devrais pas voir ça.");
        return Vector2.up;
    }

    Vector2 ClosestAxis(Vector2 direction, bool predictUsingOwnerVelocity = false) // TODO : Retourne un vecteur différent en fonction de la vélocité de l'owner pour une meme direction
    {
        if (!predictUsingOwnerVelocity)
        {
            if(Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            {
                if (direction.y > 0) return Vector2.up;
                else return Vector2.down;
            }
            else
            {
                if (direction.x > 0) return Vector2.right;
                else return Vector2.left;
            }
        }
        else
        {
            throw new Exception("Not implemented yet.");
        }
    }
    #endregion
}
