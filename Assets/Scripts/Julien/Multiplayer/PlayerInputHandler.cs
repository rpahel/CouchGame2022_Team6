using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Mover _mover;
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        var movers = FindObjectsOfType<Mover>();
        var index = _playerInput.playerIndex;
        _mover = movers.FirstOrDefault(m => m.GetPlayerIndex() == index);
    }
    
    public void OnMove(CallbackContext context)
    {
        if(_mover != null)
            _mover.SetInputVector(context.ReadValue<Vector2>());
    }

}