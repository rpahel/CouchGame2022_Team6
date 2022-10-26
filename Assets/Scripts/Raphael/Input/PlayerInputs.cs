using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    #region Autres Scripts
    //============================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Unity_Functions
    #endregion

    #region Customs_Functions
    public void OnMove(InputAction.CallbackContext input)
    {
        PManager.PMovement.OnMove(input.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (input.started)
            PManager.PMovement.OnJump();

        else if (input.performed)
            PManager.PMovement.HoldJump = true;

        else if (input.canceled)
            PManager.PMovement.HoldJump = false;
    }

    public void OnEat(InputAction.CallbackContext input)
    {
        if (input.started)
            PManager.PEat.OnEat(PManager.AimDirection);

        else if (input.performed)
            PManager.PEat.HoldEat = true;

        else if (input.canceled)
            PManager.PEat.HoldEat = false;
    }

    public void Aim(InputAction.CallbackContext input)
    {
        PManager.AimDirection = input.ReadValue<Vector2>();
    }
    #endregion
}