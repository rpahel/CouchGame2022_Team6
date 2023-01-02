using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour {

    private Vector2 navigateVec;

    [SerializeField] private float cursorSpeed;
    
    private void Start() {
      //  Cursor.visible = false;
    }

    void Update() {
        Vector2 inputPos = Input.mousePosition;
        inputPos.y -= 30;
        inputPos.x += 15;
        //transform.position = inputPos;

        if (navigateVec != Vector2.zero) {
            Vector2 nav = navigateVec * cursorSpeed;
            transform.Translate(nav.x,nav.y,0);
        }
    }

    public void OnNavigate(InputAction.CallbackContext e) {
        navigateVec = e.ReadValue<Vector2>();
    }
}
