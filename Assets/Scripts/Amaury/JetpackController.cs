using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetpackController : MonoBehaviour {

    private CharacterController controller;

    private float verticalVelocity;
    private float jumpCount;
    private bool startJetpack;
    private float jetpackTimer;
    private bool isJetpacking;
    
    public float gravity; // The gravity apply on the player
    public float jumpForce;
    public float jetpackForce;
    public int maxJumpCount;
    
    
    
    private void Start() {
        controller = GetComponent<CharacterController>();
    }

    private void Update() {
        if (startJetpack) {
            jetpackTimer += Time.deltaTime;
            
            if (jetpackTimer > 0.2f) {
                startJetpack = false;
                isJetpacking = true;
                jetpackTimer = 0f;
            }
        }

        if (isJetpacking)
        {
            verticalVelocity += jetpackForce * Time.deltaTime;
            Debug.Log("is jetpacking");
        }
        else
        {
            if (controller.isGrounded)
            {
                verticalVelocity = -gravity * Time.deltaTime;
                jumpCount = 0;
            }
            else
                verticalVelocity -= gravity * Time.deltaTime;
        }


        Vector3 jump = new Vector3(Input.GetAxis("Horizontal") * 10, verticalVelocity, 0);
        controller.Move(jump * Time.deltaTime);
        
        transform.GetChild(0).gameObject.SetActive(isJetpacking);
    }

    public void OnJump(InputAction.CallbackContext e) {
        if (e.started && jumpCount < maxJumpCount) {
            verticalVelocity = jumpForce;
            jumpCount++;
        }

    }

    public void OnJetpack(InputAction.CallbackContext e) {
        if (e.started)
        {
            Debug.Log("start jetpack");
            startJetpack = true;
        }
        else if (e.canceled) {
            isJetpacking = false;
            startJetpack = false;
            jetpackTimer = 0f;
        }
    }
}
