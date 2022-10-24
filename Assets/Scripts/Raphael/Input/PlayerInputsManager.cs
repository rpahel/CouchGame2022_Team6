using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (!TryGetComponent<PlayerMovement>(out playerMovement))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerMovement component found in Player game object !");
        }
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        playerMovement.OnMove(input.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        //if (input.started)
        //    Debug.Log("Jump pressed.");
        //if (input.performed)
        //    Debug.Log("Jump button held.");
        //if (input.canceled)
        //    Debug.Log("Jump button released.");
    }

}