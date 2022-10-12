using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfiguration _playerConfig;
    private Mover _mover;

    [SerializeField] private MeshRenderer playerMesh;

    private PlayerControls _controls;
    private void Awake()
    {
        _mover = GetComponent<Mover>();
        _controls = new PlayerControls();
    }

    public void InitializePlayer(PlayerConfiguration pc)
    {
        _playerConfig = pc;
        playerMesh.material = pc.PlayerMaterial;
        _playerConfig.Input.onActionTriggered += Input_OnActionTriggered;
    }

    private void Input_OnActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == _controls.Gameplay.Move.name)
        {
            OnMove(obj);
        }
        else if (obj.action.name == _controls.Gameplay.Jump.name)
        {
            OnJump(obj);
        }
        else if (obj.action.name == _controls.Gameplay.Shoot.name)
        {
            OnShoot(obj);
        }
        else if (obj.action.name == _controls.Gameplay.Aim.name)
        {
            OnAim(obj);
        }
        else if (obj.action.name == _controls.Gameplay.Eat.name)
        {
            OnEat(obj);
        }
            
    }
    
    private void OnMove(CallbackContext context)
    {
        if(_mover != null)
            _mover.SetInputVector(context.ReadValue<Vector2>());
    }

    private void OnJump(CallbackContext context)
    {
        if (_mover != null && context.performed)
            _mover.Jump();
        else if (_mover != null && context.canceled)
            _mover.StopJump();
    }

    private void OnShoot(CallbackContext context)
    {
        if (_mover != null && context.performed)
            _mover.Shoot();
    }
    
    private void OnAim(CallbackContext context)
    {
        if (_mover != null && context.performed)
            _mover.Aim();
        
    }
    
    private void OnEat(InputAction.CallbackContext context)
    {
        if (_mover != null && context.performed)
        {
            _mover.TryEat();
        }
    }

}