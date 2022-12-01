using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsHandler : MonoBehaviour
{
    #region Autres Scripts
    //==========================================================================

    private PlayerManager _playerManager;
    private PlayerStateSystem _playerSystem;
    private PlayerConfiguration _playerConfig;
    private PlayerInputs controls;
    public bool InputIsEnabled { get; private set; }
    #endregion

    //==========================================================================
    private void Awake()
    {
        _playerSystem = GetComponent<PlayerStateSystem>();
        _playerManager = GetComponent<PlayerManager>();
        InputIsEnabled = true;
    }
    
    public void InitializePlayer(PlayerConfiguration pc)
    {
        _playerConfig = pc;
        //playerMesh.material = pc.PlayerMaterial;
        //_playerManager.imageUI.color = pc.PlayerMaterial.color;
        _playerConfig.Input.onActionTriggered += Input_OnActionTriggered;
    }

    private void Input_OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (!InputIsEnabled) return;
        
        if (obj.action.name == controls.Game.Move.name)
        {
            OnMove(obj);
        }
        else if (obj.action.name == controls.Game.Jump.name)
        {
            OnJump(obj);
        }
        else if (obj.action.name == controls.Game.Shoot.name)
        {
            OnShoot(obj);
        }
        else if (obj.action.name == controls.Game.Eat.name)
        {
            OnEat(obj);
        }
        else if (obj.action.name == controls.Game.Special.name)
        {
            OnSpecial(obj);
        }
    }

    private void OnMove(InputAction.CallbackContext input)
    {
        _playerSystem.PlayerManager.inputVectorDirection = input.ReadValue<Vector2>().normalized;

        if (_playerSystem.PlayerState is not Moving || !InputIsEnabled)
            return;

        _playerSystem.OnMove();
    }

    private void OnJump(InputAction.CallbackContext input)
    {
        if (input.started)
            _playerSystem.OnJump();

        else if (input.performed)
            _playerManager.holdJump = true;

        else if (input.canceled)
            _playerManager.holdJump = false;
    }

    private void OnEat(InputAction.CallbackContext input)
    {
        if (input.started)
            _playerManager.OnEat();

        else if (input.performed)
            _playerManager.holdEat = true;

        else if (input.canceled)
            _playerManager.holdEat = false;
    }

    private void OnShoot(InputAction.CallbackContext input)
    {
        if (_playerManager.fullness < _playerManager.NecessaryFood) return;

        if (input.performed)
        {
            _playerSystem.OnHoldShoot();
        }

        if (!input.canceled) return;
        
        switch (_playerSystem.PlayerState)
        {
            case Moving:
                _playerSystem.PlayerManager.Shoot();
                break;
            case AimShoot:
                _playerSystem.OnShoot();
                break;
        }
    }

    private void OnSpecial(InputAction.CallbackContext input)
    {
        if (_playerManager.fullness < _playerManager.NecessaryFoodSpecial) return;

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