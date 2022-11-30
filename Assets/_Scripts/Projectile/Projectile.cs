using CustomMaths;
using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables
    //=============================================
    public SpriteRenderer insideSpriteRenderer; // On devrait plus en avoir besoin une fois qu'on aura les sprites des GA
    [SerializeField, Range(0, 3)] private float _lifetime;
    private float _age;

    //=============================================
    [HideInInspector] public PlayerManager owner;
    private Rigidbody2D _rb;
    private Vector2 _currentVelocity;
    private Collider2D _col;
    private Coroutine _lifeTimeCoroutine;
    private bool _hasHurt;

    //=============================================
    [HideInInspector] public Color color;
    [HideInInspector] public float gravity;
    [HideInInspector] public float bounceForce;
    [HideInInspector] public int percentageDealt;
    [HideInInspector] public float knockBackForce;
    //[HideInInspector] public Vector2 ownerVelocityAtLaunch;
    #endregion

    #region Unity_Functions
    //=============================================
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        insideSpriteRenderer.color = color;
        _rb.gravityScale = gravity;
        _age = _lifetime;
        _hasHurt = false;
        _lifeTimeCoroutine = StartCoroutine(DecreaseLifetime());
    }

    private void OnDisable()
    {
        insideSpriteRenderer.color = Color.white;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        _col.isTrigger = true;
        transform.position = Vector2.zero;

        if (_lifeTimeCoroutine != null)
        {
            StopCoroutine(_lifeTimeCoroutine);
            _lifeTimeCoroutine = null;
        }

        _age = 0;
        _hasHurt = false;
    }

    // Si le projectile entre dans un joueur, il lui applique des degats et rebondit.
    // S'il entre dans un piege, il rebondit.
    // S'il entre dans un bloc Destructible ou Indestructible, il essaie de spawn un cube et disparait.
    // S'il entre dans un autre projectile, il rebondit. J'ai separe ce cas du piege au cas ou on veut faire d'autres trucs avec.
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != owner.gameObject)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 sensDuKnockBack = (collider.transform.position - transform.position).x > 0 ? new Vector2(-1, 1f) : new Vector2(1, 1f);
                sensDuKnockBack.Normalize();
                collider.gameObject.GetComponent<PlayerManager>().OnDamage(owner, percentageDealt, sensDuKnockBack * knockBackForce);
                _rb.velocity = bounceForce * _currentVelocity.magnitude * new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y) * Time.fixedDeltaTime;
                _hasHurt = true;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(_currentVelocity, CustomVectors.ApproxNormal(transform.position - collider.transform.position)).normalized;
                _rb.velocity = _currentVelocity.magnitude * bounceForce * sensDuRebond * Time.fixedDeltaTime;
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Destructible")
                || collider.gameObject.layer == LayerMask.NameToLayer("Indestructible")
                || collider.gameObject.layer == LayerMask.NameToLayer("Limite"))
            {
                if (CanSpawnCubeAt(PositionInNormalDirection(collider.transform.position, transform.position - collider.transform.position, GameManager.Instance.LevelGenerator.Scale), owner.transform.position - collider.transform.position))
                    SpawnCube(collider);
                else
                    Debug.Log("Not enough space to spawn cube.");

                gameObject.SetActive(false);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(_currentVelocity, CustomVectors.ApproxNormal(transform.position - collider.transform.position)).normalized;
                _rb.velocity = _currentVelocity.magnitude * bounceForce * sensDuRebond * Time.fixedDeltaTime;
            }
        }
    }

    // Rend le projectile solide quand il sort du joueur qui l'a tir�.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == owner.gameObject)
        {
            _col.isTrigger = false;
        }
    }

    // Si le projectile touche un joueur, il lui applique des d�gats et rebondit.
    // S'il touche un pi�ge, il rebondit.
    // S'il touche un bloc Destructible ou Indestructible, il essaie de spawn un cube et disparait.
    // S'il touche un autre projectile, il rebondit. J'ai separe ce cas du piege au cas ou on veut faire d'autres trucs avec.
    // S'il touche le joueur qui l'a tire, il rebondit.
    // TODO : Quand on tire pile entre deux cubes ca creer deux cubes <- faut pas que ca le fasse du coup
    // Un bool pour savoir si le projectile a deja placer un cube devrait faire l'affaire.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != owner.gameObject)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector2 sensDuKnockBack = collision.GetContact(0).normal.x > 0 ? new Vector2(-1, 1f) : new Vector2(1, 1f);
                sensDuKnockBack.Normalize();
                collision.gameObject.GetComponent<PlayerManager>().OnDamage(owner, percentageDealt, sensDuKnockBack * knockBackForce);
                _rb.velocity = bounceForce * _currentVelocity.magnitude * new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y) * Time.fixedDeltaTime;
                _hasHurt = true;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(_currentVelocity, collision.GetContact(0).normal).normalized;
                _rb.velocity = _currentVelocity.magnitude * bounceForce * sensDuRebond * Time.fixedDeltaTime;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible")
                || collision.gameObject.layer == LayerMask.NameToLayer("Indestructible")
                || collision.gameObject.layer == LayerMask.NameToLayer("Limite"))
            {
                if (CanSpawnCubeAt(PositionInNormalDirection(collision.transform.position, collision.GetContact(0).normal, GameManager.Instance.LevelGenerator.Scale), owner.transform.position - collision.transform.position))
                    SpawnCube(collision);
                else
                    Debug.Log("Not enough space to spawn cube.");

                gameObject.SetActive(false);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(_currentVelocity, collision.GetContact(0).normal).normalized;
                _rb.velocity = _currentVelocity.magnitude * bounceForce * sensDuRebond * Time.fixedDeltaTime;
            }
        }
        else
        {
            Vector2 sensDuRebond = Vector2.Reflect(_currentVelocity, collision.GetContact(0).normal).normalized;
            _rb.velocity = sensDuRebond * _currentVelocity.magnitude * bounceForce * Time.fixedDeltaTime;
        }
    }

    private void LateUpdate()
    {
        _currentVelocity = _rb.velocity;
    }
    #endregion

    #region Custom_Functions
    //=============================================

    /// <summary>
    /// Envoie le projectile dans la direction donnee.
    /// </summary>
    /// <param name="dir">La direction de tir.</param>
    /// <param name="speed">La vitesse initiale du projectile.</param>
    public void Shoot(Vector2 dir, float speed)
    {
        _rb.velocity = dir * speed;
    }

    private IEnumerator DecreaseLifetime()
    {
        while (_age > 0)
        {
            yield return new WaitForFixedUpdate();
            _age -= Time.deltaTime;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Spawn un cube en fonction du cube touche et de la position du projectile.
    /// </summary>
    /// <param name="collision">Les informations de la collision.</param>
    private void SpawnCube(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 targetPos = PositionInNormalDirection(collision.transform.position / GameManager.Instance.LevelGenerator.Scale, normal);
        Transform targetTransform = GameManager.Instance.LevelGenerator.CubesArray[Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y)];
        Cube_Edible cube;
        if (targetTransform.TryGetComponent(out cube))
        {
            cube.GetBarfed(collision.GetContact(0).point);
        }
    }

    /// <summary>
    /// Spawn un cube en fonction du cube penetre et de la position du projectile.
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
            cube.GetBarfed(transform.position);
        }
    }

    /// <summary>
    /// Retourne une position en fonction d'une normale approxim�e de la normale fournie et d'une position de d'origine.
    /// </summary>
    /// <param name="originalPos">La position d'origine (exemple : le cube touch�).</param>
    /// <param name="normal">La normale brute (exemple : la normale de la collision avec le cube).</param>
    /// <param name="scale">L'�chelle du monde.</param>
    private Vector2 PositionInNormalDirection(Vector2 originalPos, Vector2 normal, float scale = 1f)
    {
        return originalPos + scale * CustomVectors.ApproxNormal(normal);
    }

    /// <summary>
    /// V�rifie si un cube a la place de spawner. Si le joueur qui a tir� sera dans la zone de spawn du cube, il est pouss� un peu pour faire de la place.
    /// </summary>
    /// <param name="position">Position de spawn du cube.</param>
    /// <param name="impactedCubeToOwner">Direction entre le cube touch� et le joueur.</param>
        // TODO : Retirer les debug
    bool CanSpawnCubeAt(Vector2 position, Vector2 impactedCubeToOwner)
    {
        RaycastHit2D[] hits = CustomPhysics.SquareCast(position, GameManager.Instance.LevelGenerator.Scale * .9f, true);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] && hits[i].transform.gameObject.layer != LayerMask.NameToLayer("Projectile"))
            {
                if (hits[i].transform.GetComponent<PlayerManager>() == owner)
                {
                    if (_hasHurt) return false;

                    if (owner.Rb2D.velocity.sqrMagnitude <= 0.1f) return false;

                    if (ClosestAxis(impactedCubeToOwner, true).x == 0)
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
    /// Retourne l'axe (up, down, left, right) le plus proche de la direction donn�e.
    /// </summary>
    /// <param name="direction">La direction que l'on souhaite arrondir � l'axe le plus proche.</param>
    /// <param name="predictUsingOwnerVelocity">Est-ce qu'on utilise la vitesse du joueur pour pr�dire quel sera l'axe le plus proche ?</param>
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
                Debug.DrawRay(owner.transform.position, owner.Rb2D.velocity.normalized * 4f, Color.magenta, 5f);
            }
#endif

            if (owner.Rb2D.velocity == Vector2.zero) { owner.Rb2D.velocity = Vector2.down; }
            Vector2 ownerVelocity = CustomVectors.ClosestAxis(owner.Rb2D.velocity);

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
                if (ownerVelocity == Vector2.left) return Vector2.right;
                else if (ownerVelocity == Vector2.down) return Vector2.up;
            }
            else if (direction == -Vector2.one)
            {
                if (ownerVelocity == Vector2.right) return Vector2.left;
                else if (ownerVelocity == Vector2.up) return Vector2.down;
            }
            else if (direction == new Vector2(1, -1))
            {
                if (ownerVelocity == Vector2.left) return Vector2.right;
                else if (ownerVelocity == Vector2.up) return Vector2.down;
            }
            else if (direction == new Vector2(-1, 1))
            {
                if (ownerVelocity == Vector2.right) return Vector2.left;
                else if (ownerVelocity == Vector2.down) return Vector2.up;
            }

            // C'est pas cense arriver la mais au cas ou je mets ca la
            if (ownerVelocity == Vector2.left || ownerVelocity == Vector2.right) return ownerVelocity == Vector2.left ? Vector2.right : Vector2.left;
            else return ownerVelocity == Vector2.down ? Vector2.up : Vector2.down;
        }
    }
    #endregion
}
