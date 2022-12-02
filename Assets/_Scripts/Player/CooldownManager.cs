using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private PlayerManager playerSystemManager;
    private PlayerStateSystem playerSystem;
    private FaceManager faceManager;

    private Coroutine faceCoroutine;
    private void Awake()
    {
        playerSystemManager = GetComponent<PlayerManager>();
        playerSystem = GetComponent<PlayerStateSystem>();
        faceManager = GetComponent<FaceManager>();
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

        //if (playerSystem.PlayerSystemManager.PlayerState != PLAYER_STATE.KNOCKBACKED && playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.DASHING) //A quoi sert ca
        // playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(0, playerSystem.PlayerSystemManager.Rb2D.velocity.y);

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

    public IEnumerator ActivateInputDelay(float duration)
    {
        //playerSystem.PlayerInputs.SetEnableInput(false);
        yield return new WaitForSeconds(duration);
        //playerSystem.PlayerInputs.SetEnableInput(true);
        playerSystem.SetState(new Moving(playerSystem));
    }

    public IEnumerator JumpCoroutine()
    {
        playerSystemManager.isJumping = true;
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
        gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
        playerSystemManager.SpecialTrigger.SetActive(true);
        var originalGravityScale = playerSystemManager.Rb2D.gravityScale;
        playerSystemManager.Rb2D.gravityScale = 0;
        playerSystemManager.Rb2D.velocity = Vector2.zero;
        //_trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashDuration);
        playerSystem.SetState(new Moving(playerSystem));
        playerSystemManager.Rb2D.gravityScale = originalGravityScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        playerSystemManager.SpecialTrigger.SetActive(false);
        //_trailRenderer.emitting = false;
        yield return new WaitForSeconds(playerSystemManager.DashCooldown);
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