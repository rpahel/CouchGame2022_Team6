using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using Data;


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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != owner.gameObject && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector2 sensDuKnockBack = (collision.transform.position - transform.position).x > 0 ? new Vector2(1, 1f) : new Vector2(-1, 1f);
            sensDuKnockBack.Normalize();
            collision.GetComponent<PlayerManager>().OnDamage(owner, pourcentageInflige, sensDuKnockBack * knockBackForce);
            rb.velocity = new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y) * forceDuRebond;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible") || collision.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
        {
            gameObject.SetActive(false);
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
                rb.velocity = new Vector2(-sensDuKnockBack.x, sensDuKnockBack.y) * currentVelocity.magnitude * forceDuRebond;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
                rb.velocity = sensDuRebond * currentVelocity.magnitude * forceDuRebond;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible") || collision.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
            {
                SpawnCube(collision);
                gameObject.SetActive(false);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                Vector2 sensDuRebond = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal).normalized;
                rb.velocity = sensDuRebond * currentVelocity.magnitude * forceDuRebond;
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

    private Vector2 PositionInNormalDirection(Vector2 originalPos, Vector2 normal)
    {
        const float cos30 = 0.866f;

        if (normal.y > cos30 && (normal.x > -.5f && normal.x < .5f))                             
            return originalPos + Vector2.up;            // N
        else if ((normal.x > .5f && normal.x < cos30) && (normal.y > .5f && normal.y < cos30))      
            return originalPos + Vector2.one;           // NE
        else if (normal.x > cos30 && (normal.y > -.5f && normal.y < .5f))                       
            return originalPos + Vector2.right;         // E
        else if ((normal.x > .5f && normal.x < cos30) && (normal.y < -.5f && normal.y > -cos30))    
            return originalPos + new Vector2(1, -1);    // SE
        else if (normal.y < -cos30 && (normal.x > -.5f && normal.x < .5f))                      
            return originalPos + Vector2.down;          // S
        else if ((normal.x > -cos30 && normal.x < -.5f) && (normal.y < -.5f && normal.y > -cos30))  
            return originalPos - Vector2.one;           // SW
        else if (normal.x < -cos30 && (normal.y > -.5f && normal.y < .5f))                      
            return originalPos + Vector2.left;          // W
        else if ((normal.x > -cos30 && normal.x < -.5f) && (normal.y > .5f && normal.y < cos30))    
            return originalPos + new Vector2(-1, 1);    // NW
        
        Debug.Log("Tu ne devrais pas voir ça.");
        return originalPos + Vector2.up;                // N
    }
    #endregion
}
