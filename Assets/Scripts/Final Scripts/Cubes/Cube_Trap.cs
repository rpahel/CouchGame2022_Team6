using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using DG.Tweening;
using Data;
using Debug = UnityEngine.Debug;

public class Cube_Trap : Cube
{
    

    private bool beInTrap;
    public bool stopBeInTrap;
    public float DecreaseEat;
    private PlayerManager playerManager;
    public bool canLooseGraille = true;
    private PlayerMovement movement;
    private Rigidbody2D rb;
   
    public bool canTopCollid;

    public float duration;
    public float strength;
    public int vibrato;
    public float randomess;
    public float baseSpeed;
    public int knockForce = 20;

    [SerializeField] private float errorBounds = 0.3f;

    [Range(0, 1)] public float takeDmg;


    private void Update()
    {
        if(playerManager != null)
            Debug.Log(playerManager.State);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*Debug.Log("Normal of the first point: " + collision.contacts[0].normal);
        if (collision.contacts[0].normal == new Vector2(0, -1))
        {
            if (!collision.gameObject.CompareTag("Player") || !canLooseGraille) return;

            Debug.Log("up");
            rb = collision.gameObject.GetComponent<Rigidbody2D>();
            // beInTrap = true;
            playerManager = collision.gameObject.GetComponent<PlayerManager>();
            playerManager.SetPlayerState(PlayerState.KNOCKBACKED); 
            canLooseGraille = false;
            movement = collision.gameObject.GetComponent<PlayerMovement>();               
            collision.rigidbody.AddForce((Vector3.up * knockForce) , ForceMode2D.Impulse);
            collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
            transform.DOShakeScale(duration, strength, vibrato, randomess);
            playerManager.eatAmount -= takeDmg;
                                 
            //StartCoroutine(timeToLooseEat());
            StartCoroutine(timeToStateMoving());


        }*/
        
        if (!collision.gameObject.CompareTag("Player") || !canLooseGraille) return;

        var vec = collision.contacts[0].normal;
        KnockBack(new Vector2(-vec.x, -vec.y), collision);

        /*else if (collision.contacts[0].normal == new Vector2(0, 1))
        {
            if (!collision.gameObject.CompareTag("Player") || !canLooseGraille) return;
            
            Debug.Log("down");
            rb = collision.gameObject.GetComponent<Rigidbody2D>();
            // beInTrap = true;
            canLooseGraille = false;
            playerManager = collision.gameObject.GetComponent<PlayerManager>();
            movement = collision.gameObject.GetComponent<PlayerMovement>();              
            collision.rigidbody.AddForce(Vector3.down * knockForce / 2, ForceMode2D.Impulse);
            collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
            transform.DOShakeScale(duration, strength, vibrato, randomess);
            playerManager.eatAmount -= takeDmg;
            playerManager.SetPlayerState(PlayerState.KNOCKBACKED);
            //playerManager.eatAmount -= Time.deltaTime / 3;

            //StartCoroutine(timeToLooseEat());
            StartCoroutine(timeToStateMoving());


        }
        else if (collision.contacts[0].normal == new Vector2(1, 0))
        {
            if (!collision.gameObject.CompareTag("Player") || !canLooseGraille) return;
            
            Debug.Log("right");
            KnockBack(Vector3.left, collision);


        }
        //else if (collision.contacts[0].normal == new Vector2(-1, 0))
        
        else if (collision.contacts[0].normal == new Vector2(0, 1))
        {
            if (!collision.gameObject.CompareTag("Player") || !canLooseGraille) return;
            
            Debug.Log("left");
            KnockBack(Vector3.right, collision);

        }*/

        [ContextMenu("KnockbackLeft")]
        void KnockbackLeft()
        {
            KnockBack(Vector3.right, collision);
        }
        
        [ContextMenu("KnockbackRight")]
        void KnockbackRight()
        {
            KnockBack(Vector3.left, collision);
        }
        





    }

    IEnumerator timeToLooseEat()
    {
        yield return new WaitForSeconds(0.01f);
        canLooseGraille = true;
       
    }
    IEnumerator timeToStateMoving()
    {
        yield return new WaitForSeconds(2f);
        playerManager.SetPlayerState(PlayerState.Moving);
        canLooseGraille = true;

    }

    private bool CheckContact(Vector2 contact, Vector2 dir)
    {
        return contact.x - dir.x < errorBounds || contact.x - dir.x > errorBounds && contact.y - dir.y < errorBounds ||
               contact.y - dir.y > errorBounds;
    }

    private void KnockBack(Vector3 vec, Collision2D collision)
    {
        rb = collision.gameObject.GetComponent<Rigidbody2D>();
        // beInTrap = true;
        canLooseGraille = false;
        playerManager = collision.gameObject.GetComponent<PlayerManager>();
        playerManager.SetPlayerState(PlayerState.KNOCKBACKED);
        playerManager.DisableInputs();
        movement = collision.gameObject.GetComponent<PlayerMovement>();
        collision.rigidbody.velocity = Vector2.zero;
        collision.rigidbody.AddForce(vec * knockForce , ForceMode2D.Impulse);
              
        collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
        transform.DOShakeScale(duration, strength, vibrato, randomess);
        playerManager.eatAmount -= takeDmg;
                
        //playerManager.eatAmount -= Time.deltaTime / 3;

        //StartCoroutine(timeToLooseEat());

        StartCoroutine(timeToStateMoving());
        playerManager.EnableInputs();
    }
    
    private void OnGUI()
    {
        if(rb != null)
            GUILayout.Label("Velocity = " + rb.velocity);
    }

}
