using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatTest : EatScript
{
    [Header("Script")] 
    [SerializeField] private EatScript eatScript;
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

        if (!other.transform.parent.CompareTag("CubeEdible") || !other.gameObject.activeSelf) return;
        
        Cube_Edible cubeMangeable;
        if (other.transform.parent && other.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
        {
            if (cubeEated >= maxCubeMangeable)
            {
                canEat = false;
                eatScript.StartCoroutine(eatScript.CooldownCoroutine());
            }
            else
            {
                ++cubeEated;
                Debug.Log("eat");
                eatScript.EatCube(cubeMangeable);
            }
        }
        else
            print("Pas de Raf_CubeMangeable dans le cube vis�.");
    }
}
