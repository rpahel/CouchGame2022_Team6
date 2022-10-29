using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables
    //=============================================
    [HideInInspector] public GameObject owner;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;

    //=============================================
    private Color color;
    public Color PrColor { set => color = value; }

    private float gravity;
    public float Gravity { set => gravity = value; }

    private float forceDuRebond;
    public float ForceDuRebond { set => forceDuRebond = value; }
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
        spriteRenderer.color = color;
        rb.gravityScale = gravity;
    }

    private void OnDisable()
    {
        spriteRenderer.color = Color.white;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        col.isTrigger = true;
        transform.position = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != owner)
        {
            // On check is c'est un player ou cube et accorde en fonction
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
        {
            col.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != owner)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("Joueur touché !");
            }
            else if(collision.gameObject.layer == LayerMask.NameToLayer("Indestructible"))
            {
                Debug.Log("Indestructible touché !");
            }
            else if(collision.gameObject.layer == LayerMask.NameToLayer("Destructible"))
            {
                Debug.Log("Destructible touché !");
            }
        }
    }
    #endregion

    #region Custom_Functions
    //=============================================
    public void Shoot(Vector2 dir, float speed)
    {
        rb.velocity = dir * speed;
    }
    #endregion
}
