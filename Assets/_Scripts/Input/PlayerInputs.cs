using Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    #region Autres Scripts
    //==========================================================================
    public PlayerManager PManager { get; set; }
    #endregion

    #region Customs_Functions
    //==========================================================================
    public void OnMove(InputAction.CallbackContext input)
    {
        PManager.PMovement.OnMove(input.ReadValue<Vector2>().normalized);
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

    public void OnShoot(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            PManager.PShoot.HoldShoot();
        }

        if (input.canceled)
        {
            PManager.PShoot.OnShoot(PManager.AimDirection);
        }
    }

    public void Aim(InputAction.CallbackContext input)
    {
        PManager.AimDirection = input.ReadValue<Vector2>().normalized;
    }

    public void Special(InputAction.CallbackContext input)
    {
        if (PManager.PlayerState == PLAYER_STATE.STUNNED)
            return;

        if (input.performed)
        {
            PManager.PSpecial.Charge(true);
        }

        if (input.canceled)
        {
            PManager.PSpecial.Charge(false);
        }
    }
    #endregion
}