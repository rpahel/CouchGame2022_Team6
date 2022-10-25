using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    #region Variables
    //============================
    private PlayerMovement playerMovement;
    private PlayerEat playerEat;
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

        if (!TryGetComponent<PlayerEat>(out playerEat))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerEat component found in Player game object !");
        }
    }
    #endregion

    #region Customs_Functions
    //============================
    public void OnMove(InputAction.CallbackContext input)
    {
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

    //============================
    public void OnEat(InputAction.CallbackContext input)
    {
        if (input.started)
        {
            Debug.Log("Eat pressed.");
            playerEat.OnEat(playerMovement.InputVector);
        }

        if (input.performed)
            Debug.Log("Eat button held.");

        if (input.canceled)
            Debug.Log("Eat button released.");
    }
    #endregion
}