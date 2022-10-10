using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetpackController : MonoBehaviour {

    private Rigidbody rb;

    private float jumpCount;
    
    public float jumpForce;
    public int maxJumpCount;

    public float wjForce;
    private bool canWallJump;
    private Vector3 normalVec;
    
    
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        Vector3 jump = new Vector3(Input.GetAxis("Horizontal") * 10, 0, Input.GetAxis(("Vertical")) * 10) * Time.deltaTime;
        transform.Translate(jump.x,0,jump.z);
    }

    public void OnJump(InputAction.CallbackContext e) {
        if (e.started /*&& jumpCount < maxJumpCount*/) {
            if (canWallJump) {
                if (!IsGrounded()) {
                    Vector3 wjForceVec = normalVec * wjForce;
                    wjForceVec.y = jumpForce;
                    rb.AddForce(wjForceVec,ForceMode.Impulse);
                }
                else {
                    jumpCount++;
                    rb.AddForce(jumpForce * Vector3.up,ForceMode.Impulse);
                }
            }
            else {
                jumpCount++;
                rb.AddForce(jumpForce * Vector3.up,ForceMode.Impulse);
            }
        }

    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag.Contains("Jumpable")) {
            canWallJump = true;
            normalVec = collision.contacts[0].normal;
        }
    }

    public void OnCollisionExit(Collision collision) {
        if (collision.collider.tag.Contains("Jumpable"))
            canWallJump = false;
    }

    public bool IsGrounded() {
        RaycastHit hit;
        return Physics.Raycast(transform.position, -Vector3.up, out hit,2f,1 << 6);
    }

}
