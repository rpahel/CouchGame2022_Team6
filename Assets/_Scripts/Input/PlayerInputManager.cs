using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region Autres Scripts
    //==========================================================================

    private PlayerManager _playerSystemManager;
    private PlayerStateSystem _playerSystem;
    public bool InputIsEnabled { get; private set;}
    #endregion

    //#region Customs_Functions
    //==========================================================================
    private void Awake()
    {
        _playerSystem = GetComponent<PlayerStateSystem>();
        _playerSystemManager = GetComponent<PlayerManager>();
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
            _playerSystemManager.holdJump = true;

        else if (input.canceled)
            _playerSystemManager.holdJump = false;
    }

    public void OnEat(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (input.started)
            _playerSystemManager.OnEat();

        else if (input.performed)
            _playerSystemManager.holdEat = true;

        else if (input.canceled)
            _playerSystemManager.holdEat = false;
    }

    public void OnShoot(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (_playerSystemManager.fullness < _playerSystemManager.NecessaryFood) return;

        if (input.performed)
        {
            _playerSystem.OnHoldShoot();
        }

        if (input.canceled)
        {
            if (_playerSystem.PlayerState is Moving)
                _playerSystem.PlayerSystemManager.Shoot();
            else if (_playerSystem.PlayerState is Aim)
                _playerSystem.OnShoot();
        }
    }
    public void SetEnableInput(bool result)
    {
        InputIsEnabled = result;
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