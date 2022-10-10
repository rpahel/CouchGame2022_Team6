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
    }
    
    private void OnMove(CallbackContext context)
    {
        if(_mover != null)
            _mover.SetInputVector(context.ReadValue<Vector2>());
    }

    private void OnJump(CallbackContext context)
    {
        if (_mover != null)
            _mover.Jump();
    }

    private void OnShootPress(CallbackContext context)
    {
        
    }
    
    private void OnShootHold(CallbackContext context)
    {
        
    }

}