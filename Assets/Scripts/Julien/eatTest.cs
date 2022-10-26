using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatTest : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    
    [Header("Eat")]
    public float reach = 1f;  
    [SerializeField, Range(0f, 1f)]
    private float filling = 0.12f;
    private bool canEat = true;
    [SerializeField, Range(0f, .5f)]
    private float eatCooldown = 0.5f;
    private Coroutine cooldownCoroutine;
    [SerializeField] private int maxCubeMangeable = 6;
    private int _cubeEated;

    private void EatCube(Cube_Edible cubeMangeable)
    {
        //canEat = false;
        //Debug.Log("eat");
        cubeMangeable.GetManged(this);
        ++_cubeEated;
        if (_cubeEated >= maxCubeMangeable)
        {
            canEat = false;
            StopAllCoroutines();
            cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        }
        _playerManager.eatAmount += filling;
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, 1f);
        //cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(!_playerManager.CanEat) {return;}
        
        Debug.Log("try Eat");
        if (!canEat)
        {
            print("I'm on eating cooldown !");
            
            return;
        }
        if (_playerManager.eatAmount >= 1f)
        {
            print("I'm full !!");
            return;
        }

        if (!other.transform.parent.CompareTag("CubeEdible")) return;
        
        Cube_Edible cubeMangeable;
        if (other.transform.parent && other.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
        {
            EatCube(cubeMangeable);
        }
        else
            print("Pas de Raf_CubeMangeable dans le cube vis�.");
    }

    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(eatCooldown);
        _cubeEated = 0;
        canEat = true;
        //StopAllCoroutines();
    }
}
