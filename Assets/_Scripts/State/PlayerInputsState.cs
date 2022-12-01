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
    public bool InputIsEnabled { get; private set;}
    #endregion

    //#region Customs_Functions
    //==========================================================================
    private void Awake()
    {
        _playerSystem = GetComponent<PlayerSystem>();
        _playerSystemManager = GetComponent<PlayerSystemManager>();
        InputIsEnabled = true;
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        _playerSystem.PlayerSystemManager.inputVectorDirection = input.ReadValue<Vector2>().normalized;
        
        if (_playerSystem.PlayerState is not Moving || !InputIsEnabled)
            return;

        _playerSystem.OnMove();
    }

    public void OnJump(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (input.started)
            _playerSystem.OnJump();

        else if (input.performed)
            _playerSystemManager.HoldJump = true;

        else if (input.canceled)
            _playerSystemManager.HoldJump = false;
    }

    public void OnEat(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (input.started)
            _playerSystem.OnEat(); //Was Aim Direction

        else if (input.performed)
            _playerSystemManager.HoldEat = true;

        else if (input.canceled)
            _playerSystemManager.HoldEat = false;
    }

    public void OnShoot(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (_playerSystemManager.fullness < _playerSystemManager.NecessaryFoodShoot) return;

        if (input.started)
        {
            _playerSystem.OnHoldSHoot();
        }

        if (input.canceled)
        {
            _playerSystem.OnShoot();
        }
    }

    public void Special(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (_playerSystemManager.fullness < _playerSystemManager.NecessaryFoodSpecial) return;
        
        if (input.performed)
        {
            _playerSystem.OnHoldSpecial();
        }

        if (input.canceled)
        {
            _playerSystem.OnSpecial();
        }
    }
    
    public void SetEnableInput(bool result)
    {
        InputIsEnabled = result;
    }

}