using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //============================
    private PlayerManager playerManager;

    //============================
    [Header("Données")]
    [SerializeField, Range(0, 40f)] private float vitesseMax;
    [SerializeField, Range(.1f, 2f)] private float dureeAvantArret;
    //[SerializeField, Range(0, 100f)] private float forceDeSaut;

    //============================
    private Rigidbody2D rb2d;
    private Vector2 inputVector = Vector2.zero;
    private Coroutine freinage;
    #endregion

    #region Unity_Functions
    //============================
    private void Awake()
    {
        dureeAvantArret = dureeAvantArret < 0.1f ? 0.1f : dureeAvantArret;

        if (!TryGetComponent<PlayerManager>(out playerManager))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerManager component found in Player game object !");
        }

        if (!TryGetComponent<Rigidbody2D>(out rb2d))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No Rigidbody2D component found in Player game object !");
        }
    }

    private void Update()
    {
        if(dureeAvantArret < 0.1f)
            dureeAvantArret = 0.1f;
    }

    private void FixedUpdate()
    {
        if(playerManager.PlayerState == PLAYER_STATE.WALKING)
        {
            if (inputVector == Vector2.zero)
            {
                if (freinage == null && rb2d.velocity.x != 0)
                {
                    freinage = StartCoroutine(Freinage());
                }
            }

            if ((inputVector.x / Mathf.Abs(inputVector.x)) + (rb2d.velocity.x / Mathf.Abs(rb2d.velocity.x)) == 0)
                rb2d.velocity = new Vector2(-rb2d.velocity.x, rb2d.velocity.y);

            rb2d.velocity += new Vector2(inputVector.x, 0) * Time.fixedDeltaTime * 100f;
            rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -vitesseMax, vitesseMax), rb2d.velocity.y);
        }
    }
    #endregion

    #region Custom_Functions
    //============================
    public void OnMove(Vector2 input)
    {
        inputVector = input;

        if (inputVector == Vector2.zero)
            return;

        if(freinage != null)
        {
            StopCoroutine(freinage);
            freinage = null;
        }

        playerManager.PlayerState = PLAYER_STATE.WALKING;
    }

    //============================
    IEnumerator Freinage()
    {
        float iniVelocityX = rb2d.velocity.x;
        float t = 0;
        while(t < 1f)
        {
            rb2d.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), rb2d.velocity.y);
            t += Time.fixedDeltaTime / dureeAvantArret;
            yield return new WaitForFixedUpdate();
        }
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        freinage = null;
    }
    #endregion
}
