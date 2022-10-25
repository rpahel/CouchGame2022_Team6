using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    #region Variables
    //============================
    private PlayerMovement playerMovement;
    #endregion

    #region Unity_Functions
    //============================
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
    #endregion

    #region Customs_Functions
    //============================
    public void OnMove(InputAction.CallbackContext input)
    {
        Debug.Log(input.ReadValue<Vector2>());
        playerMovement.OnMove(input.ReadValue<Vector2>());
    }

    //============================
    public void OnJump(InputAction.CallbackContext input)
    {
        //if (input.started)
        //    Debug.Log("Jump pressed.");
        //if (input.performed)
        //    Debug.Log("Jump button held.");
        //if (input.canceled)
        //    Debug.Log("Jump button released.");
    }
    #endregion
}