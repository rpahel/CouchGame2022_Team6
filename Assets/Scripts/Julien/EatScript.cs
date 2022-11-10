using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class EatScript : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private StrechAndSquash stretchEffect;
    
    [Header("Eat")]
    [SerializeField, Range(0f, 1f)]
    private float filling = 0.12f;
    private bool canEat = true;
    [SerializeField] private float eatCooldown = 0.5f;
    [SerializeField] private int maxCubeMangeable = 6;
    private int cubeEated;

    public void OnTriggerStayFunction(Collider2D other)
    {
        //Debug.Log("OnStay");
       // if (!_playerManager.CanEat) { return; }

        //Debug.Log("try Eat");
        /*if (!canEat)
        {
            print("I'm on eating cooldown !");

            return;
        }
        if (_playerManager.eatAmount >= 1f)
        {
            print("I'm full !!");
            return;
        }*/

        if (other.transform.parent.CompareTag("CubeEdible") && other.gameObject.activeSelf && _playerManager.CanEat && _playerManager.eatAmount < 1 && canEat) 
        {
            Cube_Edible cubeMangeable;
            if (other.transform.parent && other.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
            {
                if (cubeEated >= maxCubeMangeable)
                {
                    canEat = false;
                    StartCoroutine(CooldownCoroutine());
                }
                else
                {
                    //Debug.Log("eat");
                    stretchEffect.SquashEffectEat();
                    EatCube(cubeMangeable);
                }
            }
            else
                print("Pas de Raf_CubeMangeable dans le cube vis?.");
        }

        else if (other.CompareTag("Player") && _playerManager.CanEat && canEat)
        {
            var pj = other.gameObject.GetComponent<PlayerManager>();
            if (pj.State != PlayerState.Dead)
            {
                if (pj.SwitchSkin == SwitchSizeSkin.Little)
                {
                        Debug.LogError("Dead");
                        canEat = false;
                        StartCoroutine(CooldownCoroutine());
                        pj.SetDead2();
                }
            }
        }
    }
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
