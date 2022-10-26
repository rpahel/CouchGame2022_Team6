using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    #region Variables
    //============================
    private PlayerManager playerManager;
    private PlayerMovement playerMovement;
    private PlayerEat playerEat;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        if (!TryGetComponent<PlayerManager>(out playerManager)) // ça c'est obligé pcq sinon playerManager == null;
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            throw new Exception("No PlayerManager component found in Player game object !");
        }

        playerManager.TryGetPlayerComponent<PlayerMovement>(out playerMovement);
        playerManager.TryGetPlayerComponent<PlayerEat>(out playerEat);
    }
    #endregion

    #region Customs_Functions
    public void OnMove(InputAction.CallbackContext input)
    {
        playerMovement.OnMove(input.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (input.started)
            playerMovement.OnJump();
    }

    public void OnEat(InputAction.CallbackContext input)
    {
        if (input.started)
        {
            playerEat.OnEat(playerManager.AimDirection);
        }
    }

    public void Aim(InputAction.CallbackContext input)
    {
        playerManager.AimDirection = input.ReadValue<Vector2>();
    }
    #endregion
}