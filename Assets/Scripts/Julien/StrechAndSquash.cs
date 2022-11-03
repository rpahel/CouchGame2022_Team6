using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class StrechAndSquash : MonoBehaviour
{
    private ScaleEat _scaleEat;
    private Transform _transform;
        
    [Header("JumpSquash")]
    [SerializeField] private float scaleMultiplierJump = 0.6f;
    [SerializeField] private float durationJump = 1f;
    
    [Header("EatSquash")]
    [SerializeField] private float scaleMultiplierEat = 1.3f;
    [SerializeField] private float durationEat = 0.3f;
    private void Awake()
    {
        _transform = transform;
        _scaleEat = GetComponent<ScaleEat>();
    }

    public void SquashEffectJump()
    {
        if(_transform.localScale.x <= 0.9 * _scaleEat.maxScale)
            StartCoroutine(SquashEffectJumpCoroutine());
    }
    
    public void SquashEffectEat()
    {
        if(_transform.localScale.x <= 0.9 * _scaleEat.maxScale)
            StartCoroutine(SquashEffectEatCoroutine());
    }
    
    private IEnumerator SquashEffectJumpCoroutine()
    {
        _scaleEat.canUpdateScale = false;
        var originalScale = _transform.localScale;
        _transform.DOScale(originalScale * scaleMultiplierJump, durationJump);
        yield return new WaitForSeconds(durationJump);
        _transform.DOScale(originalScale, durationJump);
        _scaleEat.canUpdateScale = true;
    }

    private IEnumerator SquashEffectEatCoroutine()
    {
        _scaleEat.canUpdateScale = false;
        var originalScale = _transform.localScale;
        _transform.DOScale(originalScale * scaleMultiplierEat, durationEat);
        yield return new WaitForSeconds(durationEat);
        _transform.DOScale(originalScale, scaleMultiplierEat);
        _scaleEat.canUpdateScale = true;
    }
}
