using System;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsState : MonoBehaviour
{
    #region Autres Scripts
    //==========================================================================
    
    private PlayerSystemManager _playerSystemManager;
    private PlayerSystem _playerSystem;
    #endregion

    //#region Customs_Functions
    //==========================================================================
    private void Awake()
    {
        _playerSystem = GetComponent<PlayerSystem>();
        _playerSystemManager = GetComponent<PlayerSystemManager>();
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        _playerSystem.PlayerSystemManager.inputVectorDirection = input.ReadValue<Vector2>().normalized;
        
        if (_playerSystem.PlayerState is not Moving)
            return;

        _playerSystem.OnMove();
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (input.started)
            _playerSystem.OnJump();

        else if (input.performed)
            _playerSystemManager.HoldJump = true;

        else if (input.canceled)
            _playerSystemManager.HoldJump = false;
    }

    public void OnEat(InputAction.CallbackContext input)
    {
        if (input.started)
            _playerSystem.OnEat(); //Was Aim Direction

        else if (input.performed)
            _playerSystemManager.HoldEat = true;

        else if (input.canceled)
            _playerSystemManager.HoldEat = false;
    }

    public void OnShoot(InputAction.CallbackContext input)
    {
        if (input.started)
        {
            _playerSystem.OnHoldSHoot();
        }

        if (input.canceled)
        {
            _playerSystem.OnShoot();
        }
    }
    

    /*public void Special(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            PManager.PSpecial.Charge(true);
        }

        if (input.canceled)
        {
            PManager.PSpecial.UseSpecial();
            PManager.PSpecial.Charge(false);
        }
    }
    #endregion*/
}