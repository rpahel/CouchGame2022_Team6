using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    private PlayerStateSystem _playerSystem;
    private FaceManager _faceManager;

    private Coroutine faceCoroutine;
    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerSystem = GetComponent<PlayerStateSystem>();
        _faceManager = GetComponent<FaceManager>();
    }

    public IEnumerator CooldownShoot()
    {
        _playerManager.canShoot = false;
        yield return new WaitForSeconds(_playerManager.CooldownShoot);
        _playerManager.canShoot = true;
    }

    public IEnumerator Braking()
    {
        float iniVelocityX = _playerSystem.PlayerManager.Rb2D.velocity.x;
        float t = 0;
        while (t < 1f)
        {

            _playerSystem.PlayerManager.Rb2D.velocity = new Vector2(DOVirtual.EasedValue(iniVelocityX, 0, t, Ease.OutCubic), _playerSystem.PlayerManager.Rb2D.velocity.y);
            t += Time.fixedDeltaTime / _playerSystem.PlayerManager.StopDuration;
            yield return new WaitForFixedUpdate();
        }

        //if (playerSystem.PlayerSystemManager.PlayerState != PLAYER_STATE.KNOCKBACKED && playerSystem.PlayerSystemManager.PlayerState == PLAYER_STATE.DASHING) //A quoi sert ca
        // playerSystem.PlayerSystemManager.Rb2D.velocity = new Vector2(0, playerSystem.PlayerSystemManager.Rb2D.velocity.y);

        _playerSystem.PlayerManager.brakingCoroutine = null;
    }

    public IEnumerator MoverOverAnimation(Vector2 endPosition)
    {
        Vector2 iniPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            if (_playerSystem.PlayerState is Knockback)
                break;

            _playerSystem.PlayerManager.Rb2D.velocity = Vector2.zero;
            transform.position = DOVirtual.EasedValue(iniPos, endPosition, t, Ease.OutBack, 2f);
            t += Time.deltaTime * 4f;
            yield return null;
        }

        if (_playerSystem.PlayerState is not Knockback)
        {
            transform.position = endPosition;
        }
    }

    public IEnumerator ActivateInputDelay(float duration)
    {
        //playerSystem.PlayerInputs.SetEnableInput(false);
        yield return new WaitForSeconds(duration);
        //playerSystem.PlayerInputs.SetEnableInput(true);
        _playerSystem.SetState(new Moving(_playerSystem));
    }

    public IEnumerator JumpCoroutine()
    {
        _playerManager.isJumping = true;
        SetupCoroutine(_faceManager.FaceJump);
        float t = 0;
        _playerManager.Rb2D.gravityScale = 0;
        while (t < 1)
        {
            if (_playerManager.CheckHeadBonk())
            {
                break;
            }

            if (_playerSystem.PlayerState is not Moving && _playerSystem.PlayerState is not AimShoot)
            {
                break;
            }

            _playerManager.Rb2D.velocity = new Vector2(_playerManager.Rb2D.velocity.x, Mathf.LerpUnclamped(_playerManager.JumpForce, 0, _playerManager.JumpCurve.Evaluate(t)));
            t += Time.fixedDeltaTime / _playerManager.JumpDuration;
            yield return new WaitForFixedUpdate();
        }
        _playerManager.isJumping = false;
        _playerManager.Rb2D.gravityScale = _playerManager.GravityScale;
        _playerManager.JumpCoroutine = null;
        yield break;
    }

    public void SetDashCoroutine()
    {
        StartCoroutine(DashCoroutine(_playerManager.charge + .1f));
    }

    private IEnumerator DashCoroutine(float dashDuration)
    {
        _playerManager.canDash = false;
        gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
        _playerManager.SpecialTrigger.SetActive(true);
        var originalGravityScale = _playerManager.Rb2D.gravityScale;
        _playerManager.Rb2D.gravityScale = 0;
        _playerManager.Rb2D.velocity = Vector2.zero;
        //_trailRenderer.emitting = true;

        float t = 0;
        while(t < dashDuration)
        {
            if (_playerSystem.PlayerState is not Special)
            {
                StartCoroutine(UndoDash(originalGravityScale));
                yield break;
            }

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _playerSystem.SetState(new Moving(_playerSystem));
        StartCoroutine(UndoDash(originalGravityScale));
    }

    private IEnumerator UndoDash(float originalGravityScale)
    {
        _playerManager.Rb2D.gravityScale = originalGravityScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        _playerManager.SpecialTrigger.SetActive(false);
        //_trailRenderer.emitting = false;
        yield return new WaitForSeconds(_playerManager.DashCooldown);
        _playerManager.canDash = true;
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
        _faceManager.SetFace(sprite);
        yield return new WaitForSeconds(_faceManager.CooldownFace);
        _faceManager.ResetFace();
    }
}