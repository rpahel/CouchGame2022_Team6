using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private PlayerManager playerSystemManager;
    private PlayerStateSystem playerSystem;
    private FaceManager faceManager;
    private TrailRenderer _trailRenderer;
    private Coroutine faceCoroutine;
    
    private void Awake()
    {
        playerSystemManager = GetComponent<PlayerManager>();
        playerSystem = GetComponent<PlayerStateSystem>();
        faceManager = GetComponent<FaceManager>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public IEnumerator CooldownShoot()
    {
        playerSystemManager.canShoot = false;
        yield return new WaitForSeconds(playerSystemManager.CooldownShoot);
        playerSystemManager.canShoot = true;
    }

    public IEnumerator Braking()
    {
        float iniVelocityX = playerSystem.PlayerManager.Rb2D.velocity.x;
        float t = 0;
        while (t < 1f)
        {

            playerSystem.PlayerManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), playerSystem.PlayerManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / playerSystem.PlayerManager.StopDuration;
            yield return new WaitForFixedUpdate();
        }
        
        playerSystem.PlayerManager.brakingCoroutine = null;
    }

    public IEnumerator MoverOverAnimation(Vector2 endPosition)
    {
        Vector2 iniPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            if (playerSystem.PlayerState is Knockback)
                break;

            playerSystem.PlayerManager.Rb2D.velocity = Vector2.zero;
            transform.position = DOVirtual.EasedValue(iniPos, endPosition, t, Ease.OutBack, 2f);
            t += Time.deltaTime * 4f;
            yield return null;
        }

        if (playerSystem.PlayerState is not Knockback)
        {
            transform.position = endPosition;
        }
    }

    //public IEnumerator ActivateInputDelay(float duration)
    //{
    //    //playerSystem.PlayerInputs.SetEnableInput(false);
    //    yield return new WaitForSeconds(duration);
    //    //playerSystem.PlayerInputs.SetEnableInput(true);
    //    playerSystem.SetState(new Moving(playerSystem));
    //}

    public IEnumerator JumpCoroutine()
    {
        playerSystemManager.isJumping = true;
        playerSystemManager.SetupJumpEffect();
        playerSystem.PlaySound("Player_Jump");
        SetupCoroutine(faceManager.FaceJump);
        float t = 0;
        playerSystemManager.Rb2D.gravityScale = 0;
        while (t < 1)
        {
            if (playerSystemManager.CheckHeadBonk())
            {
                break;
            }

            if (playerSystem.PlayerState is not Moving && playerSystem.PlayerState is not AimShoot)
            {
                break;
            }

            playerSystemManager.Rb2D.velocity = new Vector2(playerSystemManager.Rb2D.velocity.x, Mathf.LerpUnclamped(playerSystemManager.JumpForce, 0, playerSystemManager.JumpCurve.Evaluate(t)));
            t += Time.fixedDeltaTime / playerSystemManager.JumpDuration;
            yield return new WaitForFixedUpdate();
        }
        playerSystemManager.isJumping = false;
        playerSystemManager.Rb2D.gravityScale = playerSystemManager.GravityScale;
        playerSystemManager.JumpCoroutine = null;
        yield break;
    }

    public void SetDashCoroutine()
    {
        StartCoroutine(DashCoroutine(playerSystemManager.charge + .1f));
    }

    private IEnumerator DashCoroutine(float dashDuration)
    {
        playerSystemManager.canDash = false;
        SetupCoroutine(faceManager.FaceDash);
        gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
        playerSystemManager.SpecialTrigger.SetActive(true);
        var originalGravityScale = playerSystemManager.Rb2D.gravityScale;
        playerSystemManager.Rb2D.gravityScale = 0;
        playerSystemManager.Rb2D.velocity = Vector2.zero;

        float t = 0;
        while (t < dashDuration)
        {
            if (playerSystem.PlayerState is not Special)
            {
                StartCoroutine(UndoDash(originalGravityScale));
                yield break;
            }

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        playerSystem.SetState(new Moving(playerSystem));
        if (playerSystem.PlayerState is Dead)
            UndoDashButImDead(originalGravityScale);
        else
            StartCoroutine(UndoDash(originalGravityScale));
    }

    private IEnumerator UndoDash(float originalGravityScale)
    {
        playerSystemManager.Rb2D.gravityScale = originalGravityScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        playerSystemManager.SpecialTrigger.SetActive(false);
        yield return new WaitForSeconds(playerSystemManager.DashCooldown);
        playerSystemManager.canDash = true;
    }

    private void UndoDashButImDead(float originalGravityScale)
    {
        playerSystemManager.Rb2D.gravityScale = originalGravityScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        playerSystemManager.SpecialTrigger.SetActive(false);
        playerSystemManager.canDash = true;
    }

    public void SetupCoroutine(Sprite sprite)
    {
        if (faceCoroutine == null)
        {
            faceCoroutine = StartCoroutine(FaceCoroutine(sprite));
        }
        else
        {
            StopCoroutine(faceCoroutine);
            faceCoroutine = StartCoroutine(FaceCoroutine(sprite));
        }
    }

    private IEnumerator FaceCoroutine(Sprite sprite)
    {
        faceManager.SetFace(sprite);
        yield return new WaitForSeconds(faceManager.CooldownFace);
        faceManager.ResetFace();
    }
}