using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    #region Autres Scripts
    //==========================================================================

    private PlayerManager _playerManager;
    private PlayerStateSystem _playerSystem;
    public bool InputIsEnabled { get; private set; }
    #endregion

    //#region Customs_Functions
    //==========================================================================
    private void Awake()
    {
        _playerSystem = GetComponent<PlayerStateSystem>();
        _playerManager = GetComponent<PlayerManager>();
        InputIsEnabled = true;
    }

    public void OnMove(InputAction.CallbackContext input)
    {
        _playerSystem.PlayerManager.inputVectorDirection = input.ReadValue<Vector2>().normalized;

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
            _playerManager.holdJump = true;

        else if (input.canceled)
            _playerManager.holdJump = false;
    }

    public void OnEat(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (input.started)
            _playerManager.OnEat();

        else if (input.performed)
            _playerManager.holdEat = true;

        else if (input.canceled)
            _playerManager.holdEat = false;
    }

    public void OnShoot(InputAction.CallbackContext input)
    {
        if (!InputIsEnabled) return;

        if (_playerManager.fullness < _playerManager.NecessaryFood) return;

        if (input.performed)
        {
            _playerSystem.OnHoldShoot();
        }

        if (input.canceled)
        {
            if (_playerSystem.PlayerState is Moving)
                _playerSystem.PlayerManager.Shoot();
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