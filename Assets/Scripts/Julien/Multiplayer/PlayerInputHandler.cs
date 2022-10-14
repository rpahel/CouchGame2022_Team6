using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfiguration _playerConfig;
    [SerializeField] private MeshRenderer playerMesh;

    public float _holdCooldown;
    public bool _canHoldCooldown;

    [Header("Scripts")]
    private Movement _movement;
    private PlayerManager _playerManager;
    private ShootProjectile _shootProjectile;
    private Eat _eat;
    private PlayerControls _controls;
    private void Awake()
    {
        _playerManager = gameObject.GetComponent<PlayerManager>();
        _movement = GetComponent<Movement>();
        _shootProjectile = gameObject.GetComponent<ShootProjectile>();
        _eat = GetComponent<Eat>();
        _controls = new PlayerControls();
    }

    public void InitializePlayer(PlayerConfiguration pc)
    {
        _playerConfig = pc;
        playerMesh.material = pc.PlayerMaterial;
        _playerManager.imageUI.color = pc.PlayerMaterial.color;
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
        else if (obj.action.name == _controls.Gameplay.Dash.name)
        {
            OnDash(obj);
        }
    }
    
    private void OnMove(CallbackContext context)
    {
        if(_playerManager != null)
            _playerManager.SetInputVector(context.ReadValue<Vector2>());
    }

    private void OnJump(CallbackContext context)
    {
        if (_movement != null && context.performed)
            _movement.Jump();
        else if (_movement != null && context.canceled)
            _movement.StopJump();
    }

    private void OnShoot(CallbackContext context)
    {
        if (_shootProjectile != null && context.canceled) 
            _shootProjectile.Shoot();
    }
    
    private void OnAim(CallbackContext context)
    {
        if (_shootProjectile != null && context.performed) 
            _shootProjectile.Aim();
    }
    
    private void OnEat(InputAction.CallbackContext context)
    {
        if (_eat != null && context.performed)
            _eat.TryEat();
        
    }

    private void OnDash(CallbackContext context)
    {
        if (_movement != null && context.started && _playerManager.eatAmount >= 0.70f && _movement._canDash == true)

        {
            Debug.Log("Start");
            _canHoldCooldown = true;
        }
         
        else if (_movement != null && context.canceled && _movement._canDash == true)
        {
            if (_holdCooldown >= 0.01f && _canHoldCooldown && _holdCooldown <= 0.99f)
            {
                dashAfterHold();
                _playerManager.eatAmount -= 0.90f;
            }
            else if (_holdCooldown >= 1f && _canHoldCooldown)
            {
                dashAfterHold();
                 _playerManager.eatAmount -= 0.30f;
            }

            _canHoldCooldown = false;
            _holdCooldown = 0f;
        }
    }

    private void Update()
    {
        if (_canHoldCooldown)
            _holdCooldown += Time.deltaTime;
        //Debug.Log(_holdCooldown);

        if (_holdCooldown >= 2f)
        {
           
            dashAfterHold();
           

        }

    }

    private void dashAfterHold()
    {
        _canHoldCooldown = false;
        Debug.Log("hold");
        _movement.Dash();
        //On appelle la fonction avec la valeu
        _movement.SetHoldValue(_holdCooldown);
        _holdCooldown = 0f;
    }

}