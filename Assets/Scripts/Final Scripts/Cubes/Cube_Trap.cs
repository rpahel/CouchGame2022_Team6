using System.Collections;
using UnityEngine;
using DG.Tweening;
using Data;
public class Cube_Trap : Cube
{
    

    private bool beInTrap;
    public bool stopBeInTrap;
    public float DecreaseEat;
    private PlayerManager playerManager;
    public bool canLooseGraille = true;
    private PlayerMovement movement;
    public PlayerState playerState;
    private Rigidbody2D rb;
   
    public bool canTopCollid;

    public float duration;
    public float strength;
    public int vibrato;
    public float randomess;
    public float baseSpeed;
    public int knockForce = 20;

    [Range(0, 1)] public float takeDmg;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Normal of the first point: " + collision.contacts[0].normal);
        if (collision.contacts[0].normal == new Vector2(0, -1))
        {


            
            if (collision.gameObject.tag == "Player" && canLooseGraille)
            {
               
              
                // beInTrap = true;
                canLooseGraille = false;
                playerManager = collision.gameObject.GetComponent<PlayerManager>();
                movement = collision.gameObject.GetComponent<PlayerMovement>();               
                collision.rigidbody.AddForce((Vector3.up * knockForce) , ForceMode2D.Impulse);
                collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
                transform.DOShakeScale(duration, strength, vibrato, randomess);
                playerManager.eatAmount -= takeDmg;
                playerManager.SetPlayerState(PlayerState.KOCKBACK);                  
                StartCoroutine(timeToLooseEat());
                StartCoroutine(timeToStateMoving());

            }


        }
       
   


        else if (collision.contacts[0].normal == new Vector2(0, 1))
        {



            if (collision.gameObject.tag == "Player" && canLooseGraille)
            {
                
                // beInTrap = true;
                canLooseGraille = false;
                playerManager = collision.gameObject.GetComponent<PlayerManager>();
                movement = collision.gameObject.GetComponent<PlayerMovement>();              
                collision.rigidbody.AddForce(Vector3.down * knockForce / 2, ForceMode2D.Impulse);
                collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
                transform.DOShakeScale(duration, strength, vibrato, randomess);
                playerManager.eatAmount -= takeDmg;
                playerManager.SetPlayerState(PlayerState.KOCKBACK);
                //playerManager.eatAmount -= Time.deltaTime / 3;

                StartCoroutine(timeToLooseEat());
                StartCoroutine(timeToStateMoving());

            }


        }
        else if (collision.contacts[0].normal == new Vector2(1, 0))
        {



            if (collision.gameObject.tag == "Player" && canLooseGraille)
            {
               
                // beInTrap = true;
                canLooseGraille = false;
                playerManager = collision.gameObject.GetComponent<PlayerManager>();
                movement = collision.gameObject.GetComponent<PlayerMovement>();
                collision.rigidbody.velocity = Vector2.zero;
                collision.rigidbody.AddForce(Vector3.left * knockForce , ForceMode2D.Impulse);
                collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
                transform.DOShakeScale(duration, strength, vibrato, randomess);
                playerManager.eatAmount -= takeDmg;
                playerManager.SetPlayerState(PlayerState.KOCKBACK);
                //playerManager.eatAmount -= Time.deltaTime / 3;

                StartCoroutine(timeToLooseEat());
                StartCoroutine(timeToStateMoving());

            }


        }
        else if (collision.contacts[0].normal == new Vector2(-1, 0))
        {



            if (collision.gameObject.tag == "Player" && canLooseGraille)
            {
               
                // beInTrap = true;
                canLooseGraille = false;
                playerManager = collision.gameObject.GetComponent<PlayerManager>();
                movement = collision.gameObject.GetComponent<PlayerMovement>();
                collision.rigidbody.velocity = Vector2.zero;
                collision.rigidbody.AddForce(Vector3.right * knockForce, ForceMode2D.Impulse);
              
                collision.transform.DOShakeScale(duration, strength, vibrato, randomess);
                transform.DOShakeScale(duration, strength, vibrato, randomess);
                playerManager.eatAmount -= takeDmg;
                playerManager.SetPlayerState(PlayerState.KOCKBACK);
                //playerManager.eatAmount -= Time.deltaTime / 3;

                StartCoroutine(timeToLooseEat());

                StartCoroutine(timeToStateMoving());
                
                
            }


        }






    }

    IEnumerator timeToLooseEat()
    {
        yield return new WaitForSeconds(0.01f);
        canLooseGraille = true;
       
    }
    IEnumerator timeToStateMoving()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log(playerState);
        playerManager.SetPlayerState(PlayerState.Moving);
        Debug.Log(playerState);


    }


}
