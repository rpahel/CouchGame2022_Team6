using System.Collections;
using UnityEngine;
using System;
using Data;
using DG.Tweening;
public class AimSpecial : State
{
    private Transform transformPos;
    private PlayerSystemManager playerSystemManager;
        
    public AimSpecial(PlayerSystem playerSystem) : base(playerSystem)
    {
    }
    
    public override void Start()
    {
        playerSystem.PlayerSystemManager.AimPivot.gameObject.SetActive(true);
        playerSystem.PlayerSystemManager.charge = 0;
            
        transformPos = playerSystem.transform;
        playerSystemManager = playerSystem.PlayerSystemManager;
    }

    public override void Update()
    {
        playerSystem.PlayerSystemManager.charge = Mathf.Clamp01(playerSystem.PlayerSystemManager.charge + Time.deltaTime / playerSystem.PlayerSystemManager.TimeToMaxCharge);
#if UNITY_EDITOR
        { 
            Debug.DrawRay((Vector2)transformPos.position + playerSystemManager.PCollider.bounds.extents.y * Vector2.down,     (playerSystemManager.MinDistance + playerSystemManager.charge * (playerSystemManager.MaxDistance - playerSystemManager.MinDistance)) * (playerSystemManager.inputVectorDirection != Vector2.zero ? playerSystemManager.inputVectorDirection : playerSystemManager.LookDirection), Color.magenta);
            Debug.DrawRay((Vector2)transformPos.position + playerSystemManager.PCollider.bounds.extents.x * Vector2.right,    (playerSystemManager.MinDistance + playerSystemManager.charge * (playerSystemManager.MaxDistance - playerSystemManager.MinDistance)) * (playerSystemManager.inputVectorDirection != Vector2.zero ? playerSystemManager.inputVectorDirection : playerSystemManager.LookDirection), Color.magenta);
            Debug.DrawRay((Vector2)transformPos.position + playerSystemManager.PCollider.bounds.extents.x * Vector2.left,     (playerSystemManager.MinDistance + playerSystemManager.charge * (playerSystemManager.MaxDistance - playerSystemManager.MinDistance)) * (playerSystemManager.inputVectorDirection != Vector2.zero ? playerSystemManager.inputVectorDirection : playerSystemManager.LookDirection), Color.magenta);
            Debug.DrawRay((Vector2)transformPos.position + playerSystemManager.PCollider.bounds.extents.y * Vector2.up,       (playerSystemManager.MinDistance + playerSystemManager.charge * (playerSystemManager.MaxDistance - playerSystemManager.MinDistance)) * (playerSystemManager.inputVectorDirection != Vector2.zero ? playerSystemManager.inputVectorDirection : playerSystemManager.LookDirection), Color.magenta);
        }
#endif
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(playerSystemManager.MaxDistance < playerSystemManager.MinDistance) playerSystemManager.maxDistance = playerSystemManager.MinDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transformPos.position, playerSystemManager.MinDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transformPos.position, playerSystemManager.MaxDistance);
    }
#endif
    

    public override void FixedUpdate()
    {
        playerSystem.PlayerSystemManager.AimPivot.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, playerSystem.PlayerSystemManager.inputVectorDirection != Vector2.zero ? playerSystem.PlayerSystemManager.inputVectorDirection : playerSystem.PlayerSystemManager.LookDirection) - 90f);
        playerSystem.CooldownManager.StartCoroutine(playerSystem.CooldownManager.Braking()); // TODO : Attention au probleme des coroutines
    }

    public override void OnSpecial()
    {
        playerSystem.PlayerSystemManager.AimPivot.gameObject.SetActive(false);
        playerSystem.SetState(new Special(playerSystem));
    }
}


