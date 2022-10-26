using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatScript : MonoBehaviour
{
    [SerializeField] protected PlayerManager _playerManager;
    
    [Header("Eat")]
    [SerializeField, Range(0f, 1f)]
    private float filling = 0.12f;
    protected bool canEat = true;
    [SerializeField] protected float eatCooldown = 0.5f;
    [SerializeField] protected int maxCubeMangeable = 6;
    protected int cubeEated;

    public void EatCube(Cube_Edible cubeMangeable)
    {
        ++cubeEated;
        cubeMangeable.GetManged(this);
        _playerManager.eatAmount += filling;
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, 1f);
    }

    public IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(eatCooldown);
        cubeEated = 0;
        canEat = true;
    }
}
